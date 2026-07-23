namespace MassTransitDemo.Messages;

// The message contract published when an order is placed. Both consumers
// below subscribe to this same event independently — MassTransit fans it
// out to each of them without the publisher knowing who is listening.
public record OrderPlaced(Guid OrderId, string CustomerName, decimal Amount, DateTime CreatedAt);
