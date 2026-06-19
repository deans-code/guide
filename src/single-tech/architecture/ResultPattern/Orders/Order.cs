public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => UnitPrice * Quantity;
    public OrderStatus Status { get; set; }
    public DateTime PlacedAt { get; set; }
}

public enum OrderStatus { Pending, Cancelled, Shipped }
