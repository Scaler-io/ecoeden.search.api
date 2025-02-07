namespace Ecoeden.Search.Api.Entities.Sql;

public class ProductStock: SqlBaseEntity
{
    public string ProductId { get; set; }
    public string SupplierId { get; set; }
    public long Quantity { get; set; } = 0;
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
