using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Extensions;
using IdentityModel.Client;

namespace Ecoeden.Search.Api.Providers;

public class IdentityServiceProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = logger;

    public async Task<string> GetCatalogueAccessToken(CatalogueApiSettings apiSettings)
    {
        var client = _httpClientFactory.CreateClient();

        var discoveryDocument = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _configuration["IdentityServiceUrl"],
            Policy = new DiscoveryPolicy { RequireHttps = false, ValidateIssuerName = false, ValidateEndpoints = false }      
        });

        if (discoveryDocument.IsError)
        {
            throw new HttpRequestException(discoveryDocument.Error);
        }

        var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = discoveryDocument.TokenEndpoint,
            ClientId = apiSettings.ClientId,
            ClientSecret = apiSettings.ClientSecret,
            Scope = apiSettings.Scope,
        });

        _logger.Here().Information("token response {@token}", response);

        if (response.IsError)
        {
            throw new HttpRequestException(response.Error);
        }

        return response.AccessToken;
    }
}
