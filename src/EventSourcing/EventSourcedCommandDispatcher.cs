using Quantum.Command.Interceptors;
using Quantum.Command.Sink;
using Quantum.Resolver;

namespace Quantum.Command.EventSourcing;

public class EventSourcedCommandDispatcher(IResolver resolver, IDomainEventSink domainEventSink, ITransactionScopeInterceptor transactionScopeInterceptor) 
    : CommandDispatcher(resolver, domainEventSink, transactionScopeInterceptor);