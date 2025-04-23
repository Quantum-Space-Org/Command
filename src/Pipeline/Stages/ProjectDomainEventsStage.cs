using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quantum.Domain;
using Quantum.Domain.Messages.Event;
using Quantum.EventSourcing;
using Quantum.EventSourcing.Projection;
using Quantum.Resolver;
using LogEventIds = Quantum.Core.LogEventIds;

namespace Quantum.Command.Pipeline.Stages
{
    public class ProjectDomainEventsStage : IAmAPipelineStage
    {
        private readonly ILedger _ledger;
        private readonly IResolver _resolver;
        private readonly ILogger<ProjectDomainEventsStage> _logger;
        private readonly IDeDuplicator _deDuplicator;

        public ProjectDomainEventsStage(ILedger ledger,
            IResolver resolver,
            IDeDuplicator deDuplicator,
            ILogger<ProjectDomainEventsStage> logger)
        {
            _ledger = ledger;
            _resolver = resolver;
            _deDuplicator = deDuplicator;
            _logger = logger;
        }

        public override async Task Process<T>(T command, StageContext context)
        {
            var domainEvents = context.GetDomainEvents();
            foreach (var domainEvent in domainEvents)
            {
                await Project(domainEvent);
            }
        }

        private async Task Project(KeyValuePair<IsAnIdentity, QueuedEvents> domainEvent)
        {
            foreach (var domainEventsValue in domainEvent.Value.Events)
            {
                if (await HasItBeenSeenBefore(domainEventsValue, domainEventsValue.GetType().AssemblyQualifiedName, 
                        new EventId(LogEventIds.ProjectingDomainEvent, domainEventsValue.GetType().AssemblyQualifiedName)))
                    continue;

                var projectorTypes = WhoAreInterestedIn(new EventId(LogEventIds.ProjectingDomainEvent, domainEventsValue.GetType().AssemblyQualifiedName), domainEventsValue.GetType().AssemblyQualifiedName);

                foreach (var projectorType in projectorTypes)
                {
                    _logger.LogInformation(new EventId(LogEventIds.ProjectingDomainEvent, domainEventsValue.GetType().AssemblyQualifiedName), $"Start projecting {projectorType.FullName} with event type {domainEventsValue.GetType().AssemblyQualifiedName}");

                    var projector = _resolver.Resolve(projectorType);

                    _logger.LogInformation(new EventId(LogEventIds.ProjectingDomainEvent, domainEventsValue.GetType().AssemblyQualifiedName), $"{projectorType.FullName} was successfully resolved!");

                    try
                    {
                        await ProjectEvent(new EventId(LogEventIds.ProjectingDomainEvent, domainEventsValue.GetType().AssemblyQualifiedName), projector, domainEventsValue);
                        await SaveEventAsSeen(domainEventsValue, domainEventsValue.GetType().AssemblyQualifiedName);
                    }
                    catch (Exception ex)
                    {
                        // how to handle exception
                        _logger.LogError(new EventId(LogEventIds.ProjectingDomainEvent, domainEventsValue.GetType().AssemblyQualifiedName), $"An error occurred in processing event {domainEventsValue.GetType().AssemblyQualifiedName} by {projectorType.FullName}, exception {ex.Message}, inner {ex.InnerException}");
                    }
                }
            }
        }

        private List<Type> WhoAreInterestedIn(EventId eventId, string eventAssemblyQualifiedName)
        {
            var whoAreInterestedIn = _ledger.WhoAreInterestedIn(Type.GetType(eventAssemblyQualifiedName));
            _logger.LogInformation(eventId, $"{whoAreInterestedIn.Count} projectors are exist that interested in {eventAssemblyQualifiedName}");

            return whoAreInterestedIn;
        }

        private async Task SaveEventAsSeen(IsADomainEvent domainEventsValue, string eventAssemblyQualifiedName)
            => await _deDuplicator.Save(domainEventsValue.MessageMetadata.Id, eventAssemblyQualifiedName);

        private async Task ProjectEvent(EventId eventId, object projector, IsADomainEvent domainEventsValue)
        {
            _logger.LogInformation(eventId, $"Start processing ... ");

            await ((ImAProjector)projector).Process(domainEventsValue);

            _logger.LogInformation(eventId, $"end of processing, Successful !");
        }

        private async Task<bool> HasItBeenSeenBefore(IsADomainEvent domainEventsValue, string assemblyQualifiedName, EventId eventId)
        {
            _logger.LogInformation(eventId, $"An event observed. {domainEventsValue.GetType().ToString()} {domainEventsValue.MessageMetadata.Id}");

            if (await _deDuplicator.IsThisADuplicateEventWeHaveSeenBefore(domainEventsValue.MessageMetadata.Id, assemblyQualifiedName))
            {
                _logger.LogInformation(eventId, $"We have seen {assemblyQualifiedName} before");

                return true;
            }

            return false;
        }
    }
}