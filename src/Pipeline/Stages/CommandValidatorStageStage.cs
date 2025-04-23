using System.Threading.Tasks;
using Quantum.Core;
using Quantum.Core.Exceptions;
using Quantum.Resolver;

namespace Quantum.Command.Pipeline.Stages;

public class CommandValidationException : Exception
{

    public ValidationResult ValidationResult { get; set; }
    public CommandValidationException(ValidationResult validationResult)
    {
        ValidationResult = validationResult;
    }
}

public class CommandValidatorStageStage : IAmAPipelineStage
{
    private readonly IResolver _resolver;

    public CommandValidatorStageStage(IResolver resolver)
        => _resolver = resolver;

    public override async Task Process<TCommand>(TCommand command, StageContext context)
    {
        if (TryResolve<TCommand>(out var result))
        {
            var validationResult = result.Validate(command);
            if (!validationResult.IsSucceeded)
                throw new CommandValidationException(validationResult);
        }
    }

    private bool TryResolve<TCommand>(out ICommandValidator<TCommand> result)
        where TCommand : IAmACommand
    {
        result = null;
        try
        {
            result = _resolver.Resolve<ICommandValidator<TCommand>>();
            return true;
        }
        catch (QuantumComponentIsNotRegisteredException ex)
        {
            //LogException(ex);

        }
        return false;

    }
}