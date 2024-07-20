namespace Ecoeden.Search.Api.Configurations;

public sealed class AppConfigOption
{
    public const string OptionName = "AppConfigurations";
    public string ApplicationIdentifier { get; set; }
    public string ApplicationEnvironment { get; set; }
    public int HealthCheckTimeoutInSeconds { get; set; }
}
