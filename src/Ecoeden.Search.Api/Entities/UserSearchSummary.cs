using Nest;

namespace Ecoeden.Search.Api.Entities;

public class UserSearchSummary
{
    public string Id { get; set; }
    public string UserName { get; set; }
    [Keyword(Name = "fullName")]
    public string FullName { get; set; }
    [Keyword(Name = "email")]
    public string Email { get; set; }
    public bool IsDefaultAdmin { get; set; }
    public bool IsActive { get; set; }
    public List<string> UserRoles { get; set; }
    public List<string> Permissions { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
