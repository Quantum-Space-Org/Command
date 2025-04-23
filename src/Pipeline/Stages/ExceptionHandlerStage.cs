using System.Threading.Tasks;
using Quantum.Core.Exceptions;
using Quantum.Domain;

namespace Quantum.Command.Pipeline.Stages
{
    public class ExceptionHandlerStage : IAmAPipelineStage
    {
        public override async Task Process<TContext>(TContext command, StageContext context)
        {
            try
            {
                //await GoToSuccessorStage(command, context);
            }
            catch (DomainValidationException ex)
            {
                throw;
            }
            catch (CommandHandlerIsNotRegisteredException ex)
            {
                throw;
            }
        }
    }
}