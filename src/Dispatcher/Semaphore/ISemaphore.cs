namespace Quantum.Dispatcher.Semaphore;

public interface ISemaphore
{
    void Enter();
    void Exit();
    bool IsThereAnyoneStill();
}