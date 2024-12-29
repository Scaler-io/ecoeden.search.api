using Ecoeden.Search.Api.Data;
using Ecoeden.Search.Api.Entities.Sql;
using Ecoeden.Search.Api.Extensions;
using Ecoeden.Search.Api.Models.Core;
using Ecoeden.Search.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecoeden.Search.Api.Services.EventRecording;

public class EventRecorderService(ILogger logger, EcoedenDbContext context) : IEventRecorderService
{
    private readonly ILogger _logger = logger;
    private readonly EcoedenDbContext _context = context;

    public async Task<Result<EventPublishHistory>> GetEvent(string correlationId)
    {
        _logger.Here().MethodEntered();
        var result = await _context.EventPublishHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.CorrelationId == correlationId);

        if (result is null)
        {
            _logger.Here().WithCorrelationId(correlationId).Error("{0} - No event was found", ErrorCodes.NotFound);
            return Result<EventPublishHistory>.Failure(ErrorCodes.NotFound);
        }

        _logger.Here().WithCorrelationId(correlationId).Information("History record found with {correlationId}", correlationId);
        _logger.Here().MethodExited();
        return Result<EventPublishHistory>.Success(result);
    }

    public async Task<Result<bool>> CreateEvent(EventPublishHistory history)
    {
        _logger.Here().MethodEntered();
        _context.EventPublishHistories.Add(history);
        var result = await _context.SaveChangesAsync();
        if (result < 1)
        {
            _logger.Here().WithCorrelationId(history.CorrelationId).Error("{0} - no recored was created", ErrorCodes.OperationFailed);
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        _logger.Here().Information("Record is created successfullt");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateGenericEvent(EventPublishHistory history)
    {
        _logger.Here().MethodEntered();
        _context.Set<EventPublishHistory>().Attach(history);
        _context.Entry(history).State = EntityState.Modified;
        _context.EventPublishHistories.Update(history);
        var result = await _context.SaveChangesAsync();
        if (result < 1)
        {
            _logger.Here().WithCorrelationId(history.CorrelationId).Error("{0} - no recored was updated", ErrorCodes.OperationFailed);
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        _logger.Here().Information("Record is updated successfully");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }
}
