using Nest;

namespace Ecoeden.Search.Api.Entities;

public class SupplierSearchSummary
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
