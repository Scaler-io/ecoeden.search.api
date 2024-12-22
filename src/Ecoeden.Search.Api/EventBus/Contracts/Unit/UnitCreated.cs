using Ecoeden.Search.Api.EventBus.Contracts;
using Ecoeden.Search.Api.Models.Enums;

namespace Contracts.Events;
public class UnitCreated : GenericEvent
{
    public string Id { get; set; }
    public string Name { get; set; }
    protected override GenericEventType GenericEventType { get; set; } = GenericEventType.UnitCreated;
}
