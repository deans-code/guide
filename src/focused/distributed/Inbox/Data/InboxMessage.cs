namespace InboxDemo.Data;

public enum InboxStatus { Pending, Processed }

public class InboxMessage
{
    public Guid MessageId { get; set; }   // Broker-assigned ID — primary key enforces deduplication
    public required string Type { get; set; }
    public required string Payload { get; set; }
    public InboxStatus Status { get; set; } = InboxStatus.Pending;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
