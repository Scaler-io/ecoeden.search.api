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

namespace Ecoeden.Search.Api.EventBus.Consumers.Customer;

public class CustomerUpdatedConsumer(IEventRecorderService eventRecorderService,
    ILogger logger,
    IMapper mapper,
    ISearchService<CustomerSearchSummary> searchService,
    IOptions<ElasticSearchOption> elasticOptions
) : ConsumerBase<CustomerUpdated>(eventRecorderService), IConsumer<CustomerUpdated>
{
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly ISearchService<CustomerSearchSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticOptions = elasticOptions.Value;

    public async Task Consume(ConsumeContext<CustomerUpdated> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(CustomerUpdated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(CustomerUpdated).Name);

        // update customer to open document [Elastic search]
        var summary = _mapper.Map<CustomerSearchSummary>(context.Message);
        var result = await _searchService.UpdateDocumentAsync(summary, new() { { "id", summary.Id } }, _elasticOptions.CustomerIndex);

        if (!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("Event", typeof(CustomerUpdated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);

            await RecordEvent(context, EventStatus.Failed);
        }

        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("Event", typeof(CustomerUpdated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
