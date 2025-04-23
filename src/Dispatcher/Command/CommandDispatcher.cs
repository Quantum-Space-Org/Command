using Quantum.Core.Exceptions;

namespace Quantum.Dispatcher.Command;

using System.Linq;
using Quantum.Command.Command.Handlers;
using Quantum.Command.Command.Interceptors;
using Quantum.Command.Command.Sink;
using Resolver;

public class CommandDispatcher(IResolver resolver,
    IDomainEventSink domainEventSink, ITransactionScopeInterceptor transactionScopeInterceptor)
    : ICommandDispatcher
{
    private Dictionary<Type, List<Action<IsADomainEvent, long, string>>> _actions = new();

    private Dictionary<IsAnIdentity, List<IsADomainEvent>>
        _queuedDomainEvents = new();

    public override async Task<ValidationResult> Dispatch<T>(T command)
    {
        await 
        transactionScopeInterceptor.Start();
        try
        {
            var validator = resolver.Resolve<ICommandValidator<T>>();
            var validationResult = validator.Validate(command);
            if (validationResult.IsSucceeded is false)
                return validationResult;
        }
        catch (QuantumComponentIsNotRegisteredException ex)
        {
            // there is no validator registered for T
            LogException(ex);
        }

        IWantToHandleThisCommand<T> handler;

        try
        {
            handler = resolver.Resolve<IWantToHandleThisCommand<T>>();
        }
        catch (QuantumComponentIsNotRegisteredException e)
        {
            LogException(e);

            throw new CommandHandlerIsNotRegisteredException(command.GetType());
        }

        try
        {
            await handler.Handle(command);
        }
        catch (DomainValidationException exception)
        {
            return exception.ValidationResult;
        }

        _queuedDomainEvents =
            new Dictionary<IsAnIdentity, List<IsADomainEvent>>
            {
                {handler.EventStreamId, handler.DeQueueDomainEvents()}
            };

        CallListeners();

        await PublishDomainEvent();

        await
            transactionScopeInterceptor.Commit();
        return ValidationResult.Success();
    }

    private async Task PublishDomainEvent()
    {
        if (_queuedDomainEvents.Any())
        {
            foreach (var item in _queuedDomainEvents)
            {

                await domainEventSink.Queue(item.Key, item.Value);
            }
        }
    }

    private void CallListeners()
    {
        if (_actions.Count == 0)
            return;

        foreach (var x in _queuedDomainEvents)
        {
            const int version = 1;

            foreach (var item in x.Value)
            {
                var type = item.GetType();

                if (!_actions.TryGetValue(type, out var listeners))
                    continue;

                foreach (var listener in listeners)
                {
                    listener.Invoke(item, version, x.Key.ToString());
                }
            }
        }
    }

    public override void IWantToListenTo<T>(Action<IsADomainEvent, long, string> action)
    {
        _actions ??= new();

        if (_actions.TryGetValue(typeof(T), out var key))
            key.Add(action);

        _actions[typeof(T)] = [action];
    }

    private void LogException(Exception exception, string message = "")
    {
    }
}