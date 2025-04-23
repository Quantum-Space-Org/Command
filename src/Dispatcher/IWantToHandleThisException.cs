namespace Quantum.Command;

public interface IWantToHandleThisException<in TException>
    where TException : Exception
{
    Result Handle(TException exception);
}