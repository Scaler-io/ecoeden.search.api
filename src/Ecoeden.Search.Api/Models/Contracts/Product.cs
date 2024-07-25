namespace Ecoeden.Search.Api.Models.Contracts;

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string ImageFile { get; set; }
    public decimal Price { get; set; }
    public string Slug { get; set; }
    public string Sku { get; set; }
    public MetaData MetaData { get; set; }
}


public class MetaData
{
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}