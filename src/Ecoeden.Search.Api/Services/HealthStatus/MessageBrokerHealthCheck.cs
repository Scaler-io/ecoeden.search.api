using App.Metrics.Health;
using Contracts.Events;
using MassTransit;

namespace Ecoeden.Search.Api.Services.HealthStatus;

public class MessageBrokerHealthCheck(IBusControl busControl) : IHealthCheck
{
    private readonly IBusControl _busControl = busControl;

    public async ValueTask<HealthCheckResult> CheckIsHealthyAsync()
    {
        try
        {
            var endpoint = await _busControl.GetSendEndpoint(new Uri("rabbitmq://localhostdd/search-health-check"));
            // Optionally, you can send a lightweight message to ensure the connection is fully working
            await endpoint.Send(new ProductCreated());

            return HealthCheckResult.Healthy("RabbitMQ is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ is unhealthy", ex);
        }
    }
}
