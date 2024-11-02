using Ecoeden.Search.Api.EventBus.Contracts;
using Ecoeden.Search.Api.Models.Enums;

namespace Contracts.Events;

public class SupplierDeleted : GenericEvent 
{
    public string Id { get; set; }
    protected override GenericEventType GenericEventType { get; set; } = GenericEventType.SupplierUpdated;
}
