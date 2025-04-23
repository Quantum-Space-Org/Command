using System.Threading.Tasks;

namespace Quantum.Dispatcher.Command;

public interface IInternalServiceExecutor
{
    Task Execute(InternalCommand internalCommand);
}