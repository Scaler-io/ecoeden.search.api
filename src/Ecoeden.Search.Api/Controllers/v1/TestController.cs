using Asp.Versioning;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Providers;
using Ecoeden.Search.Api.Services.Pagination;
using Ecoeden.Search.Api.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace Ecoeden.Search.Api.Controllers.v1;

[ApiVersion("1")]
public class TestController(ILogger logger, 
    ISearchService<ProductSearchSummary> searchService, 
    IPaginatedSearchService<ProductSearchSummary> _paginatedSearchService,
    CatalogueApiProvider catalogueApiProvider
    ) 
    : ApiBaseController(logger)
{
    private readonly ISearchService<ProductSearchSummary> _searchService = searchService;
    private readonly IPaginatedSearchService<ProductSearchSummary> _paginatedSearchService = _paginatedSearchService;
    private readonly CatalogueApiProvider _catalogueApiProvider = catalogueApiProvider;

    [HttpPost]
    public async Task<IActionResult> TestGet()
    {
        ProductSearchSummary productSummary = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sample product",
            Category = "Sample category",
            ImageFile = "http://ecoeden.com/sampleImage",
            Slug = "sample-product",
            CreatedOn = DateTime.UtcNow,
            LastUpdatedOn = DateTime.UtcNow
        };

        ProductSearchSummary productSummary2 = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Bad product",
            Category = "Bad product category",
            ImageFile = "http://ecoeden.com/sampleImage",
            Slug = "bad-product",
            CreatedOn = DateTime.UtcNow,
            LastUpdatedOn = DateTime.UtcNow
        };

        var result = await _searchService.SeedDocumentAsync(productSummary, productSummary.Id, "product-search-index");
        var result2 = await _searchService.SeedDocumentAsync(productSummary2, productSummary2.Id, "product-search-index");

        return Ok("Done");
    }

    [HttpGet("count")]
    public async Task<IActionResult> TestPost([FromQuery]RequestQuery query)
    {
        var result = await _paginatedSearchService.GetCount(Guid.NewGuid().ToString(), "product-search-index", query);

        return Ok(result);
    }

    [HttpGet("products")]
    public async Task<IActionResult> TestGetProduct([FromQuery] RequestQuery query)
    {
        var result = await _paginatedSearchService.GetPaginatedData(query, Guid.NewGuid().ToString(), "product-search-index");
        return Ok(result);
    }

    [HttpGet("catalogue/identity")]
    public async Task<IActionResult> TestTokenEndpoint()
    {
        var result = await _catalogueApiProvider.GetProductCatalogues();
        return OkOrFailure(result);
    }
}
