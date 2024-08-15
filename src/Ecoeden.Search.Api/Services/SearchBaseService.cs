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
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(0)
                .Analysis(a => a
                    .Tokenizers(t => t
                        .EdgeNGram("ngram_tokenizer", e => e
                            .MinGram(3)
                            .MaxGram(10)
                            .TokenChars(TokenChar.Letter, TokenChar.Digit, TokenChar.Symbol)
                        )
                    )
                    .Analyzers(an => an
                        .Custom("ngram_analyzer", ca => ca
                            .Tokenizer("ngram_tokenizer")
                        )
                    )
                )
            )
            .Map<TDocument>(m => m
            .AutoMap())
            .Map<TDocument>(m => CreateMapping(m))
        );

        return createIndexResponse.IsValid;
    }

    private static TypeMappingDescriptor<TDocument> CreateMapping<TDocument>(TypeMappingDescriptor<TDocument> m) where TDocument : class
    {
        if (typeof(TDocument) == typeof(ProductSearchSummary))
        {
            return m.Properties<ProductSearchSummary>(p => p
                .Keyword(k => k
                    .Name(n => n.Category)
                )
                .Text(t => t
                    .Name(n => n.Name)
                    .Analyzer("ngram_analyzer")
                )
                .Text(t => t
                    .Name(n => n.Slug)
                    .Analyzer("ngram_analyzer")
                )
            );
        }

        return m.Properties<UserSearchSummary>(p => p
            .Keyword(k => k
                .Name(n => n.UserRoles)
            )
            .Text(t => t
                .Name(n => n.Email)
                .Analyzer("ngram_analyzer")
            )
            .Text(t => t
                .Name(n => n.FullName)
                .Analyzer("ngram_analyzer")
            )
            .Text(t => t
                .Name(n => n.UserName)
                .Analyzer("ngram_analyzer")
            )
        );
    }

    protected async Task<bool> IndexExist(string index)
    {
        _logger.Here().Information("No index found with name {index}", index);
        var indexResponse = await ElasticsearchClient.Indices.ExistsAsync(index);
        return indexResponse.Exists;
    }
}
