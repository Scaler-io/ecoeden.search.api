namespace Ecoeden.Search.Api.Entities;
public class SearchableProduct
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public string ImageFile { get; set; }
    public string Slug { get; set; }
    public string SKU { get; set; }
    public string CreatedOn { get; set; }
    public string LastUpdatedOn { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
