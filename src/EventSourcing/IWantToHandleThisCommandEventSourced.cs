using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quantum.Command.Handlers;
using Quantum.Domain;
using Quantum.Domain.Messages.Event;
using Quantum.EventSourcing;

namespace Quantum.Command.EventSourcing;

public abstract class IWantToHandleThisCommandEventSourced<TCommand>
: IWantToHandleThisCommand<TCommand>
    where TCommand : IAmACommand
{
    private IEventStore _eventStore;

    private List<IsADomainEvent> Events { get; } = new();

    public IsAnIdentity EventStreamId { get; private set; }
    private object entity;

    protected ApplicationServiceRulesChecker<IWantToHandleThisCommandEventSourced<TCommand>> If => new(this);

    public IWantToHandleThisCommandEventSourced<TCommand> Create<T>(Func<T> func)
    {
        var entityValue = func.Invoke();

        entity = entityValue;

        return this;
    }

    public async Task<IWantToHandleThisCommandEventSourced<TCommand>> Reconstitute<T>(EventStreamId id)
        where T : class
    {
        var stream = await _eventStore.LoadEventStreamAsync(id, EventStreamPosition.AtStart());

        entity = (T)Activator.CreateInstance(typeof(T), new object[]
        {
            stream.Payloads.ToArray()
        });

        return this;
    }

    public IWantToHandleThisCommandEventSourced<TCommand> Mutate<T>(Action<T> action)
    {
        action.Invoke((T)entity);
        return this;
    }
}
