using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quantum.Core;

namespace Quantum.Command.Internal;

public class InternalServiceExecutor : IInternalServiceExecutor
{
    private readonly CommandsSchedulerDbContext _dbContext;
    private readonly IQuantumInternalCommandPipeline _pipeline;
    private readonly ILogger<InternalServiceExecutor> _logger;

    public InternalServiceExecutor(CommandsSchedulerDbContext dbContext, IQuantumInternalCommandPipeline pipeline, ILogger<InternalServiceExecutor> logger)
    {
        _dbContext = dbContext;
        _pipeline = pipeline;
        _logger = logger;
    }

    public async Task Execute(InternalCommand internalCommand)
    {
        if (internalCommand.Seen)
        {
            //log
            return;
        }

        if (Type.GetType(internalCommand.Type) == null)
        {
            //log
            return;
        }

        var validationResult = await _pipeline.Process(ToCommand(internalCommand));

        if (validationResult.IsSucceeded)
        {
            internalCommand.SetSeen();
            internalCommand.SetSeenAt();

            _dbContext.InternalCommands.Update(internalCommand);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            //log
            return;
        }
    }

    private static IAmACommand ToCommand(InternalCommand internalCommand)
    {
        var type = Type.GetType(internalCommand.Type);

        return internalCommand.Command.Deserialize(type) as IAmACommand;
    }
}