using Ecoeden.Search.Api.Models.Enums;

namespace Ecoeden.Search.Api.EventBus.Contracts;

public abstract class GenericEvent
{
    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdatedOn { get; set; }
    public string CorrelationId { get; set; }
    public object? AdditionalProperties { get; set; }
    protected abstract GenericEventType GenericEventType { get; set; }
}
