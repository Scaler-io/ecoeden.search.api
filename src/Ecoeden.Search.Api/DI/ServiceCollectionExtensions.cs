using Asp.Versioning.ApiExplorer;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.EventBus.Consumers;
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
using MassTransit;
using Ecoeden.Search.Api.Data;
using Microsoft.EntityFrameworkCore;
using Ecoeden.Search.Api.Services.EventRecording;
using Ecoeden.Search.Api.Services.HealthStatus;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ecoeden.Search.Api.Services.EventProcessor;

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

        services.AddScoped<IHealthCheckConfiguration, HealthCheckConfiguration>();
        services.AddScoped<IHealthCheck, SqlDbHealthCheck>();
        services.AddScoped<IHealthCheck, MessageBrokerHealthCheck>();
        services.AddScoped<IHealthCheck, ElasticClientHealthCheck>();
        services.AddScoped<IHealthCheckService, HealthCheckService>();

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

        services.AddMassTransit(config =>
        {
            config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
            config.AddConsumersFromNamespaceContaining<ProductCreatedConsumer>();
            config.UsingRabbitMq((context, cfg) =>
            {
                var rabbitmq = configuration.GetSection(EventBusOptions.OptionName).Get<EventBusOptions>();
                cfg.Host(rabbitmq.Host, "/", host =>
                {
                    host.Username(rabbitmq.Username);
                    host.Password(rabbitmq.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        //  sql server dabase services
        services.AddDbContext<EcoedenDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Sqlserver"), options => 
                options.MigrationsHistoryTable("__EFMigrationsHistory", "ecoeden.event"));
        }); 
        services.AddScoped<IEventRecorderService, EventRecorderService>();

        //services.AddHostedService<EventProcessorBackgroundService>();

        services.AddOpenTelemetry()
            .ConfigureResource(options => options.AddService(configuration["AppConfigurations:ApplicationIdentifier"]))
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
                .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .AddZipkinExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["Zipkin:Url"]);
                });
            });


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
