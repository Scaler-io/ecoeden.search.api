using Ecoeden.Search.Api.Services.Search;

namespace Ecoeden.Search.Api.Services.Factory;

public class SearchServiceFactory(IServiceProvider serviceProvider) : ISearchServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    public ISearchService<TDocument> Create<TDocument>() where TDocument : class
    {
        using var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider;

        return service.GetRequiredService<ISearchService<TDocument>>();
    }
}
