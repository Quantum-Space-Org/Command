using System.Threading.Tasks;

namespace Quantum.Command.Internal;

public interface IInternalServiceExecutor
{
    Task Execute(InternalCommand internalCommand);
}