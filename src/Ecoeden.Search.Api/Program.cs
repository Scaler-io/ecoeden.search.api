using Ecoeden.Search.Api;
using Ecoeden.Search.Api.DI;
using Ecoeden.Search.Api.Swagger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var apiName = SwaggerConfiguration.ExtractApiNameFromEnvironment();
var apiDescription = builder.Configuration["ApiDescription"];
var apiHost = builder.Configuration["ApiOriginHost"];
var swaggerConfiguration = new SwaggerConfiguration(apiName, apiDescription, apiHost, builder.Environment.IsDevelopment());

var logger = Logging.GetLogger(builder.Configuration, builder.Environment);
builder.Host.UseSerilog(logger);

builder.Services.AddApplicationServices(builder.Configuration, swaggerConfiguration)
    .ConfigureOptions(builder.Configuration)
    .ConfigureHttpClients(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ecoedencors", policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowCredentials().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.AddApplicationPipelines(app.Environment.IsDevelopment());

try
{
    await app.RunAsync();
}
finally
{
    Log.CloseAndFlush();
}
