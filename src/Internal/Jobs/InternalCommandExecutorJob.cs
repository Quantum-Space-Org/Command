using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Quantum.Command.Internal.Jobs;

public class InternalCommandExecutorJob : IJob
{
    private readonly IInternalServiceExecutor _internalServiceExecutor;
    private readonly CommandsSchedulerDbContext _dbContext;

    public InternalCommandExecutorJob(IInternalServiceExecutor internalServiceExecutor, CommandsSchedulerDbContext dbContext)
    {
        _internalServiceExecutor = internalServiceExecutor;
        _dbContext = dbContext;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var internalCommands = await _dbContext
            .InternalCommands.AsNoTracking()
            .Where(a => a.Seen == false)
            .OrderBy(a => a.OccurredAt)
            .Take(10)
            .ToListAsync();

        foreach (var message in internalCommands)
        {
            await _internalServiceExecutor.Execute(message);
        }
    }
}