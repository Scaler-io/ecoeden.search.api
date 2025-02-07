namespace Ecoeden.Search.Api.Models.Contracts.Stock;

public class ProductStock
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string SupplierId { get; set; }
    public long  Quantity { get; set; }
    public MetaData MetaData { get; set; }
}
