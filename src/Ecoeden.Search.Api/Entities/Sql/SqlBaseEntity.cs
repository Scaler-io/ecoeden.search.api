namespace Ecoeden.Search.Api.Entities.Sql;

public class SqlBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    public string CorrelationId { get; set; }
}
