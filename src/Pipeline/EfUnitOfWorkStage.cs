using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quantum.DataBase.EntityFramework;

namespace Quantum.Command.Pipeline;

public class EfUnitOfWorkStage : IAmAPipelineStage
{
    private readonly ILogger<EfUnitOfWorkStage> _logger;
    private readonly QuantumDbContext _context;

    public EfUnitOfWorkStage(QuantumDbContext context, ILogger<EfUnitOfWorkStage> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task Process<T>(T command, StageContext context)
    {
        _context.SaveChangesFailed += _context_SaveChangesFailed;
        await _context.SaveChangesAsync();
    }

    private void _context_SaveChangesFailed(object sender, SaveChangesFailedEventArgs e)
    {
        _logger.LogError($"An error occurred in closing UoW. error {e}");
    }
}