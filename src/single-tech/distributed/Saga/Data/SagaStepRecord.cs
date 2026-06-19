namespace SagaDemo.Data;

public class SagaStepRecord
{
    public int Id { get; set; }
    public Guid SagaId { get; set; }
    public required string Name { get; set; }
    public string Status { get; set; } = "Running";
    public string? Error { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}
