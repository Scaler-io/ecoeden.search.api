using App.Metrics.Health;
using Ecoeden.Search.Api.Configurations;
using Microsoft.Extensions.Options;
using Nest;

namespace Ecoeden.Search.Api.Services.HealthStatus;

public class ElasticClientHealthCheck : IHealthCheck
{
    private readonly ElasticClient _elasticCLient;
    public ElasticClientHealthCheck(IOptions<ElasticSearchOption> option)
    {
        var uri = new Uri(option.Value.Uri);
        var connectionString = new ConnectionSettings(uri);
        _elasticCLient = new ElasticClient(connectionString);
    }

    public async ValueTask<HealthCheckResult> CheckIsHealthyAsync()
    {
        var pingResponse = await _elasticCLient.PingAsync();
        if (!pingResponse.IsValid) return HealthCheckResult.Unhealthy();

        return HealthCheckResult.Healthy();
    }
}
