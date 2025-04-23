namespace Quantum.Command;

public interface ICommandValidator<in TCommand>
    where TCommand : IAmACommand
{
    ValidationResult Validate(TCommand command);
}