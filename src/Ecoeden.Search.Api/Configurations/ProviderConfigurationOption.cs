namespace Ecoeden.Search.Api.Configurations;

public class ProviderConfigurationOption
{
    public const string OptionName = "ProviderSettings";
    public CatalogueApiSettings CatalogueApiSettings { get; set; }
}


public class CatalogueApiSettings
{
    public string BaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
}