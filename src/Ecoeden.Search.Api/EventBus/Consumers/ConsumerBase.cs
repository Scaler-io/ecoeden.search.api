using Ecoeden.Search.Api.Entities.Sql;
using Ecoeden.Search.Api.EventBus.Contracts;
using Ecoeden.Search.Api.Models.Enums;
using Ecoeden.Search.Api.Services.EventRecording;
using MassTransit;
using Newtonsoft.Json;

namespace Ecoeden.Search.Api.EventBus.Consumers;

public class ConsumerBase<TEvent>(IEventRecorderService eventRecorderService) where TEvent : GenericEvent
{
    private readonly IEventRecorderService _eventRecorderService = eventRecorderService;

    protected async Task RecordEvent(ConsumeContext<TEvent> context, EventStatus status)
    {
        var recordExists = await _eventRecorderService.GetEvent(context.Message.CorrelationId);
        if (!recordExists.IsSuccess)
        {
            var jsonData = JsonConvert.SerializeObject(context.Message);
            EventPublishHistory publishHistory = new()
            {
                CorrelationId = context.Message.CorrelationId,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                EventStatus = status,
                FailureSource = "Ecoeden.Search.Api",
                EventType = typeof(TEvent).Name,
                Data = jsonData
            };
            await _eventRecorderService.CreateEvent(publishHistory);
        }
        else
        {
            recordExists.Data.EventStatus = status;
            await _eventRecorderService.UpdateGenericEvent(recordExists.Data);
        }
    }
}
