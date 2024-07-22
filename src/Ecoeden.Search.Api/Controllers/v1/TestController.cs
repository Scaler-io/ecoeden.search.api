using Asp.Versioning;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace Ecoeden.Search.Api.Controllers.v1;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}")]
public class TestController : ControllerBase
{
    private readonly ISearchService<ProductSearchSummary> _searchService;

    public TestController(ISearchService<ProductSearchSummary> searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> TestGet()
    {
        ProductSearchSummary productSummary = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sample product",
            Category = "Sample category",
            ImageFile = "http://ecoeden.com/sampleImage",
            Slug = "sample-product",
            CreatedOn = DateTime.UtcNow.ToString("dd/MM/yyyy"),
            LastUpdatedOn = DateTime.UtcNow.ToString("dd/MM/yyyy")
        };

        var result = await _searchService.SeedDocumentAsync(productSummary, productSummary.Id, "product-search-index");

        return Ok(result);
    }
}
