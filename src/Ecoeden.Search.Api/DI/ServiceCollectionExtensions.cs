using Asp.Versioning.ApiExplorer;
using Ecoeden.Search.Api.Middlewares;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Ecoeden.Search.Api.Services.Factory;
using Ecoeden.Search.Api.Services.Pagination;
using Ecoeden.Search.Api.Services.Search;
using Ecoeden.Search.Api.Swagger;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Search.Api.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, 
        SwaggerConfiguration swaggerConfiguration)
    {
        var environment = services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();
        var logger = Logging.GetLogger(configuration, environment);

        services.AddSingleton(logger);

        services.AddControllers()
            .AddNewtonsoftJson(configuration =>
            {
                configuration.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                configuration.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                configuration.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        services.AddHttpContextAccessor();

        services.AddTransient<CorrelationHeaderEnricher>()
            .AddTransient<RequestLoggerMiddleware>()
            .AddTransient<GlobalExceptionMiddleware>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerExamplesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        services.AddSwaggerExamples();
        services.AddSwaggerGen(option =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            swaggerConfiguration.SetupSwaggerGenService(option, provider);
        });

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // setup api versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = Asp.Versioning.ApiVersion.Default;
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = HandleFrameworkValidationFailure();
        });

        // custom service registration
        services.AddScoped<ISearchServiceFactory, SearchServiceFactory>();
        services.AddScoped(typeof(ISearchService<>), typeof(SearchService<>));
        services.AddScoped(typeof(IPaginatedSearchService<>), typeof(PaginatedSearchService<>));

        return services;
    }

    private static Func<ActionContext, IActionResult> HandleFrameworkValidationFailure()
    {
        return context =>
        {
            var errors = context.ModelState
            .Where(m => m.Value.Errors.Count > 0)
            .ToList();

            ApiValidationResponse validationError = new()
            {
                Errors = []
            };
            
            foreach (var error in errors)
            {
                FieldLevelError fieldLevelError = new()
                {
                    Code = ErrorCodes.BadRequest.ToString(),
                    Field = error.Key,
                    Message = error.Value?.Errors?.First().ErrorMessage
                };
                validationError.Errors.Add(fieldLevelError);
            }

            return new BadRequestObjectResult(validationError);
        };
    }
}
