namespace IdempotencyDemo.Data;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string CustomerId { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public string Status { get; set; } = "Succeeded";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
