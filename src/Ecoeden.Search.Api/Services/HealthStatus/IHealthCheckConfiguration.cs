using App.Metrics.Health;

namespace Ecoeden.Search.Api.Services.HealthStatus;

public interface IHealthCheckConfiguration
{
    IRunHealthChecks HealthRunner { get; }
    int HealthCheckTimeoutSeconds { get; }
}
