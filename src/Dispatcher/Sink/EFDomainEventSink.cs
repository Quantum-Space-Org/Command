using System.Linq;
using Quantum.DataBase.EntityFramework;

namespace Quantum.Command.Sink;

public class EFDomainEventSink : IDomainEventSink
{
    private readonly QuantumDbContext _dbContext;

    public EFDomainEventSink(QuantumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Queue(IsAnIdentity eventStreamId, ICollection<IsADomainEvent> events)
    {
        if (events == null || !events.Any())
        {
            return;
        }

        var queuedEvents = events.Select(e => new QueuedDomainEvents
        {
            StreamId = eventStreamId.ToString(),
            Type = e.GetType().Name,
            Name = e.GetId(),
            Payload = e.Serialize(),
            QueuedAt = DateTime.UtcNow
        }).ToList();

        _dbContext.Set<QueuedDomainEvents>().AddRange(queuedEvents);
        await _dbContext.SaveChangesAsync();
    }

}