namespace Catalog.Contracts;

// The public surface of the Catalog module. Other modules depend on this
// interface only — Catalog.Data and Catalog.Domain are internal and cannot
// be referenced from outside this assembly.
public interface IProductCatalog
{
    Task<ProductSummary?> GetProductAsync(Guid productId, CancellationToken ct = default);

    Task<bool> TryReserveStockAsync(Guid productId, int quantity, CancellationToken ct = default);
}
