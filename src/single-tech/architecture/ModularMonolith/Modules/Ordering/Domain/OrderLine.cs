namespace Ordering.Domain;

// A snapshot of the product at the time of ordering — the Ordering module
// stores its own copy rather than joining across module boundaries.
internal class OrderLine
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
