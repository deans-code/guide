using System.Text.Json;
using OutboxDemo.Data;

namespace OutboxDemo.Services;

public class OrderService(AppDbContext db) : IOrderService
{
    public async Task<Order> PlaceOrderAsync(string customerName, decimal amount, CancellationToken ct = default)
    {
        var order = new Order { CustomerName = customerName, Amount = amount };

        var outboxMessage = new OutboxMessage
        {
            Type = "order.placed",
            Payload = JsonSerializer.Serialize(new
            {
                OrderId    = order.Id,
                order.CustomerName,
                order.Amount,
                order.CreatedAt
            })
        };

        // ── Atomic write: order row + outbox message in one transaction ────────
        //
        // This is the core of the outbox pattern. Both records commit together,
        // so there is no window where the order exists but the event is missing,
        // or vice versa. The OutboxProcessor publishes to the broker separately,
        // decoupling messaging reliability from the HTTP request lifecycle.
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        db.Orders.Add(order);
        db.OutboxMessages.Add(outboxMessage);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return order;
    }
}
