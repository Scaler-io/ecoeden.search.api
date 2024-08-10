namespace Ecoeden.Search.Api.Configurations;

public class ProviderConfigurationOption
{
    public const string OptionName = "ProviderSettings";
    public CatalogueApiSettings CatalogueApiSettings { get; set; }
    public UserApiSettings UserApiSettings { get; set; }
}

public class ApiSettings
{
    public string BaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
    public string SubscriptionKey { get; set; }
}

public class CatalogueApiSettings : ApiSettings
{
}

public class UserApiSettings : ApiSettings
{
}