using Ecoeden.Search.Api.Services.Search;

namespace Ecoeden.Search.Api.Services.Factory;

public interface ISearchServiceFactory
{
    ISearchService<TDocument> Create<TDocument>() where TDocument : class;
}
