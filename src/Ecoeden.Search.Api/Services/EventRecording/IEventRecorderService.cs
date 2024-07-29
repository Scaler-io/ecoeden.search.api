using Ecoeden.Search.Api.Entities.Sql;
using Ecoeden.Search.Api.Models.Core;

namespace Ecoeden.Search.Api.Services.EventRecording;

public interface IEventRecorderService
{
    Task<Result<EventPublishHistory>> GetEvent(string correlationId);
    Task<Result<bool>> CreateEvent(EventPublishHistory history);
    Task<Result<bool>> UpdateGenericEvent(EventPublishHistory history);
}
