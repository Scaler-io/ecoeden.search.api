using Ecoeden.Search.Api.EventBus.Contracts;
using Ecoeden.Search.Api.Models.Enums;

namespace Contracts.Events;

public class ProductUpdated : GenericEvent
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string ImageFile { get; set; }
    public string Slug { get; set; }
    protected override GenericEventType GenericEventType { get; set; } = GenericEventType.ProductUpdated;
}
