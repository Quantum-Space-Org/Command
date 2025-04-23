namespace Quantum.Command.Handlers;

using System.Linq;

public abstract class IWantToHandleThisCommand<TCommand>
    where TCommand : IAmACommand
{
    private List<IsADomainEvent> QueuedDomainEvents { get; } = [];

    public IsAnIdentity EventStreamId { get; private set; }

    public abstract Task Handle(TCommand command);

    public virtual Task Compensate(TCommand command)
        => Task.CompletedTask;

    public void EnQueue(IsAnIdentity eventStreamId, List<IsADomainEvent> domainEvents)
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

public class ApplicationServiceValidationException(ValidationResult validationResult)
    : DomainValidationException(validationResult);

public class BusinessRulesIsEmptyException : Exception;

public class FuncRuleIsNullException : Exception;