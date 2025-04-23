namespace Quantum.Dispatcher;

public interface IAmInterestedIn<ThisEvent>
    where ThisEvent : IsADomainEvent
{
    Task Subscribe(ThisEvent @event);
}