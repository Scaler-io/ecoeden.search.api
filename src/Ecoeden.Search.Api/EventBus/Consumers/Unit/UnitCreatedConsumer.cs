using AutoMapper;
using Contracts.Events;
using Ecoeden.Search.Api.Configurations;
using Ecoeden.Search.Api.Entities;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Enums;
using Ecoeden.Search.Api.Services.EventRecording;
using Ecoeden.Search.Api.Services.Search;
using Microsoft.Extensions.Options;
using MassTransit;

namespace Ecoeden.Search.Api.EventBus.Consumers.Unit;

public class UnitCreatedConsumer(ILogger logger,
    IMapper mapper,
    ISearchService<UnitSearchSummary> searchService,
    IOptions<ElasticSearchOption> elasticOptions,
    IEventRecorderService eventRecorderService
) : ConsumerBase<UnitCreated>(eventRecorderService), IConsumer<UnitCreated>
{
    private readonly ILogger _logger = logger;
    private readonly ISearchService<UnitSearchSummary> _searchService = searchService;
    private readonly IMapper _mapper = mapper;
    private readonly ElasticSearchOption _elasticOptions = elasticOptions.Value;

    public async Task Consume(ConsumeContext<UnitCreated> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(UnitCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(UnitCreated).Name);

        var summary = _mapper.Map<UnitSearchSummary>(context.Message);
        var result = await _searchService.SeedDocumentAsync(summary, summary.Id, _elasticOptions.UnitIndex);

        if (!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("Event", typeof(UnitCreated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);

            await RecordEvent(context, EventStatus.Failed);
        }

        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(UnitCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
