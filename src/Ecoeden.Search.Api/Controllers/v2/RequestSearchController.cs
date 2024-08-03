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
        var service = indexName.IsProductSearchIndex() ? _factory.CreatePaginatedService<ProductSearchSummary>() : null;
        var result = await service.GetPaginatedData(query, RequestInformation.CorrelationId, indexName);    
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("{indexName}/count")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SearchCount", Description = "Fetches the total document count from elastic search")]
    public async Task<IActionResult> SearchCount([FromBody] RequestQuery query, [FromRoute] string indexName)
    {
        Logger.Here().MethodEntered();
        var service = indexName.IsProductSearchIndex() ? _factory.CreatePaginatedService<ProductSearchSummary>() : null;
        var result = await service.GetCount(RequestInformation.CorrelationId, indexName, null);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }
}
