namespace Quantum.Dispatcher.Command;

public static class CommandEventuator
{
    public static CommandEventuatorBuilder ShouldEventuateTo(this IAmACommand command,
        IsADomainEvent domainEventA)
    {
        return new CommandEventuatorBuilder(domainEventA);
    }
}

public class CommandEventuatorBuilder
{
    private readonly Queue<IsADomainEvent> _events;

    public CommandEventuatorBuilder(IsADomainEvent isADomainEvent)
    {
        _events = new Queue<IsADomainEvent>();
        Enqueue(isADomainEvent);
    }

    public CommandEventuatorBuilder And(IsADomainEvent domainEvent)
    {
        Enqueue(domainEvent);
        return this;
    }

    private void Enqueue(IsADomainEvent domainEventB)
    {
        _events.Enqueue(domainEventB);
    }

    public Queue<IsADomainEvent> ThankYou()
    {
        return _events;
    }
}
