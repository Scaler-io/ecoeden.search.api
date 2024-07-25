using Ecoeden.Search.Api.Services.Pagination;
using Ecoeden.Search.Api.Services.Search;

namespace Ecoeden.Search.Api.Services.Factory;

public interface ISearchServiceFactory
{
    ISearchService<TDocument> Create<TDocument>() where TDocument : class;
    IPaginatedSearchService<TDocument> CreatePaginatedService<TDocument>() where TDocument: class;
}
