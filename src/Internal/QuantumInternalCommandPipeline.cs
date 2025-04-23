using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quantum.Core;
using Quantum.Dispatcher.Pipeline.Stages;
using Quantum.Domain;
using Quantum.Domain.Messages.Event;

namespace Quantum.Dispatcher.Pipeline
{
    public class QuantumInternalCommandPipeline : IQuantumInternalCommandPipeline
    {
        private readonly Queue<IAmAPipelineStage> _stages;
        private readonly StageContext _context = new();

        public QuantumInternalCommandPipeline()
            => _stages = new Queue<IAmAPipelineStage>();

        public void AddStage(IAmAPipelineStage stage)
            => _stages.Enqueue(stage);

        public async Task<ValidationResult> Process<T>(T command)
            where T : IAmACommand
        {
            var starterStage = _stages.First();

            if (starterStage == null)
            {
                throw new StarterStageIsEmptyOrNullException("starter stage is empty");
            }

            try
            {
                while (StagesIsNotEmpty())
                {
                    var stage = _stages.Dequeue();
                    await stage.Process(command, _context);
                }
            }
            catch (DomainValidationException ex)
            {
                return ex.ValidationResult;
            }
            catch (CommandValidationException ex)
            {
                return ex.ValidationResult;
            }

            return ValidationResult.Success();
        }

        private bool StagesIsNotEmpty()
            => This.Is.True(_stages.Any());

        public void IWantToListenTo<T>(Action<IsADomainEvent, long, string> action)
            where T : IsADomainEvent
        {
            _context.AddEventListeners<T>(action);
        }

        public Queue<IAmAPipelineStage> Stages()
        {
            return _stages;
        }

        public bool HasAnyStages() 
            => Stages() != null && Stages().Any();

        public class StarterStageIsEmptyOrNullException : Exception
        {
            public StarterStageIsEmptyOrNullException(string starterStageIsEmpty)
                : base(starterStageIsEmpty)
            {
            }
        }
    }
}