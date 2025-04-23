namespace Quantum.Dispatcher.Command;

public interface IAmApplicationServiceRuleChecker
{
    Task<Result> Check();
}