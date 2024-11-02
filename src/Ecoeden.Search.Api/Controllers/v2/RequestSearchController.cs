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
        
        if (indexName.IsProductSearchIndex())
        {
            var result = await ExecuteSearchAsync<ProductSearchSummary>(query, indexName);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }
        else if (indexName.IsUserSearchIndex())
        {
            var result = await ExecuteSearchAsync<UserSearchSummary>(query, indexName);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }
        else if (indexName.IsSupplierSearchIndex())
        {
            var result = await ExecuteSearchAsync<SupplierSearchSummary>(query, indexName);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }

        Logger.Here().MethodExited();
        return BadRequest(new ApiValidationResponse("Invalid index name provided"));
    }

    [HttpPost("{indexName}/count")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SearchCount", Description = "Fetches the total document count from elastic search")]
    public async Task<IActionResult> SearchCount([FromRoute] string indexName, [FromBody] RequestQuery query = null)
    {
        Logger.Here().MethodEntered();
        if (indexName.IsProductSearchIndex())
        {
            var result = await ExecuteCountAsync<ProductSearchSummary>(indexName, query);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }
        if (indexName.IsUserSearchIndex())
        {
            var result = await ExecuteCountAsync<UserSearchSummary>(indexName, query);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }
        if(indexName.IsSupplierSearchIndex())
        {
            var result = await ExecuteCountAsync<SupplierSearchSummary>(indexName, query);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }

        Logger.Here().MethodExited();
        return BadRequest(new ApiValidationResponse("Invalid index name provided"));
    }

    private async Task<Result<Pagination<T>>> ExecuteSearchAsync<T>(RequestQuery query, string indexName) where T : class
    {
        var service = _factory.CreatePaginatedService<T>();
        var result = await service.GetPaginatedData(query, RequestInformation.CorrelationId, indexName);
        return result;
    }

    private async Task<Result<long>> ExecuteCountAsync<T>(string indexName, RequestQuery query) where T : class
    {
        var service = _factory.CreatePaginatedService<T>();
        var result = await service.GetCount(RequestInformation.CorrelationId, indexName, query);
        return result;
    }
}
