using System.Collections.Concurrent;
using System.Linq;

namespace Quantum.Command.Sink;

public class InMemoryDomainEventSink : IDomainEventSink
{
    private readonly ConcurrentDictionary<IsAnIdentity, List<IsADomainEvent>> _eventStore = new();

    public async Task Queue(IsAnIdentity eventStreamId, ICollection<IsADomainEvent> events)
    {
        if (events == null || !events.Any())
        {
            return;
        }

        _eventStore.AddOrUpdate(
            eventStreamId,
            events.ToList(),
            (_, existingEvents) =>
            {
                var updatedList = existingEvents.ToList();
                updatedList.AddRange(events);
                return updatedList;
            });
        await Task.CompletedTask;
    }

    public ICollection<IsADomainEvent> GetEvents(IsAnIdentity eventStreamId)
    {
        if (_eventStore.TryGetValue(eventStreamId, out var events))
        {
            return events.ToList();
        }
        return new List<IsADomainEvent>();
    }

    public void Clear()
    {
        _eventStore.Clear();
    }
}