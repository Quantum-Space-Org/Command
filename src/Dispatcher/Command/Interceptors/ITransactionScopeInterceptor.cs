namespace Quantum.Command.Command.Interceptors;

public interface ITransactionScopeInterceptor
{
    Task Start();
    Task Commit();
    Task RollBack();
}