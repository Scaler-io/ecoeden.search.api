using App.Metrics.Health;
using Ecoeden.Search.Api.Data;

namespace Ecoeden.Search.Api.Services.HealthStatus;

public class SqlDbHealthCheck(EcoedenDbContext context) : IHealthCheck
{
    public async ValueTask<HealthCheckResult> CheckIsHealthyAsync()
    {
        if (await context.Database.CanConnectAsync())
        {
            return HealthCheckResult.Healthy();
        }
        return HealthCheckResult.Unhealthy();
    }
}
