namespace Catalog.Domain;

// Internal — other modules cannot reference this type or the table it maps to.
internal class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
