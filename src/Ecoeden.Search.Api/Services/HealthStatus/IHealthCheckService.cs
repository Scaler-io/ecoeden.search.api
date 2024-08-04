using Ecoeden.Search.Api.Models.Contracts;
using Ecoeden.Search.Api.Models.Core;

namespace Ecoeden.Search.Api.Services.HealthStatus;

public interface IHealthCheckService
{
    Task<Result<HealthCheckDto>> CheckApiHealth(string correlationId);
}
