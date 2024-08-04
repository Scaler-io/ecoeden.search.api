using App.Metrics.Health;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Extensions;
using Microsoft.Extensions.Options;

namespace Ecoeden.Search.Api.Services.HealthStatus;

public sealed class HealthCheckConfiguration(ILogger logger,
    IEnumerable<IHealthCheck> healthChecks,
    IOptions<AppConfigOption> appOptions) : IHealthCheckConfiguration
{
    public IRunHealthChecks HealthRunner { get; } = AppMetricsHealth.CreateDefaultBuilder()
        .HealthChecks
        .AddChecks(healthChecks.Select(healthCheck => CreateHealthCheck(healthCheck, logger)))
        .Build()
        .HealthCheckRunner;

    public int HealthCheckTimeoutSeconds { get; } = appOptions.Value.HealthCheckTimeoutInSeconds;

    private static HealthCheck CreateHealthCheck(IHealthCheck healthCheck, ILogger logger)
    {
        return new HealthCheck(healthCheck.GetType().Name, async () =>
        {
            try
            {
                return await healthCheck.CheckIsHealthyAsync();
            }
            catch (Exception ex)
            {
                logger.Here().Error("{Exception}:", $"Health check failure {ex}");
                return HealthCheckResult.Unhealthy();
            }
        });
    }
}
