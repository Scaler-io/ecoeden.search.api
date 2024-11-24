using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Constants;
using Ecoeden.Search.Api.Models.Contracts;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ecoeden.Search.Api.Providers;

public class CatalogueApiProvider(IHttpClientFactory httpClientFactory,
    IdentityServiceProvider identityServiceProvider,
    IOptions<ProviderConfigurationOption> providerSettings,
    ILogger logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IdentityServiceProvider _identityServiceProvider = identityServiceProvider;
    private readonly ProviderConfigurationOption _providerSettings = providerSettings.Value;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<Product>>> GetProductCatalogues()
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Making HTTP call to catalogue api endpoint");

        var client = _httpClientFactory.CreateClient(ApiProviderNames.CatalogueApi);
        var token = await _identityServiceProvider.GetAccessToken(_providerSettings, ApiProviderNames.CatalogueApi);

        if (string.IsNullOrEmpty(token))
        {
            _logger.Here().Error("Access token could not be generated");
            return Result<IEnumerable<Product>>.Failure(ErrorCodes.OperationFailed);
        }

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("api-version", "v2");

        _logger.Here().Information("{baseUrl} - {headers}", client.BaseAddress, client.DefaultRequestHeaders);

        var response = await client.GetAsync("products");

        if (!response.IsSuccessStatusCode)
        {
            _logger.Here().Error("Failed to load product catalogues {0} - {1}", response.StatusCode, response.ReasonPhrase);
            return Result<IEnumerable<Product>>.Failure(ErrorCodes.OperationFailed);
        }

        var data = await response.Content.ReadAsStringAsync();
        var jsonData = JsonConvert.DeserializeObject<IEnumerable<Product>>(data);

        _logger.Here().MethodExited();
        return Result<IEnumerable<Product>>.Success(jsonData);
    }
}
