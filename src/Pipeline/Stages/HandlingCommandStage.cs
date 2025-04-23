using System.Linq;
using System.Threading.Tasks;
using Quantum.Command.Handlers;
using Quantum.Core.Exceptions;
using Quantum.EventSourcing;
using Quantum.Resolver;

namespace Quantum.Command.Pipeline.Stages
{
    public class HandlingCommandStage : IAmAPipelineStage
    {
        private readonly IResolver _resolver;
        private readonly IEventStore _eventStore;
        public HandlingCommandStage(IResolver resolver, IEventStore eventStore)
        {
            _resolver = resolver;
            _eventStore = eventStore;
        }

        public override async Task Process<T>(T command, StageContext context)
        {
            IWantToHandleThisCommand<T> handler;

            try
            {
                handler = _resolver.Resolve<IWantToHandleThisCommand<T>>();
            }
            catch (QuantumComponentIsNotRegisteredException e)
            {
                throw new CommandHandlerIsNotRegisteredException(command.GetType());
            }


            try
            {
                await handler.Handle(command);
            }
            catch (Exception e)
            {
                var method = handler.GetType().GetMethods().FirstOrDefault(a => a.Name == "Handle");
                var firstOrDefault = method.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(RetryAttribute));

                if (firstOrDefault != null)
                {
                    if (e.GetType() == (firstOrDefault as RetryAttribute).ExceptionType)
                        await handler.Handle(command);
                }
                else
                {
                    throw e;
                }
            }

            var eventStreamId = handler.EventStreamId;
            var queuedEvents = handler.DeQueueDomainEvents();

            if (eventStreamId != null && queuedEvents != null && queuedEvents.Any())
            {

                long expectedVersion = await _eventStore.GetCurrentVersionOfStream(eventStreamId);

                context.QueueDomainEvents(eventStreamId, queuedEvents, expectedVersion);
            }
        }


        private IWantToHandleThisCommand<TCommand> GetHandler<TCommand>(TCommand command)
            where TCommand : IAmACommand
        {
            try
            {
                return _resolver.Resolve<IWantToHandleThisCommand<TCommand>>();
            }
            catch (QuantumComponentIsNotRegisteredException e)
            {
                throw new CommandHandlerIsNotRegisteredException(typeof(TCommand));
            }
        }
    }
}