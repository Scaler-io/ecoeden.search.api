namespace Ecoeden.Search.Api.Models.Contracts.Supplier;

public class Supplier
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ContactDetails ContactDetails { get; set; }
    public bool Status { get; set; } = false;
    public MetaData MetaData { get; set; }
}
