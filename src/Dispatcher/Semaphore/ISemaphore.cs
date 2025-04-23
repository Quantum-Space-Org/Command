namespace Quantum.Command.Semaphore;

public interface ISemaphore
{
    void Enter();
    void Exit();
    bool IsThereAnyoneStill();
}