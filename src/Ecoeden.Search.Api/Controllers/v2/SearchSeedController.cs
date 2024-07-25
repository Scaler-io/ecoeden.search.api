using Asp.Versioning;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Services.Factory;
using Ecoeden.Search.Api.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecoeden.Search.Api.Controllers.v2;

[ApiVersion("2")]
public class SearchSeedController(ILogger logger, IOptions<ElasticSearchOption> elasticOptions, ISearchServiceFactory factory) 
    : ApiBaseController(logger)
{
    private readonly ISearchServiceFactory _factory = factory;
    private readonly ElasticSearchOption _elasticSearchOption = elasticOptions.Value;

    [HttpPost("search/seed")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SeedElasticSearch", Description = "seeds elastic search odata store")]
    public async Task<IActionResult> SeedElasticSearch()
    {
        Logger.Here().MethodEntered();
        var productSearchService = _factory.Create<ProductSearchSummary>();
        var result = await productSearchService.SearchReIndex(_elasticSearchOption.ProductIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }
}
