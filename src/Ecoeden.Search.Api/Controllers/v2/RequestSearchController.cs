using Asp.Versioning;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Services.Factory;
using Ecoeden.Search.Api.Swagger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecoeden.Search.Api.Controllers.v2;

[ApiVersion("2")]
public class RequestSearchController(ILogger logger, ISearchServiceFactory factory) : ApiBaseController(logger)
{
    private readonly ISearchServiceFactory _factory = factory;

    [HttpPost("{indexName}")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "Search", Description = "searches open data from elastic search")]
    public async Task<IActionResult> Search([FromBody] RequestQuery query, [FromRoute] string indexName)
    {
        Logger.Here().MethodEntered();
        var response = indexName switch
        {
            var name when name.IsUserSearchIndex() => await HandleSearchAsync<UserSearchSummary>(query, indexName),
            var name when name.IsProductSearchIndex() => await HandleSearchAsync<ProductSearchSummary>(query, indexName),
            var name when name.IsSupplierSearchIndex() => await HandleSearchAsync<SupplierSearchSummary>(query, indexName),
            var name when name.IsCustomerSearchIndex() => await HandleSearchAsync<CustomerSearchSummary>(query, indexName),
            var name when name.IsUnitSearchIndex() => await HandleSearchAsync<UnitSearchSummary>(query, indexName),
            _ => BadRequest(new ApiValidationResponse("Invalid index name provided"))
        };      
        Logger.Here().MethodExited();
        return response;
    }

    [HttpPost("{indexName}/count")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SearchCount", Description = "Fetches the total document count from elastic search")]
    public async Task<IActionResult> SearchCount([FromRoute] string indexName, [FromBody] RequestQuery query = null)
    {
        Logger.Here().MethodEntered();
        var result = await ExecuteTypedCountAsync(indexName, query);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    private async Task<Result<Pagination<T>>> ExecuteSearchAsync<T>(RequestQuery query, string indexName) where T : class
    {
        var service = _factory.CreatePaginatedService<T>();
        return await service.GetPaginatedData(query, RequestInformation.CorrelationId, indexName);
    }

    private async Task<IActionResult> HandleSearchAsync<T>(RequestQuery query, string indexName) where T : class
    {
        var result = await ExecuteSearchAsync<T>(query, indexName);
        return OkOrFailure(result);
    }

    private async Task<Result<long>> ExecuteTypedCountAsync(string indexName, RequestQuery query)
    {
        if (indexName.IsProductSearchIndex())
            return await ExecuteCountAsync<ProductSearchSummary>(indexName, query);
        if (indexName.IsUserSearchIndex())
            return await ExecuteCountAsync<UserSearchSummary>(indexName, query);
        if (indexName.IsSupplierSearchIndex())
            return await ExecuteCountAsync<SupplierSearchSummary>(indexName, query);
        if (indexName.IsCustomerSearchIndex())
            return await ExecuteCountAsync<CustomerSearchSummary>(indexName, query);
        if (indexName.IsUnitSearchIndex())
            return await ExecuteCountAsync<UnitSearchSummary>(indexName, query);

        return Result<long>.Failure(Models.Enums.ErrorCodes.BadRequest, "Invalid index name provided");
    }

    private async Task<Result<long>> ExecuteCountAsync<T>(string indexName, RequestQuery query) where T : class
    {
        var service = _factory.CreatePaginatedService<T>();
        var result = await service.GetCount(RequestInformation.CorrelationId, indexName, query);
        return result;
    }

}
