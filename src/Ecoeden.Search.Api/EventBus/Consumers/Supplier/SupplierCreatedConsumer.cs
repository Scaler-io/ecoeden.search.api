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

namespace Ecoeden.Search.Api.EventBus.Consumers.Supplier;

public class SupplierCreatedConsumer(ILogger logger,
    IMapper mapper,
    ISearchService<SupplierSearchSummary> searchService,
    IOptions<ElasticSearchOption> elasticOptions, 
    IEventRecorderService eventRecorderService) 
    : ConsumerBase<SupplierCreated>(eventRecorderService), IConsumer<SupplierCreated>
{
    private readonly ILogger _logger = logger;
    private readonly ISearchService<SupplierSearchSummary> _searchService = searchService;
    private readonly IMapper _mapper = mapper;
    private readonly ElasticSearchOption _elasticOptions = elasticOptions.Value;

    public async Task Consume(ConsumeContext<SupplierCreated> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(SupplierCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(SupplierCreated).Name);

        // store supplier to open document [Elastic search]
        var summary = _mapper.Map<SupplierSearchSummary>(context.Message);
        var result = await _searchService.SeedDocumentAsync(summary, summary.Id, _elasticOptions.SupplierIndex);

        if (!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId) 
                .ForContext("Event", typeof(SupplierCreated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);

            await RecordEvent(context, EventStatus.Failed);
        }

        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(SupplierCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
