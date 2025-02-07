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

    [HttpPost("seed/products")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SeedProducts", Description = "seeds elastic search odata store")]
    public async Task<IActionResult> SeedProducts()
    {
        Logger.Here().MethodEntered();
        var productSearchService = _factory.Create<ProductSearchSummary>();
        var result = await productSearchService.SearchReIndex(_elasticSearchOption.ProductIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("seed/users")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SeedUsers", Description = "seeds elastic search odata store")]
    public async Task<IActionResult> SeedUsers()
    {
        Logger.Here().MethodEntered();
        var usertSearchService = _factory.Create<UserSearchSummary>();
        var result = await usertSearchService.SearchReIndex(_elasticSearchOption.UserIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("seed/suppliers")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SeedSuppliers", Description = "seeds elastic search odata store")]
    public async Task<IActionResult> SeedSuppliers()
    {
        Logger.Here().MethodEntered();
        var supplierSearchService = _factory.Create<SupplierSearchSummary>();
        var result = await supplierSearchService.SearchReIndex(_elasticSearchOption.SupplierIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("seed/customers")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SeedCustomers", Description = "seeds elastic search odata store")]
    public async Task<IActionResult> SeedCustomers()
    {
        Logger.Here().MethodEntered();
        var customerSearchService = _factory.Create<CustomerSearchSummary>();
        var result = await customerSearchService.SearchReIndex(_elasticSearchOption.CustomerIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("seed/units")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SeedUnits", Description = "seeds elastic search odata store")]
    public async Task<IActionResult> SeedUnits()
    {
        Logger.Here().MethodEntered();
        var unitSearchService = _factory.Create<UnitSearchSummary>();
        var result = await unitSearchService.SearchReIndex(_elasticSearchOption.UnitIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("seed/stocks")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "SeedStocks", Description = "seeds elastic search odata store")]
    public async Task<IActionResult> SeedStocks()
    {
        Logger.Here().MethodEntered();
        var stcokSearchService = _factory.Create<StockSearchSummary>();
        var result = await stcokSearchService.SearchReIndex(_elasticSearchOption.StockIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }
}