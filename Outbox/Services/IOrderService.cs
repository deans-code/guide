using OutboxDemo.Data;

namespace OutboxDemo.Services;

public interface IOrderService
{
    Task<Order> PlaceOrderAsync(string customerName, decimal amount, CancellationToken ct = default);
}
