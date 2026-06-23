namespace Catalog.Contracts;

// The only shape of product data other modules are allowed to see.
public record ProductSummary(Guid Id, string Name, decimal Price, int StockQuantity);
