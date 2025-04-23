namespace Quantum.Dispatcher.Pipeline;

[AttributeUsage(AttributeTargets.Method)]
public class RetryAttribute : Attribute
{
    public Type ExceptionType { get; }

    public RetryAttribute(Type exceptionType)
    {
        ExceptionType = exceptionType;
    }
}