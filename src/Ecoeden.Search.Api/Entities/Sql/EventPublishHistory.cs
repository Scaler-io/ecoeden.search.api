using Ecoeden.Search.Api.Models.Enums;

namespace Ecoeden.Search.Api.Entities.Sql;

public class EventPublishHistory : SqlBaseEntity
{
    public string EventType { get; set; }
    public string FailureSource { get; set; }
    public string Data { get; set; }
    public EventStatus EventStatus { get; set; }
}
