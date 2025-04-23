namespace Quantum.Dispatcher.Command;

public abstract class ICommandDispatcher
{
    public abstract Task<ValidationResult> Dispatch<T>(T command)
        where T : IAmACommand;
    public abstract void IWantToListenTo<T>(Action<IsADomainEvent, long, string> action)
        where T : IsADomainEvent;
}