using AutoMapper;
using Contracts.Events;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Ecoeden.Search.Api.Services.EventRecording;
using Ecoeden.Search.Api.Services.Factory;
using Ecoeden.Search.Api.Services.Pagination;
using Ecoeden.Search.Api.Services.Search;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Ecoeden.Search.Api.EventBus.Consumers.Stocks;

public class ProductStockCreatedConsumer(ILogger logger,
    ISearchServiceFactory searchServiceFactory,
    IOptions<ElasticSearchOption> elasticOptions,
    IEventRecorderService eventRecorderService
 ) : ConsumerBase<ProductStockCreated>(eventRecorderService), IConsumer<ProductStockCreated>
{
    private readonly ILogger _logger = logger;
    private readonly ElasticSearchOption _elasticSearchOption = elasticOptions.Value;
    private readonly ISearchService<StockSearchSummary> _searchService = searchServiceFactory.Create<StockSearchSummary>();
    private readonly IPaginatedSearchService<ProductSearchSummary> _productSearchService = searchServiceFactory.CreatePaginatedService<ProductSearchSummary>();
    private readonly IPaginatedSearchService<SupplierSearchSummary> _supplierSearchService = searchServiceFactory.CreatePaginatedService<SupplierSearchSummary>();

    public async Task Consume(ConsumeContext<ProductStockCreated> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
           .ForContext("MessageId", context.MessageId)
           .ForContext("Event", typeof(ProductStockCreated).Name)
           .WithCorrelationId(context.Message.CorrelationId)
           .Information("Message processing started for the event {type}", typeof(ProductStockCreated).Name);

        var summary = await PrepareStockSummary(context.Message);

        if (!summary.IsSuccess) return;
        var result = await _searchService.SeedDocumentAsync(summary.Data, summary.Data.Id, _elasticSearchOption.StockIndex);

        if (!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("Event", typeof(ProductStockCreated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);

            await RecordEvent(context, EventStatus.Failed);
        }

        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(ProductStockCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");

        _logger.Here().MethodExited();
    }

    private async Task<Result<StockSearchSummary>> PrepareStockSummary(ProductStockCreated message)
    {
        var productSearchTask = _productSearchService.GetPaginatedData(new()
        {
            IsFilteredQuery = true,
            Filters = new() { { "id", message.ProductId } },
            SortField = "lastUpdatedOn",
            SortOrder = "Asc"
        }, message.CorrelationId, _elasticSearchOption.ProductIndex);

        var supplierSearchTask = _supplierSearchService.GetPaginatedData(new()
        {
            IsFilteredQuery = true,
            Filters = new() { { "id", message.SupplierId } },
            SortField = "updatedOn",
            SortOrder = "Asc"
        }, message.CorrelationId, _elasticSearchOption.SupplierIndex);

        await Task.WhenAll(productSearchTask, supplierSearchTask);


        if(!productSearchTask.Result.IsSuccess || !supplierSearchTask.Result.IsSuccess )
        {
            _logger.Here().Error("Error fetching product or supplier details with product - {productId} and supplier - {supplierId}"
                    , message.ProductId, message.SupplierId);
            return Result<StockSearchSummary>.Failure(ErrorCodes.InternalServerError);
        }

        var product = productSearchTask.Result.Data.Data.FirstOrDefault();
        var supplier = supplierSearchTask.Result.Data.Data.FirstOrDefault();

        return Result<StockSearchSummary>.Success(new()
        {
            Id = message.Id,
            Product = new()
            {
                Id = product.Id,
                Name = product.Name,
            },
            Supplier = new()
            {
                Id = supplier.Id,
                Name = supplier.Name,
            },
            Category = product.Category,
            Quantity = message.Quantity,
            CreatedOn = message.CreatedOn,
            UpdatedOn = message.LastUpdatedOn,
        });
    }
}
