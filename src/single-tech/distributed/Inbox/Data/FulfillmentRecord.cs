namespace InboxDemo.Data;

public class FulfillmentRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public required string CustomerName { get; set; }
    public decimal Amount { get; set; }
    public DateTime FulfilledAt { get; set; } = DateTime.UtcNow;
    public Guid InboxMessageId { get; set; }
}
