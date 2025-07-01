using System.Linq;
using Quantum.Command.Handlers;

namespace Quantum.Command;

public class ApplicationServiceRulesChecker<TCommandHandler>
    (TCommandHandler _handler)
{
    private readonly Queue<Func<Task<Result>>> _businessRules = new();

    public ApplicationServiceRulesChecker<TCommandHandler> This(Func<Task<Result>> businessRuleFunc)
    {
        Add(businessRuleFunc);

        return this;
    }

    public ApplicationServiceRulesChecker<TCommandHandler> This(IAmApplicationServiceRuleChecker businessRuleFunc)
    {
        Add(businessRuleFunc.Check);

        return this;
    }

    public ApplicationServiceRulesChecker<TCommandHandler> And(IAmApplicationServiceRuleChecker businessRuleFunc)
    {
        Add(businessRuleFunc.Check);

        return this;
    }

    public ApplicationServiceRulesChecker<TCommandHandler> And(Func<Task<Result>> businessRuleFunc)
    {
        Add(businessRuleFunc);

        return this;
    }

    public ApplicationServiceRulesChecker<TCommandHandler> IsTrue()
    {
        if (_businessRules.Any() == false)
            throw new BusinessRulesIsEmptyException();

        Execute();
        return this;
    }

    public ApplicationServiceRulesChecker<TCommandHandler> AreTrue()
    {
        if (_businessRules.Any() == false)
            throw new BusinessRulesIsEmptyException();

        if (_handler != null)
            return this;

        Execute();
        throw new ArgumentNullException("handler is can not be null!");
    }

    internal virtual async Task Execute()
    {
        if (_businessRules != null)
            foreach (var businessRuleCheckerKeyValue in _businessRules)
            {
                var businessRuleFunction = businessRuleCheckerKeyValue;

                var result = await businessRuleFunction.Invoke();

                if (result.IsSucceeded == false)
                {

                    throw new ApplicationServiceValidationException(ValidationResult.Fail(result));
                }
            }
    }

    private void Add(Func<Task<Result>> businessRuleFunc)
    {
        if (businessRuleFunc == null)
            throw new FuncRuleIsNullException();

        _businessRules.Enqueue(businessRuleFunc);
    }

    public TCommandHandler Then => _handler;
}

public class ApplicationServiceValidationException(ValidationResult validationResult)
    : DomainValidationException(validationResult);

public class BusinessRulesIsEmptyException : Exception;

public class FuncRuleIsNullException : Exception;