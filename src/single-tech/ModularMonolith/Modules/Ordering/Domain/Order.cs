namespace Ordering.Domain;

internal class Order
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public decimal Total { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
}
