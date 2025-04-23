using System.Collections.Generic;
using System.Threading.Tasks;
using Quantum.Core.Exceptions;
using Quantum.Domain;
using Quantum.Domain.Messages.Event;
using Quantum.Resolver;

namespace Quantum.Command.Pipeline.Stages
{
    public class PublishDomainEventsStage : IAmAPipelineStage
    {
        private readonly IResolver _resolver;

        public PublishDomainEventsStage(IResolver resolver)
        {
            _resolver = resolver;
        }

        public override async Task Process<TContext>(TContext command, StageContext context)
        {
            var domainEvents = context.GetDomainEvents();

            Dictionary<IsAnIdentity, List<IsADomainEvent>> dic = new Dictionary<IsAnIdentity, List<IsADomainEvent>>();

            foreach (var (key, value) in domainEvents)
            {
                foreach (var isADomainEvent in value.Events)
                {
                    var result = await Selector(isADomainEvent);
                    if (result.Item1 == null || result.Item2 == null)
                        continue;

                    dic.Add(result.Item1, result.Item2);
                }
            }

            foreach (var (anIdentity, domainEventList) in dic)
            {
                context.QueueDomainEvents(anIdentity, domainEventList,1);
            }
        }

        private async Task<(IsAnIdentity, List<IsADomainEvent>)> Selector<T>(T isADomainEvent)
        where T : IsADomainEvent
        {
            var domainEvenNotificationType = typeof(IWantToSubscribeTo<>);
            var domainNotificationWithGenericType = domainEvenNotificationType.MakeGenericType(isADomainEvent.GetType());

            try
            {
                var domainNotificationWithGenericTypeObject = _resolver.Resolve(domainNotificationWithGenericType);

                var methodInfo = domainNotificationWithGenericTypeObject.GetType().GetMethod("Handle");

                methodInfo?.Invoke(domainNotificationWithGenericTypeObject, new object[] { isADomainEvent });

                var hasAnyQueuedEventsMethodInfo = domainNotificationWithGenericTypeObject.GetType().GetMethod("HasAnyQueuedEvents");
                var hasAnyQueuedEvents = hasAnyQueuedEventsMethodInfo?.Invoke(domainNotificationWithGenericTypeObject, new object[] { });
                if (!(bool)hasAnyQueuedEvents)
                    return (null, null);

                var getQueuedEventsMethodInfo = domainNotificationWithGenericTypeObject.GetType().GetMethod("GetQueuedEvents");

                var events = getQueuedEventsMethodInfo?.Invoke(domainNotificationWithGenericTypeObject, new object[] { });

                var domainEvents = (List<IsADomainEvent>)events;

                var getEventStreamIdMethodInfo = domainNotificationWithGenericTypeObject.GetType().GetMethod("GetEventStreamId");
                var eventStreamId = getEventStreamIdMethodInfo?.Invoke(domainNotificationWithGenericTypeObject, new object[] { });

                return ((IsAnIdentity)eventStreamId, domainEvents);
            }
            catch (QuantumComponentIsNotRegisteredException e)
            {

            }

            return (null, null);
        }
    }
}