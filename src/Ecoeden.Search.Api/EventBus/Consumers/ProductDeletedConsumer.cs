﻿using Contracts.Events;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Services.EventRecording;
using Ecoeden.Search.Api.Services.Search;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Ecoeden.Search.Api.EventBus.Consumers;

public class ProductDeletedConsumer(ILogger logger,
    ISearchService<ProductSearchSummary> searchService,
    IOptions<ElasticSearchOption> elasticOptions,
    IEventRecorderService eventRecorderService) : ConsumerBase<ProductDeleted>(eventRecorderService), IConsumer<ProductDeleted>
{
    private readonly ILogger _logger = logger;
    private readonly ISearchService<ProductSearchSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticOptions = elasticOptions.Value;

    public async Task Consume(ConsumeContext<ProductDeleted> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(ProductCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(ProductCreated).Name);

        // update product to open document [Elastic search]

        var result = await _searchService.RemoveDocumentAsync(new() { { "id", context.Message.Id } }, _elasticOptions.ProductIndex);

        if (!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("Event", typeof(ProductCreated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);
            await RecordEvent(context, Models.Enums.EventStatus.Failed);
        }

        await RecordEvent(context, Models.Enums.EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(ProductCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
