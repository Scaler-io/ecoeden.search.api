using App.Metrics.Health;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Contracts;
using Ecoeden.Search.Api.Models.Core;

namespace Ecoeden.Search.Api.Services.HealthStatus;

public class HealthCheckService(ILogger logger, IHealthCheckConfiguration healthCheckConfiguration) : IHealthCheckService
{
    private readonly ILogger _logger = logger;
    private readonly IHealthCheckConfiguration _healthCheckConfiguration = healthCheckConfiguration;

    public async Task<Result<HealthCheckDto>> CheckApiHealth(string correlationId)
    {
        _logger.Here().MethodEntered();
        var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_healthCheckConfiguration.HealthCheckTimeoutSeconds));
        var response = await _healthCheckConfiguration.HealthRunner.ReadAsync(timeoutTokenSource.Token);

        var isHealthy = response.Status == HealthCheckStatus.Healthy;
        if (!isHealthy)
        {
            foreach (var result in response.Results)
            {
                _logger.Here().WithCorrelationId(correlationId)
                    .Error("{Message}", $"health check: {result.Name} : {result.Check.Status}");
            }
        }

        _logger.Here().WithCorrelationId(correlationId)
            .Information("{Message}", $"health check completed. Status: {response.Status}");

        HealthCheckDto healthCheckDto = new()
        {
            IsHealthy = isHealthy
        };

        _logger.Here().MethodExited();
        return Result<HealthCheckDto>.Success(healthCheckDto);
    }
}
