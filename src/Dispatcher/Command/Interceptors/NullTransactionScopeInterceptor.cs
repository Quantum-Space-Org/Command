namespace Quantum.Command.Command.Interceptors;

public class NullTransactionScopeInterceptor : ITransactionScopeInterceptor
{
    public Task Start() => Task.CompletedTask;

    public Task Commit() => Task.CompletedTask;

    public Task RollBack() => Task.CompletedTask;
}