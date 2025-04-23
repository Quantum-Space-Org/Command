namespace Quantum.Dispatcher.Semaphore;

public class NullSemaphore : ISemaphore
{
    public void Enter()
    {
            
    }

    public void Exit()
    {
            
    }

    public bool IsThereAnyoneStill() => false;

    public static ISemaphore New() => new NullSemaphore();
}