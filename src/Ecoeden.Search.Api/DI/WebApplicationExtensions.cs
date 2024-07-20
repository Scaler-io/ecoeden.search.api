using Asp.Versioning.ApiExplorer;
using Ecoeden.Search.Api.Middlewares;
using Ecoeden.Search.Api.Swagger;

namespace Ecoeden.Search.Api.DI;

public static class WebApplicationExtensions
{
    public static WebApplication AddApplicationPipelines(this WebApplication app, bool isDevelopment)
    {
        if (isDevelopment)
        {
            app.UseSwagger(SwaggerConfiguration.SetupSwaggerOptions);
            app.UseSwaggerUI(option =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                SwaggerConfiguration.SetupSwaggerUiOptions(option, provider);
            });
        }

        app.UseMiddleware<CorrelationHeaderEnricher>()
            .UseMiddleware<RequestLoggerMiddleware>()
            .UseMiddleware<GlobalExceptionMiddleware>();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
