using System.Transactions;
using Microsoft.Extensions.Logging;

namespace Quantum.Command.Interceptors;

public class TransactionManagerInterceptor : ITransactionScopeInterceptor
{
    private TransactionScope _transactionScope;
    private readonly ILogger<TransactionManagerInterceptor> _logger;

    public TransactionManagerInterceptor(ILogger<TransactionManagerInterceptor> logger)
    {
        _logger = logger;
    }

    public async Task Start()
    {
        _logger.LogInformation("Starting TransactionScope");
        _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await Task.CompletedTask;
    }

    public async Task Commit()
    {
        try
        {
            if (_transactionScope != null)
            {
                _transactionScope.Complete();
                _logger.LogInformation("Committing TransactionScope");
            }
        }
        catch (TransactionAbortedException ex)
        {
            _logger.LogError(ex, "TransactionAbortedException in Commit");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in Commit");
            throw;
        }
        finally
        {
            _transactionScope?.Dispose();
            _transactionScope = null;
        }
        await Task.CompletedTask;
    }

    public async Task RollBack()
    {
        try
        {
            if (_transactionScope != null)
            {
                _logger.LogInformation("Rolling back TransactionScope");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in RollBack");
            throw;
        }
        finally
        {
            _transactionScope?.Dispose();
            _transactionScope = null;
        }
        await Task.CompletedTask;
    }
}