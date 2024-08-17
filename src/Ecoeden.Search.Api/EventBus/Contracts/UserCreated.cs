using Ecoeden.Search.Api.EventBus.Contracts;
using Ecoeden.Search.Api.Models.Enums;

namespace Contracts.Events;

public class UserCreated : GenericEvent
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool IsDefaultAdmin { get; set; }
    public bool IsActive { get; set; }
    public List<string> UserRoles { get; set; }
    public List<string> Permissions { get; set; }
    public DateTime LastLogin { get; set; }
    protected override GenericEventType GenericEventType { get; set; } = GenericEventType.UserCreated;
}
