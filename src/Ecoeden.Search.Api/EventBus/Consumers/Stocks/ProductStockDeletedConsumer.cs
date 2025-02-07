using Contracts.Events;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Services.EventRecording;
using Ecoeden.Search.Api.Services.Search;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Ecoeden.Search.Api.EventBus.Consumers.Stocks;

public class ProductStockDeletedConsumer(IEventRecorderService eventRecorderService, ILogger logger,
    ISearchService<StockSearchSummary> searchService,
    IOptions<ElasticSearchOption> elasticOptions
) : ConsumerBase<ProductStockDeleted>(eventRecorderService), IConsumer<ProductStockDeleted>
{
    private readonly ILogger _logger = logger;
    private readonly ISearchService<StockSearchSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticOptions = elasticOptions.Value;

    public async Task Consume(ConsumeContext<ProductStockDeleted> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(ProductStockDeleted).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(ProductStockDeleted).Name);


        var result = await _searchService.RemoveDocumentAsync(new() { { "id", context.Message.Id } }, _elasticOptions.StockIndex);

        if (!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("Event", typeof(ProductStockDeleted).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);
            await RecordEvent(context, Models.Enums.EventStatus.Failed);
        }

        await RecordEvent(context, Models.Enums.EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(ProductStockDeleted).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
