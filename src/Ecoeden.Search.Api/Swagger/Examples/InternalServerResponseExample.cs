using Ecoeden.Search.Api.Models.Constants;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Search.Api.Swagger.Examples;

public class InternalServerResponseExample : IExamplesProvider<ApiExceptionResponse>
{
    public ApiExceptionResponse GetExamples()
    {
        return new()
        {
            Code = ErrorCodes.InternalServerError,
            ErrorMessage = ErrorMessages.InternalServerError,
            StackTrace = "server exception stack trace"
        };
    }
}
