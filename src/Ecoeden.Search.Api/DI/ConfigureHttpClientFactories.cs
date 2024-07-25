using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Models.Constants;
using Ecoeden.Search.Api.Providers;

namespace Ecoeden.Search.Api.DI;

public static class ConfigureHttpClientFactories
{
    public static IServiceCollection ConfigureHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var providerSettings = configuration.GetSection(ProviderConfigurationOption.OptionName)
            .Get<ProviderConfigurationOption>();

        services.AddHttpClient(ApiProviderNames.CatalogueApi, client =>
        {
            client.BaseAddress = new Uri(providerSettings.CatalogueApiSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddTransient<IdentityServiceProvider>();
        services.AddTransient<CatalogueApiProvider>();

        return services;
    }
}
