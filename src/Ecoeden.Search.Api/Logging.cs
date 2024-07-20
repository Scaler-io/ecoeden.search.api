using Destructurama;
using Ecoeden.Search.Api.Configurations;
using Serilog.Events;
using Serilog;

namespace Ecoeden.Search.Api;

public class Logging
{
    public static ILogger GetLogger(IConfiguration configuration, IWebHostEnvironment env)
    {
        var loggingOptions = configuration.GetSection(LoggingOption.OptionName).Get<LoggingOption>();
        var appConfigurations = configuration.GetSection(AppConfigOption.OptionName).Get<AppConfigOption>();
        var elasticOptions = configuration.GetSection(ElasticSearchOption.OptionName).Get<ElasticSearchOption>();
        var logIndexPattern = $"Ecoeden.Search.Api-{env.EnvironmentName}";

        Enum.TryParse(loggingOptions.Console.LogLevel, false, out LogEventLevel minimumEventLevel);

        var loggerConfiguartion = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(new Serilog.Core.LoggingLevelSwitch(minimumEventLevel))
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty(nameof(Environment.MachineName), Environment.MachineName)
            .Enrich.WithProperty(nameof(appConfigurations.ApplicationIdentifier), appConfigurations.ApplicationIdentifier)
            .Enrich.WithProperty(nameof(appConfigurations.ApplicationEnvironment), appConfigurations.ApplicationEnvironment);

        if (loggingOptions.Console.Enabled)
        {
            loggerConfiguartion.WriteTo.Console(minimumEventLevel, loggingOptions.LogOutputTemplate);
        }
        if (loggingOptions.Elastic.Enabled)
        {
            loggerConfiguartion.WriteTo.Elasticsearch(elasticOptions.Uri, logIndexPattern);
        }

        return loggerConfiguartion
            .Destructure
            .UsingAttributes()
            .CreateLogger();
    }
}
