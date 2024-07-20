using Ecoeden.Search.Api.Configurations;

namespace Ecoeden.Search.Api.DI;

public static class ServiCollectionConfigurationExtensions
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoggingOption>(configuration.GetSection(LoggingOption.OptionName));
        services.Configure<AppConfigOption>(configuration.GetSection(AppConfigOption.OptionName));
        services.Configure<ElasticSearchOption>(configuration.GetSection(ElasticSearchOption.OptionName));

        return services;
    }
}
