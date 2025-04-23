using System.Threading.Tasks;

namespace Quantum.Dispatcher.Pipeline
{
    public abstract class IAmAPipelineStage
    {
        public IAmAPipelineStage Predecessor { get; set; }
        public IAmAPipelineStage Successor { get; set; }

        protected async Task GoToSuccessorStage<T>(T command, StageContext context)
            where T : IAmACommand
        {
            if (Successor != null)
                await Successor.Process(command, context);
        }
        public abstract Task Process<T>(T command, StageContext context)
            where T : IAmACommand;
    }
}