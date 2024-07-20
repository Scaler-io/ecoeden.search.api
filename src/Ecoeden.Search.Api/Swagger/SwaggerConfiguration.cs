using Asp.Versioning.ApiExplorer;
using Ecoeden.Search.Api.Models.Constants;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Ecoeden.Search.Api.Swagger;

public class SwaggerConfiguration
{
    private const string DefaultScheme = "http";
    private const string DefaultEnvironmentName = "";
    private readonly string _apiName;
    private readonly string _apiDescription;
    private static bool _isDevelopment;
    private static string _apiHost;

    public SwaggerConfiguration(string apiName, string apiDescription, string apiHost, bool isDevelopment)
    {
        _apiName = apiName;
        _apiDescription = apiDescription;
        _apiHost = apiHost;
        _isDevelopment = isDevelopment;
    }

    public static string ExtractApiNameFromEnvironment()
    {
        var environmentName = Environment.GetEnvironmentVariable(EnvironmentConstants.SwaggerEnvironmentName) ?? DefaultEnvironmentName;
        var apiName = $"Search API {environmentName}".Trim();
        return apiName;
    }

    public static void SetupSwaggerOptions(SwaggerOptions options)
    {
        options.SerializeAsV2 = false;
        var scheme = Environment.GetEnvironmentVariable(EnvironmentConstants.SwaggerScheme)?.ToLowerInvariant() ?? DefaultScheme;
        options.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers.Clear();
            swagger.Servers.Add(new OpenApiServer { Url = $"{scheme}://{_apiHost}" });
            swagger.Servers.Add(new() { Url = $"https://{_apiHost}" });
        });
    }

    public void SetupSwaggerGenService(SwaggerGenOptions options, IApiVersionDescriptionProvider serviceProvider)
    {
        foreach (var description in serviceProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = _apiName,
                Version = description.GroupName,
                Description = _apiDescription
            });
        }

        options.DocumentFilter<SwaggerBasePath>();
        options.DocumentFilter<SwaggerRemoveVersionFromRoute>();
        options.UseInlineDefinitionsForEnums();

        options.ExampleFilters();
        options.OperationFilter<SwaggerHeaderFilter>();
        options.SchemaFilter<EnumSchemaFilter>();
        options.CustomSchemaIds(x => x.FullName);
        if (_isDevelopment)
        {
            options.OperationFilter<SwaggerApiVersionFilter>();
        }
    }

    public static void SetupSwaggerUiOptions(SwaggerUIOptions options, IApiVersionDescriptionProvider serviceProvider)
    {
        foreach (var description in serviceProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"Search API - {description.GroupName.ToUpperInvariant()}");
        }
    }
}
