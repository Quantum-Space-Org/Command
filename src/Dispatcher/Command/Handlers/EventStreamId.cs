namespace Quantum.Command.Command.Handlers;

public class EventStreamId(string Id, Type Type) : IsAnIdentity
{
    public static EventStreamId New<T>(string id) => new(id, typeof(T));

    public override string ToString()
        => $"{Type.Name} - {Id}";
}