using Ecoeden.Search.Api.Configurations;
using IdentityModel.Client;

namespace Ecoeden.Search.Api.Providers;

public class IdentityServiceProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> GetCatalogueAccessToken(CatalogueApiSettings apiSettings)
    {
        var client = _httpClientFactory.CreateClient();

        var discoveryDocument = await client.GetDiscoveryDocumentAsync(_configuration["IdentityServiceUrl"]);

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

        if (response.IsError)
        {
            throw new HttpRequestException(response.Error);
        }

        return response.AccessToken;
    }
}
