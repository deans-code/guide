namespace DapperDemo.Data;

public class Order
{
    public int Id { get; set; }
    public required string CustomerName { get; set; }
    public DateTime PlacedAtUtc { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}
