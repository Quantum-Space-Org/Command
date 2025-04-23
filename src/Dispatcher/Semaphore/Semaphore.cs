namespace Quantum.Dispatcher.Semaphore;

public class Semaphore : ISemaphore
{
    private int _incrementer = 0;
    public void Enter() => Increment();
    public bool IsThereAnyoneStill() => _incrementer > 0;
    public void Exit() => Decrement();
        
    private void Decrement() => _incrementer--;
    private void Increment() => _incrementer++;
}