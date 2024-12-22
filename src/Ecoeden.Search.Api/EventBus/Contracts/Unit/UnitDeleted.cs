using Ecoeden.Search.Api.EventBus.Contracts;
using Ecoeden.Search.Api.Models.Enums;

namespace Contracts.Events;

public class UnitDeleted: GenericEvent
{
    public string Id { get; set; }
    protected override GenericEventType GenericEventType { get; set; } = GenericEventType.UnitDeleted;
}
