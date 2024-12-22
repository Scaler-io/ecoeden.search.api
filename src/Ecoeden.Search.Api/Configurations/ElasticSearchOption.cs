namespace Ecoeden.Search.Api.Configurations;

public sealed class ElasticSearchOption
{
    public const string OptionName = "ElasticSearch";
    public string Uri { get; set; }
    public string ProductIndex { get; set; }
    public string UserIndex { get; set; }
    public string SupplierIndex { get; set; }
    public string CustomerIndex { get; set; }
    public string UnitIndex { get; set; }
}
