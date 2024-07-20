using Ecoeden.Search.Api.Models.Enums;

namespace Ecoeden.Search.Api.Models.Core;

public sealed class ApiExceptionResponse : ApiResponse
{
    public ApiExceptionResponse(string errorMessage = null, string stackTrace = "") : 
        base(ErrorCodes.InternalServerError, errorMessage)
    {
        StackTrace = stackTrace;
    }

    public string StackTrace { get; set; }
}
