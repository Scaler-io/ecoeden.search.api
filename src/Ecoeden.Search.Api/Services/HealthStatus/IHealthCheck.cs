using App.Metrics.Health;

namespace Ecoeden.Search.Api.Services.HealthStatus;

public interface IHealthCheck
{
    ValueTask<HealthCheckResult> CheckIsHealthyAsync();
}
