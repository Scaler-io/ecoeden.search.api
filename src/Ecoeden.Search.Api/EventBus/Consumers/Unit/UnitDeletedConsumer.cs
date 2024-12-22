using AutoMapper;
using Contracts.Events;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Enums;
using Ecoeden.Search.Api.Services.EventRecording;
using Ecoeden.Search.Api.Services.Search;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Ecoeden.Search.Api.EventBus.Consumers.Unit;

public class UnitDeletedConsumer(ILogger logger,
    IMapper mapper,
    ISearchService<UnitSearchSummary> searchService,
    IOptions<ElasticSearchOption> elasticOptions,
    IEventRecorderService eventRecorderService) : ConsumerBase<UnitDeleted>(eventRecorderService), IConsumer<UnitDeleted>
{
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly ElasticSearchOption _elasticOptions = elasticOptions.Value;
    private readonly ISearchService<UnitSearchSummary> _searchService = searchService;

    public async Task Consume(ConsumeContext<UnitDeleted> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(UnitDeleted).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(UnitDeleted).Name);

        var summary = _mapper.Map<UnitSearchSummary>(context.Message);
        var result = await _searchService.RemoveDocumentAsync(new() { { "id", context.Message.Id } }, _elasticOptions.UnitIndex);

        if (!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("Event", typeof(UnitDeleted).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);

            await RecordEvent(context, EventStatus.Failed);
        }

        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(UnitDeleted).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
