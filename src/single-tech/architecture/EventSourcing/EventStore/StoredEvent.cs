namespace EventSourcingDemo.EventStore;

public class StoredEvent
{
    public long Id { get; set; }
    public required string StreamId { get; set; }
    public required string EventType { get; set; }
    public required string Payload { get; set; }
    public int Version { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
