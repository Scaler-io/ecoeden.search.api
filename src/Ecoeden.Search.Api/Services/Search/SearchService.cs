using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Mappers;
using Ecoeden.Search.Api.Models.Constants;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Ecoeden.Search.Api.Providers;
using Ecoeden.Search.Api.Services.Factory;
using Ecoeden.Search.Api.Services.Pagination;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;

namespace Ecoeden.Search.Api.Services.Search;

public class SearchService<TDocument>(ILogger logger,
    CatalogueApiProvider catalogueApiProvider,
    UserApiProvider userApiProvider,
    InventoryApiProvider inventoryApiProvider,
    IOptions<ElasticSearchOption> options,
    ISearchServiceFactory searchServiceFactory)
    : SearchBaseService(logger, options), ISearchService<TDocument> where TDocument : class
{
    private readonly CatalogueApiProvider _catalogueApiProvider = catalogueApiProvider;
    private readonly UserApiProvider _userApiProvider = userApiProvider;
    private readonly InventoryApiProvider _inventoryApiProvider = inventoryApiProvider;
    private readonly ElasticSearchOption _elasticSearchOption = options.Value;
    private readonly IPaginatedSearchService<ProductSearchSummary> _productSearchService = searchServiceFactory.CreatePaginatedService<ProductSearchSummary>();
    private readonly IPaginatedSearchService<SupplierSearchSummary> _supplierSearchService = searchServiceFactory.CreatePaginatedService<SupplierSearchSummary>();

    public async Task<Result<bool>> SeedDocumentAsync(TDocument document, string id, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Requesting - storing data to elastic search {index}", index);

        if (!await IndexExist(index))
        {
            await CreateNewIndex<TDocument>(index);
        }

        var indexResponse = await ElasticsearchClient.IndexAsync(document, idx => idx.Index(index)
            .Id(id)
            .Refresh(Refresh.WaitFor));

        if (!indexResponse.IsValid)
        {
            _logger.Here().Error("Data seeding to elastic serach index {@index} failed", index);
            return Result<bool>.Failure(ErrorCodes.OperationFailed, ErrorMessages.Operationfailed);
        }

        _logger.Here().Information("Data seeding completed successfully for document {id}", id);
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);

    }

    public async Task<Result<bool>> UpdateDocumentAsync(TDocument document, Dictionary<string, string> fieldValue, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - update document in elastic search {@index}", index);

        var documentResponse = await ElasticsearchClient
                .SearchAsync<TDocument>(s => s.Index(index)
                .Query(q => q.Match(m => m
                    .Field(fieldValue.Keys.First())
                    .Query(fieldValue.Values.First())
                )));

        var docId = documentResponse.Hits.First().Id;
        var data = documentResponse.Documents.First();
        var documentUpdateResponse = await ElasticsearchClient.UpdateAsync<TDocument, object>(
                new DocumentPath<TDocument>(docId).Index(index),
                u => u.Doc(document)
                    .DocAsUpsert()
            );
        var docData = documentUpdateResponse.Result;

        if (!documentUpdateResponse.IsValid)
        {
            _logger.Here().Error("Elastic search docuemnt update failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Elastic document update failed");
        }

        _logger.Here().Information("Elastic document update successfull");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SearchReIndex(string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - reindex search data for {@index}", index);

        if (await IndexExist(index))
        {
            var deleteResposne = await ElasticsearchClient.Indices.DeleteAsync(index);
            if (!deleteResposne.IsValid)
            {
                _logger.Here().Error("Index deletion failed");
                return Result<bool>.Failure(ErrorCodes.OperationFailed, "Index deletion failed");
            }
        }

        await CreateNewIndex<TDocument>(index);

        BulkResponse bulkResponse = new();

        switch (index)
        {
            case "product-search-index":
                var productSearchSummaries = await GetProductsAsync();
                bulkResponse = await ElasticsearchClient.BulkAsync(b => b.Index(index).IndexMany(productSearchSummaries));
                break;
            case "user-search-index":
                var userSearchSummaries = await GetUsersAsync();
                bulkResponse = await ElasticsearchClient.BulkAsync(b => b.Index(index).IndexMany(userSearchSummaries));
                break;
            case "supplier-search-index":
                var supplierSearchSummaries = await GetSuppliersAsync();
                bulkResponse = await ElasticsearchClient.BulkAsync(b => b.Index(index).IndexMany(supplierSearchSummaries));
                break;
            case "customer-search-index":
                var customerSearchSummaries = await GetCustomersAsync();
                bulkResponse = await ElasticsearchClient.BulkAsync(b => b.Index(index).IndexMany(customerSearchSummaries));
                break;
            case "unit-search-index":
                var unitSearchSummaries = await GetUnitsAsync();
                bulkResponse = await ElasticsearchClient.BulkAsync(b => b.Index(index).IndexMany(unitSearchSummaries));
                break;
            case "stock-search-index":
                var stockSearchSummaries = await GetStocksAsync();
                bulkResponse = await ElasticsearchClient.BulkAsync(b => b.Index(index).IndexMany(stockSearchSummaries));
                break;
            default:
                break;
        }

        if (!bulkResponse.IsValid)
        {
            _logger.Here().Error("Re-indexing failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Re-index operation failed");
        }

        _logger.Here().Information("Re-index for {index} successful", index);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveDocumentAsync(Dictionary<string, object> query, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Request - remove data from elastic search {index}", index);

        if (!await IndexExist(index))
        {
            return Result<bool>.Failure(ErrorCodes.OperationFailed, $"Elastic operation failed, index {index} was not found");
        }

        var fieldName = query.Keys.First();
        var fieldValue = query.Values.First();

        var deleteResponse = await ElasticsearchClient.DeleteByQueryAsync<TDocument>(d => d
                .Index(index)
                .Query(q => q
                .Match(m => m.Field(fieldName).Query(fieldValue.ToString()))));

        if (!deleteResponse.IsValid)
        {
            _logger.Here().Error("Elastic search delete operation failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Elastic document delete operation failed");
        }

        _logger.Here().Information("Elastic document delete successful");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    private async Task<IEnumerable<ProductSearchSummary>> GetProductsAsync()
    {
        var results = await _catalogueApiProvider.GetProductCatalogues();
        return ProductMapper.Map(results.Data);
    }

    private async Task<IEnumerable<UserSearchSummary>> GetUsersAsync()
    {
        var results = await _userApiProvider.GetUsers();
        return UserMapper.Map(results.Data);
    }

    private async Task<IEnumerable<SupplierSearchSummary>> GetSuppliersAsync()
    {
        var results = await _inventoryApiProvider.GetSuppliers();
        return SupplierMapper.Map(results.Data);
    }

    private async Task<IEnumerable<CustomerSearchSummary>> GetCustomersAsync()
    {
        var results = await _inventoryApiProvider.GetCustomers();
        return CustomerMapper.Map(results.Data);
    }

    private async Task<IEnumerable<UnitSearchSummary>> GetUnitsAsync()
    {
        var results = await _inventoryApiProvider.GetUnits();
        return UnitMapper.Map(results.Data);
    }

    private async Task<IEnumerable<StockSearchSummary>> GetStocksAsync()
    {
        var result = await _inventoryApiProvider.GetProductStocks();

        var stocks = result.Data;
        List<StockSearchSummary> stockSummaries = [];

        foreach (var stock in stocks)
        {
            var productTask = _productSearchService.GetPaginatedData(new()
            {
                IsFilteredQuery = true,
                Filters = new() { { "id", stock.ProductId } },
            }, Guid.NewGuid().ToString(), _elasticSearchOption.ProductIndex);

            var supplierTask = _supplierSearchService.GetPaginatedData(new()
            {
                IsFilteredQuery = true,
                Filters = new() { { "id", stock.SupplierId } },
                SortField = "updatedOn",
                SortOrder = "Asc"
            }, Guid.NewGuid().ToString(), _elasticSearchOption.SupplierIndex);

            await Task.WhenAll(productTask, supplierTask);

            if (!productTask.Result.IsSuccess || !supplierTask.Result.IsSuccess)
            {
                _logger.Here().Error("Error fetching product or supplier details with product - {productId} and supplier - {supplierId}"
                    , stock.ProductId, stock.SupplierId);
                continue;
            }
            var product = productTask.Result.Data;
            var supplier = supplierTask.Result.Data;

            stockSummaries.Add(StockMapper.Map(stock: stock, product: product.Data[0], supplier: supplier.Data[0]));
        }

        return stockSummaries;
    }
}
