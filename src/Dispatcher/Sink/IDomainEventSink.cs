namespace Quantum.Command.Sink;

public interface IDomainEventSink
{
    Task Queue(IsAnIdentity EventStreamId, ICollection<IsADomainEvent> events);
}