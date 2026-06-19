using ErrorOr;
using Microsoft.EntityFrameworkCore;

public class OrderService(AppDbContext db)
{
    public async Task<ErrorOr<Order>> PlaceOrderAsync(int productId, int quantity)
    {
        if (quantity <= 0)
            return ProductErrors.InvalidQuantity;

        var product = await db.Products.FindAsync(productId);
        if (product is null)
            return ProductErrors.NotFound(productId);

        if (product.Stock < quantity)
            return ProductErrors.InsufficientStock(product.Name, product.Stock);

        var order = new Order
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = quantity,
            UnitPrice = product.Price,
            Status = OrderStatus.Pending,
            PlacedAt = DateTime.UtcNow
        };

        product.Stock -= quantity;
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return order;
    }

    public async Task<ErrorOr<Order>> CancelOrderAsync(int orderId)
    {
        var order = await db.Orders.FindAsync(orderId);
        if (order is null)
            return OrderErrors.NotFound(orderId);

        if (order.Status == OrderStatus.Cancelled)
            return OrderErrors.AlreadyCancelled(orderId);

        if (order.Status == OrderStatus.Shipped)
            return OrderErrors.AlreadyShipped(orderId);

        var product = await db.Products.FindAsync(order.ProductId);
        if (product is not null)
            product.Stock += order.Quantity;

        order.Status = OrderStatus.Cancelled;
        await db.SaveChangesAsync();

        return order;
    }

    public async Task<ErrorOr<Order>> GetOrderAsync(int orderId)
    {
        var order = await db.Orders.FindAsync(orderId);
        return order is null ? OrderErrors.NotFound(orderId) : order;
    }

    public Task<List<Order>> GetAllOrdersAsync() =>
        db.Orders.OrderByDescending(o => o.PlacedAt).ToListAsync();
}
