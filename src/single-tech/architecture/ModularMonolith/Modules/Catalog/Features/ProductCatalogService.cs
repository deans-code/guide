using Catalog.Contracts;
using Catalog.Data;

namespace Catalog.Features;

// Internal implementation of the module's public contract. Registered
// against IProductCatalog in CatalogModule.AddModule.
internal class ProductCatalogService(CatalogDbContext db) : IProductCatalog
{
    public async Task<ProductSummary?> GetProductAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await db.Products.FindAsync([productId], ct);
        return product is null
            ? null
            : new ProductSummary(product.Id, product.Name, product.Price, product.StockQuantity);
    }

    public async Task<bool> TryReserveStockAsync(Guid productId, int quantity, CancellationToken ct = default)
    {
        var product = await db.Products.FindAsync([productId], ct);
        if (product is null || product.StockQuantity < quantity)
            return false;

        product.StockQuantity -= quantity;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
