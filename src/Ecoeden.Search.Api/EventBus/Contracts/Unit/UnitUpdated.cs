using Ecoeden.Search.Api.EventBus.Contracts;
using Ecoeden.Search.Api.Models.Enums;

namespace Contracts.Events;
public class UnitUpdated: GenericEvent
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Status { get; set; }
    protected override GenericEventType GenericEventType { get; set; } = GenericEventType.UnitUpdated;
}
