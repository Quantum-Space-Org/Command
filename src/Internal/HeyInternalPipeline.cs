using System;
using Quantum.Core;

namespace Quantum.Dispatcher.Pipeline;

public class HeyInternalPipeline
{
    public static HeyInternalPipeline IWant() => new();
    public HeyInternalPipeline ToDefineAPipeline()
    {
        return new HeyInternalPipeline();
    }
    public PipelineBuilder WithStarterStage(IAmAPipelineStage stage)
    {
        return new PipelineBuilder(stage);
    }

    public class PipelineBuilder
    {
        private readonly IQuantumInternalCommandPipeline _quantumInternalCommandPipeline;
        private IAmAPipelineStage _lastStage;
        public PipelineBuilder(IAmAPipelineStage starterLastStage)
        {

            _quantumInternalCommandPipeline = new QuantumInternalCommandPipeline();

            _lastStage = starterLastStage ?? throw new ArgumentNullException("starterLastStage");
            _lastStage.Predecessor = null;
        }

        public PipelineBuilder WithSuccessor(IAmAPipelineStage newStage)
        {
            _lastStage.Successor = newStage;
            _quantumInternalCommandPipeline.AddStage(_lastStage);
            newStage.Predecessor = _lastStage;
            _lastStage = newStage;

            return this;
        }


           
        public IQuantumInternalCommandPipeline ThankYou()
        {
            _lastStage.Successor = null;
            _quantumInternalCommandPipeline.AddStage(_lastStage);
                
            GuardAgainstEmptyStages(_quantumInternalCommandPipeline);

            return _quantumInternalCommandPipeline;
        }

        private void GuardAgainstEmptyStages(IQuantumInternalCommandPipeline quantumPipeline)
        {
            if (This.Is.False(quantumPipeline.HasAnyStages()))
                throw new QuantumPipelineEmptyStagesException();
        }
    }
}