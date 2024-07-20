﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecoeden.Search.Api.Swagger;

public class SwaggerHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var headers = context.MethodInfo.GetCustomAttributes(true).OfType<SwaggerHeaderAttribute>();
        operation.Parameters = [];

        foreach(var header in headers)
        {
            operation.Parameters.Add(new()
            {
                Name = header.Name,
                In = ParameterLocation.Header,
                Description = header.Description,
                Schema = new OpenApiSchema { Type = header.Type ?? "string" },
                Required = header.Required
            });
        }
    }
}
