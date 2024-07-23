using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Constants;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ecoeden.Search.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}")]
public class ApiBaseController(ILogger logger) : ControllerBase
{
    protected ILogger Logger { get; set; } = logger;

    protected RequestInformation RequestInformation => new()
    {
        CorrelationId = GetOrGenerateCorrelationId()
    };

    private string GetOrGenerateCorrelationId() => Request?.GetRequestHeaderOrDefault("CorrelationId", $"GEN-{Guid.NewGuid()}");

    protected IActionResult OkOrFailure<T>(Result<T> result)
    {
        if (result == null) return NotFound(new ApiResponse(Models.Enums.ErrorCodes.NotFound));
        if (result.IsSuccess && result.Data == null) return NotFound(new ApiResponse(Models.Enums.ErrorCodes.NotFound));
        if (result.IsSuccess && result.Data != null) return Ok(result.Data);

        return result.ErrorCode switch
        {
            ErrorCodes.BadRequest => BadRequest(new ApiValidationResponse(result.ErrorMessage)),
            ErrorCodes.InternalServerError => InternalServerError(new ApiExceptionResponse(result.ErrorMessage)),
            ErrorCodes.NotFound => NotFound(new ApiResponse(ErrorCodes.NotFound, result.ErrorMessage)),
            ErrorCodes.Unauthorized => Unauthorized(new ApiResponse(ErrorCodes.Unauthorized, result.ErrorMessage)),
            ErrorCodes.OperationFailed => BadRequest(new ApiResponse(ErrorCodes.OperationFailed, result.ErrorMessage)),
            ErrorCodes.NotAllowed => BadRequest(new ApiResponse(ErrorCodes.NotAllowed, result.ErrorMessage)),
            _ => BadRequest(new ApiResponse(Models.Enums.ErrorCodes.BadRequest, ErrorMessages.BadRequest))
        };
    }

    private static ObjectResult InternalServerError(ApiResponse response)
    {
        return new ObjectResult(response)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            ContentTypes =
            [
                "application/json"
            ]
        };
    }

    protected IActionResult ProcessValidationResult(ValidationResult validationResult)
    {
        var errors = validationResult.Errors;
        var validationError = new ApiValidationResponse()
        {
            Errors = []
        };

        validationError.Errors.AddRange(
         errors.Select(error => new FieldLevelError
         {
             Code = error.ErrorCode,
             Field = error.PropertyName,
             Message = error.ErrorMessage
         })
        );

        return new BadRequestObjectResult(validationError);
    }

    public static bool IsInvalidResult(ValidationResult validationResult)
    {
        return validationResult != null && !validationResult.IsValid;
    }
}
