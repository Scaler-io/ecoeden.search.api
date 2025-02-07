namespace Ecoeden.Search.Api.Entities;

public class StockSearchSummary
{
    public string Id { get; set; }
    public SearchableProduct Product { get; set; }
    public SearchableSupplier Supplier { get; set; }
    public string Category { get; set; }
    public long Quantity { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}

public class SearchableProduct
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class SearchableSupplier
{
    public string Id { get; set; }
    public string Name { get; set; }
}
