using System.Threading.Tasks;

namespace Quantum.Command.Pipeline.Stages
{
    public class CsrfGuardStage : IAmAPipelineStage
    {
        public override async Task Process<T>(T command, StageContext context)
        {
            //throw new NotImplementedException();
        }
    }
}