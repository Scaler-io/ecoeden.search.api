using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecoeden.Search.Api.Swagger;

public class SwaggerApiVersionFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Api-Version",
            In = ParameterLocation.Header,
            Schema = new OpenApiSchema { Type = "string" },
            Description = "Version of the API. Example v1",
            Required = true
        });
    }
}
