using Quantum.Command.Command.Interceptors;
using Quantum.Command.Command.Sink;
using Quantum.Dispatcher.Command;
using Quantum.Resolver;

namespace Quantum.Command.EventSourcing;

public class EventSourcedCommandDispatcher(IResolver resolver, IDomainEventSink domainEventSink, ITransactionScopeInterceptor transactionScopeInterceptor) 
    : CommandDispatcher(resolver, domainEventSink, transactionScopeInterceptor);