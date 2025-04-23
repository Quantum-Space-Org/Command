namespace Quantum.Command.Sink;

public class QueuedDomainEvents
{
    public long Id { get; set; }
    public string StreamId { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Payload { get; set; }
    public DateTime QueuedAt { get; set; }
}
