using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Extensions;
using IdentityModel.Client;

namespace Ecoeden.Search.Api.Providers;

public class IdentityServiceProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = logger;

    public async Task<string> GetAccessToken(ProviderConfigurationOption providerOption, string requestedClientName)
    {
        var client = _httpClientFactory.CreateClient();
        var authority = _configuration["IdentityServiceUrl"];
        var discoveryDocument = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = authority,
            Policy = new DiscoveryPolicy { RequireHttps = false, ValidateIssuerName = false, ValidateEndpoints = false }
        });

        if (discoveryDocument.IsError)
        {
            throw new HttpRequestException(discoveryDocument.Error);
        }

        var response = await client.RequestClientCredentialsTokenAsync(GetTokenRequest(requestedClientName, providerOption, discoveryDocument));

        _logger.Here().Information("token response {@token}", response);

        if (response.IsError)
        {
            throw new HttpRequestException(response.Error);
        }

        return response.AccessToken;


    }

    private static ClientCredentialsTokenRequest GetTokenRequest(string requestedClientName, ProviderConfigurationOption providerOption, DiscoveryDocumentResponse discoveryDocument)
    {
        return requestedClientName switch
        {
            "CatalogueApi" => new()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = providerOption.CatalogueApiSettings.ClientId,
                ClientSecret = providerOption.CatalogueApiSettings.ClientSecret,
                Scope = providerOption.CatalogueApiSettings.Scope,
            },
            "UserApi" => new()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = providerOption.UserApiSettings.ClientId,
                ClientSecret = providerOption.UserApiSettings.ClientSecret,
                Scope = providerOption.UserApiSettings.Scope
            },
            "InventoryApi" => new()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = providerOption.InventoryApiSettings.ClientId,
                ClientSecret = providerOption.InventoryApiSettings.ClientSecret,
                Scope = providerOption.InventoryApiSettings.Scope
            },
            _ => throw new NotImplementedException()
        };
    }
}
