using Quantum.Core;

namespace Quantum.Command.Pipeline;

public class HeyPipeline
{
    public static HeyPipeline IWant() => new();
    public HeyPipeline ToDefineAPipeline()
    {
        return new HeyPipeline();
    }
    public PipelineBuilder WithStarterStage(IAmAPipelineStage stage)
    {
        return new PipelineBuilder(stage);
    }

    public class PipelineBuilder
    {
        private readonly IQuantumPipeline _quantumPipeline;
        private IAmAPipelineStage _lastStage;
        public PipelineBuilder(IAmAPipelineStage starterLastStage)
        {
            _quantumPipeline = new QuantumPipeline();

            _lastStage = starterLastStage ?? throw new ArgumentNullException("starterLastStage");
            _lastStage.Predecessor = null;
        }

        public PipelineBuilder WithSuccessor(IAmAPipelineStage newStage)
        {
            _lastStage.Successor = newStage;
            _quantumPipeline.AddStage(_lastStage);
            newStage.Predecessor = _lastStage;
            _lastStage = newStage;

            return this;
        }

        public IQuantumPipeline ThankYou()
        {
            _lastStage.Successor = null;
            _quantumPipeline.AddStage(_lastStage);

            GuardAgainstEmptyStages(_quantumPipeline);
            return _quantumPipeline;
        }

           
        private void GuardAgainstEmptyStages(IQuantumPipeline quantumPipeline)
        {
            if (This.Is.False(quantumPipeline.HasAnyStages()))
                throw new QuantumPipelineEmptyStagesException();

        }

    }
}