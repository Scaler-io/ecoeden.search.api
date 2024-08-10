using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Microsoft.Extensions.Options;
using Nest;

namespace Ecoeden.Search.Api.Services;

public class SearchBaseService : QueryBuilderBaseService
{
    protected ElasticClient ElasticsearchClient { get; }
    protected ILogger _logger;
    protected ElasticSearchOption _settings;

    protected SearchBaseService(ILogger logger, IOptions<ElasticSearchOption> options)
    {
        _logger = logger;
        _settings = options.Value;
        var elasticUri = new Uri(_settings.Uri);
        var connectionSetting = new ConnectionSettings(elasticUri);
        ElasticsearchClient = new ElasticClient(connectionSetting);
    }

    protected async Task<bool> CreateNewIndex<TDocument>(string index) where TDocument : class
    {
        _logger.Here().Information("Creating new index with name {index}", index);

        var createIndexResponse = await ElasticsearchClient.Indices.CreateAsync(index, c => c
            .Map<TDocument>(m => m
            .AutoMap())
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(0)
            ));

        return createIndexResponse.IsValid;
    }

    protected async Task<bool> IndexExist(string index)
    {
        _logger.Here().Information("No index found with name {index}", index);
        var indexResponse = await ElasticsearchClient.Indices.ExistsAsync(index);
        return indexResponse.Exists;
    }
}
