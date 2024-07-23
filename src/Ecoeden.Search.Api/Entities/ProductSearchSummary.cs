namespace Ecoeden.Search.Api.Entities;
public class ProductSearchSummary
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string ImageFile { get; set; }
    public string Slug { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdatedOn { get; set; }
}
