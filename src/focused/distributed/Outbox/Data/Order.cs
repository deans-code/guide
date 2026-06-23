namespace OutboxDemo.Data;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Placed";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
