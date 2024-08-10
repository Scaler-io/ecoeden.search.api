using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Constants;
using Ecoeden.Search.Api.Models.Contracts;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Ecoeden.Search.Api.Providers;

public class UserApiProvider(IHttpClientFactory httpClientFactory,
    IdentityServiceProvider identityServiceProvider,
    IOptions<ProviderConfigurationOption> providerSettings,
    ILogger logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IdentityServiceProvider _identityServiceProvider = identityServiceProvider;
    private readonly ProviderConfigurationOption _providerSettings = providerSettings.Value;
    private readonly ILogger _logger = logger;

    public async Task<Result<IEnumerable<User>>> GetUsers()
    {
        _logger.Here().MethodEntered();
        _logger.Here().Information("Making HTTP call to user api endpoint");

        var client = _httpClientFactory.CreateClient(ApiProviderNames.UserApi);
        var token = await _identityServiceProvider.GetAccessToken(_providerSettings, ApiProviderNames.UserApi);

        if (string.IsNullOrEmpty(token))
        {
            _logger.Here().Error("Access token could not be generated");
            return Result<IEnumerable<User>>.Failure(ErrorCodes.OperationFailed);
        }

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("api-version", "v2");

        var response = await client.GetAsync("");

        if (!response.IsSuccessStatusCode)
        {
            _logger.Here().Error("Failed to load users list {0} - {1}", response.StatusCode, response.ReasonPhrase);
            return Result<IEnumerable<User>>.Failure(ErrorCodes.OperationFailed);
        }

        var data = await response.Content.ReadAsStringAsync();
        var jsonData = JsonConvert.DeserializeObject<IEnumerable<User>>(data);

        _logger.Here().MethodExited();
        return Result<IEnumerable<User>>.Success(jsonData);
    }
}
