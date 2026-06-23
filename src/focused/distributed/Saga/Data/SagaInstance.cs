namespace SagaDemo.Data;

public class SagaInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string OrderId { get; set; }
    public required string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Running";
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public List<SagaStepRecord> Steps { get; set; } = [];
}
