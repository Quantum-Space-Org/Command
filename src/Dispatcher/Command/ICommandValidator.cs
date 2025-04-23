namespace Quantum.Dispatcher.Command;

public interface ICommandValidator<in TCommand>
    where TCommand : IAmACommand
{
    ValidationResult Validate(TCommand command);
}