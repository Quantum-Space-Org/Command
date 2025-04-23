using Microsoft.Extensions.Logging;
using Quantum.DataBase.EntityFramework;

namespace Quantum.Command.Command.Interceptors;

public class EntityFrameworkTransactionInterceptor(
    QuantumDbContext dbContext,
    ILogger<EntityFrameworkTransactionInterceptor> logger)
    : ITransactionScopeInterceptor
{
    public async Task Start()
    {
        logger.LogInformation("Starting database transaction");
        await dbContext.Database.BeginTransactionAsync();
    }

    public async Task Commit()
    {
        try
        {
            if (dbContext.Database.CurrentTransaction != null)
            {
                logger.LogInformation("Saving changes before committing transaction");
                await dbContext.SaveChangesAsync();

                logger.LogInformation("Committing database transaction");
                await dbContext.Database.CommitTransactionAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while committing transaction");
            throw;
        }
    }

    public async Task RollBack()
    {
        try
        {
            if (dbContext.Database.CurrentTransaction != null)
            {
                logger.LogInformation("Rolling back database transaction");
                await dbContext.Database.RollbackTransactionAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while rolling back transaction");
            throw;
        }
        finally
        {
        }
    }
}