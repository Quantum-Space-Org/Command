namespace Quantum.Command;

public interface IAmApplicationServiceRuleChecker
{
    Task<Result> Check();
}