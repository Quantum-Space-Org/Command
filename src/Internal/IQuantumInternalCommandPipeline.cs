using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quantum.Core;
using Quantum.Domain;
using Quantum.Domain.Messages.Event;

namespace Quantum.Dispatcher.Pipeline
{
    public interface IQuantumInternalCommandPipeline
    {
        void AddStage(IAmAPipelineStage stage);
        Task<ValidationResult> Process<T>(T command) where T : IAmACommand;
        void IWantToListenTo<T>(Action<IsADomainEvent, long, string> action) where T : IsADomainEvent;
        Queue<IAmAPipelineStage> Stages();
        bool HasAnyStages();
    }
}