using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Quantum.Dispatcher.Pipeline.Stages
{
    using System.Diagnostics;

    public class LogPipelineStage(ILogger<LogPipelineStage> logger) : IAmAPipelineStage
    {
        private readonly ILogger<LogPipelineStage> _logger = logger;

        public override async Task Process<TContext>(TContext command, StageContext context)
        {
            var stopwatch = new Stopwatch();

            _logger.LogInformation("start processing command ");

            stopwatch.Start();
            await GoToSuccessorStage(command, context);
            stopwatch.Stop();

            _logger.LogInformation($"processing of {@command} toke {stopwatch.ElapsedMilliseconds} Milliseconds", command);
        }
    }
}