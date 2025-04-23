using Quantum.Command.Command.Sink;

namespace Quantum.Dispatcher.Command;

public class NullDomainEventSink : IDomainEventSink
{
    public static IDomainEventSink Instance => new NullDomainEventSink();

    public Task Queue(IsAnIdentity EventStreamId, ICollection<IsADomainEvent> events)
        => Task.CompletedTask;
}