namespace OutboxDemo.Data;

public enum OutboxStatus { Pending, Processed, Failed }

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Type { get; set; }
    public required string Payload { get; set; }
    public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public int AttemptCount { get; set; }
    public string? LastError { get; set; }
}
