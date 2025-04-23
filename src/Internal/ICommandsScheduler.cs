using System.Threading.Tasks;
using Quantum.Core;
using Quantum.Domain.Messages.Command;

namespace Quantum.Dispatcher.Command;

public interface ICommandsScheduler
{
    Task EnqueueAsync(IsACommand command);
}

public class NullCommandsScheduler : ICommandsScheduler
{
    public static ICommandsScheduler Instance => new NullCommandsScheduler();

    public Task EnqueueAsync(IsACommand command)
    {
        return Task.CompletedTask;
    }
}

public class CommandsScheduler : ICommandsScheduler
{
    private readonly CommandsSchedulerDbContext _dbContext;

    public CommandsScheduler(CommandsSchedulerDbContext dbContext)
        => _dbContext = dbContext;

    public async Task EnqueueAsync(IsACommand command)
    {
        var internalCommand = new InternalCommand(command.GetId(), command.Metadata.Type, command.Serialize());
        await _dbContext.InternalCommands.AddAsync(internalCommand);
        await _dbContext.SaveChangesAsync();
    }
}