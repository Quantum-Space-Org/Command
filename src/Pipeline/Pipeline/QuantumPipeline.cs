using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quantum.Core;
using Quantum.Dispatcher.Pipeline.Stages;
using Quantum.Domain;
using Quantum.Domain.Messages.Event;

namespace Quantum.Dispatcher.Pipeline
{
    public interface IQuantumPipeline
    {
        void AddStage(IAmAPipelineStage stage);
        Task<ValidationResult> Process<T>(T command) where T : IAmACommand;
        void IWantToListenTo<T>(Action<IsADomainEvent, long, string> action) where T : IsADomainEvent;
        Queue<IAmAPipelineStage> Stages();
        bool HasAnyStages();
    }

    public class QuantumPipeline : IQuantumPipeline
    {
        private readonly Queue<IAmAPipelineStage> _stages;
        private readonly StageContext _context = new();

        public QuantumPipeline()
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
                foreach (var amAPipelineStage in _stages)
                {
                    await amAPipelineStage.Process(command, _context);
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
            finally
            {
                _context.Clear();
            }
            return ValidationResult.Success();
        }
        
        public void IWantToListenTo<T>(Action<IsADomainEvent, long, string> action)
            where T : IsADomainEvent =>
            _context.AddEventListeners<T>(action);

        public Queue<IAmAPipelineStage> Stages() 
            => _stages;

        public bool HasAnyStages() 
            => _stages != null && _stages.Any();

        public class StarterStageIsEmptyOrNullException : Exception
        {
            public StarterStageIsEmptyOrNullException(string starterStageIsEmpty)
                : base(starterStageIsEmpty)
            {
            }
        }
    }
}