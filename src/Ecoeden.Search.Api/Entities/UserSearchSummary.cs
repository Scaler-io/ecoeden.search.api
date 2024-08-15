using Nest;

namespace Ecoeden.Search.Api.Entities;

public class UserSearchSummary
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool IsDefaultAdmin { get; set; }
    public bool IsActive { get; set; }
    [Keyword(Name = "userRoles")]
    public List<string> UserRoles { get; set; }
    public List<string> Permissions { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
