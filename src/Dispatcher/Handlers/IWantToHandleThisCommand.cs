namespace Quantum.Command.Handlers;

using System.Collections.Immutable;
using System.Linq;

public abstract class IWantToHandleThisCommand<TCommand>
    where TCommand : IAmACommand
{
    private List<IsADomainEvent> QueuedDomainEvents { get; } = [];

    public IsAnIdentity EventStreamId { get; protected set; }

    public abstract Task Handle(TCommand command);

    public virtual Task Compensate(TCommand command)
        => Task.CompletedTask;

    public void EnQueue(IsAnIdentity eventStreamId, ImmutableList<IsADomainEvent> domainEvents)
    {
        EventStreamId = eventStreamId;
        QueuedDomainEvents.AddRange(domainEvents);
    }

    public List<IsADomainEvent> DeQueueDomainEvents()
    {
        var result = QueuedDomainEvents.ToList();

        QueuedDomainEvents.Clear();

        return result;
    }
}