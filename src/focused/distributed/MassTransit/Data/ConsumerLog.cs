namespace MassTransitDemo.Data;

public class ConsumerLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid OrderId { get; set; }
    public required string ConsumerName { get; set; }
    public required int Attempt { get; set; }
    public required bool Succeeded { get; set; }
    public required string Detail { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
