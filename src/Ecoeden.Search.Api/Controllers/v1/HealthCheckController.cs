
using System.Net;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Services.HealthStatus;
using Ecoeden.Search.Api.Swagger;
using Ecoeden.Search.Api.Swagger.Examples;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Search.Api.Controllers.v1;

public class HealthCheckController(ILogger logger, IHealthCheckService healthCheckService) : ApiBaseController(logger)
{
    private readonly IHealthCheckService _healthCheckService = healthCheckService;

    [HttpGet("status")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "CheckApiHealth", Description = "checks for api health")]
    // 200
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(object))]
    // 500
    [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
    [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
    public async Task<IActionResult> CheckApiHealth()
    {
        Logger.Here().MethodEntered();
        var result = await _healthCheckService.CheckApiHealth(RequestInformation.CorrelationId);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }
}
