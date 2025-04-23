namespace Quantum.Command.Interceptors;

public interface ITransactionScopeInterceptor
{
    Task Start();
    Task Commit();
    Task RollBack();
}