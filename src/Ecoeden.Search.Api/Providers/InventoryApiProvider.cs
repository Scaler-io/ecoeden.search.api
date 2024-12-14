using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Constants;
using Ecoeden.Search.Api.Models.Contracts.Customer;
using Ecoeden.Search.Api.Models.Contracts.Supplier;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ecoeden.Search.Api.Providers;

public class InventoryApiProvider(IHttpClientFactory httpClientFactory,
    IdentityServiceProvider identityServiceProvider,
    IOptions<ProviderConfigurationOption> providerSettings,
    ILogger logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IdentityServiceProvider _identityServiceProvider = identityServiceProvider;
    private readonly ProviderConfigurationOption _providerSettings = providerSettings.Value;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<Supplier>>> GetSuppliers()
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Making HTTP call to supplier endpoint");

        var client = _httpClientFactory.CreateClient(ApiProviderNames.InventoryApi);
        var token = await _identityServiceProvider.GetAccessToken(_providerSettings, ApiProviderNames.InventoryApi);

        if (string.IsNullOrEmpty(token))
        {
            _logger.Here().Error("Access token could not be generated");
            return Result<IEnumerable<Supplier>>.Failure(ErrorCodes.OperationFailed);
        }

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("api-version", "v1");

        _logger.Here().Information("base url from settings - {url}", _providerSettings.InventoryApiSettings.BaseUrl);
        _logger.Here().Information("{baseUrl} - {headers}", client.BaseAddress, client.DefaultRequestHeaders);

        var response = await client.GetAsync("supplier");

        if (!response.IsSuccessStatusCode)
        {
            _logger.Here().Error("Failed to load suppliers {0} - {1}", response.StatusCode, response.ReasonPhrase);
            return Result<IEnumerable<Supplier>>.Failure(ErrorCodes.OperationFailed);
        }

        var data = await response.Content.ReadAsStringAsync();
        var jsonData = JsonConvert.DeserializeObject<IEnumerable<Supplier>>(data);

        _logger.Here().MethodExited();
        return Result<IEnumerable<Supplier>>.Success(jsonData);
    }

    public async Task<Result<IEnumerable<Customer>>> GetCustomers()
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Making HTTP call to customer endpoint");

        var client = _httpClientFactory.CreateClient(ApiProviderNames.InventoryApi);
        var token = await _identityServiceProvider.GetAccessToken(_providerSettings, ApiProviderNames.InventoryApi);

        if (string.IsNullOrEmpty(token))
        {
            _logger.Here().Error("Access token could not be generated");
            return Result<IEnumerable<Customer>>.Failure(ErrorCodes.OperationFailed);
        }

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("api-version", "v1");

        _logger.Here().Information("base url from settings - {url}", _providerSettings.InventoryApiSettings.BaseUrl);
        _logger.Here().Information("{baseUrl} - {headers}", client.BaseAddress, client.DefaultRequestHeaders);

        var response = await client.GetAsync("customer");

        if (!response.IsSuccessStatusCode)
        {
            _logger.Here().Error("Failed to load customers {0} - {1}", response.StatusCode, response.ReasonPhrase);
            return Result<IEnumerable<Customer>>.Failure(ErrorCodes.OperationFailed);
        }

        var data = await response.Content.ReadAsStringAsync();
        var jsonData = JsonConvert.DeserializeObject<IEnumerable<Customer>>(data);

        _logger.Here().MethodExited();
        return Result<IEnumerable<Customer>>.Success(jsonData);
    }
}
