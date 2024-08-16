using Ecoeden.Search.Api.Models.Core;

namespace Ecoeden.Search.Api.Services.Pagination;

public interface IPaginatedSearchService<TDocument> where TDocument : class
{
    Task<Result<Pagination<TDocument>>> GetPaginatedData(RequestQuery query, string correlationId, string searchIndex);
    Task<Result<long>> GetCount(string CorrelationId, string searchIndex);
}
