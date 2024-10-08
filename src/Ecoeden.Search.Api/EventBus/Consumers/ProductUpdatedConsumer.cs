﻿using AutoMapper;
using Contracts.Events;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Enums;
using Ecoeden.Search.Api.Services.EventRecording;
using Ecoeden.Search.Api.Services.Search;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Ecoeden.Search.Api.EventBus.Consumers;

public class ProductUpdatedConsumer(ILogger logger,
    IMapper mapper,
    ISearchService<ProductSearchSummary> searchService,
    IOptions<ElasticSearchOption> elasticOptions,
    IEventRecorderService eventRecorderService) : ConsumerBase<ProductUpdated>(eventRecorderService), IConsumer<ProductUpdated>
{
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly ISearchService<ProductSearchSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticOptions = elasticOptions.Value;

    public async Task Consume(ConsumeContext<ProductUpdated> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(ProductCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(ProductCreated).Name);

        // update product to open document [Elastic search]
        var summary = _mapper.Map<ProductSearchSummary>(context.Message);

        var result = await _searchService.UpdateDocumentAsync(summary, new() { { "id", summary.Id } }, _elasticOptions.ProductIndex);

        if (!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("Event", typeof(ProductCreated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);

            await RecordEvent(context, EventStatus.Failed);
        }

        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(ProductCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
