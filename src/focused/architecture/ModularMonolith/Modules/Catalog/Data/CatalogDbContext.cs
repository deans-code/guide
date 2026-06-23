using Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Data;

// Internal — owned exclusively by the Catalog module. The Ordering module
// cannot open this DbContext or query the Products table directly.
internal class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}
