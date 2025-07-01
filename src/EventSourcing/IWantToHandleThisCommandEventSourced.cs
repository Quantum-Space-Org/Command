using System;
using System.Linq;
using System.Threading.Tasks;
using Quantum.Command.Handlers;
using Quantum.Domain;
using Quantum.EventSourcing;

namespace Quantum.Command.EventSourcing;

public abstract class IWantToHandleThisCommandEventSourced<TCommand>(IEventStore eventStore)
    : IWantToHandleThisCommand<TCommand>
    where TCommand : IAmACommand
{
    private IsAnAggregateRoot entity;

    protected ApplicationServiceRulesChecker<IWantToHandleThisCommandEventSourced<TCommand>>
        If => new(this);

    public IWantToHandleThisCommandEventSourced<TCommand> Create<T>(IsAnIdentity id, Func<T> func) where T : IsAnAggregateRoot
    {
        entity = func.Invoke();

        EventStreamId = id;
        EnQueue(EventStreamId, entity.GetQueuedDomainEvents());
        return this;
    }

    public async Task<IWantToHandleThisCommandEventSourced<TCommand>> Reconstitute<T>(EventStreamId id
    , EventStreamPosition position = null)
        where T : IsAnAggregateRoot
    {
        EventStreamId = id;

        position ??= EventStreamPosition.AtStart();

        var stream = await eventStore.LoadEventStreamAsync(id, position);

        entity = (T)Activator.CreateInstance(typeof(T), new object[]
        {
            stream.Payloads.ToArray()
        });

        return this;
    }

    public IWantToHandleThisCommandEventSourced<TCommand> Mutate<T>(Action<T> action) where T : IsAnAggregateRoot
    {
        action.Invoke((T)entity);

        EnQueue(EventStreamId, entity.GetQueuedDomainEvents());

        return this;
    }
}
