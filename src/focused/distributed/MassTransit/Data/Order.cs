namespace MassTransitDemo.Data;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
