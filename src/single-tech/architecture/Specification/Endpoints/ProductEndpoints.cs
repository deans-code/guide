using Microsoft.EntityFrameworkCore;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/products");

        group.MapGet("/", async (
            AppDbContext db,
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            bool? inStock,
            bool? featured) =>
        {
            ISpecification<Product> spec = new ActiveSpecification();

            if (category is not null)
                spec = spec.And(new CategorySpecification(category));

            if (minPrice.HasValue || maxPrice.HasValue)
                spec = spec.And(new PriceRangeSpecification(minPrice ?? 0, maxPrice ?? decimal.MaxValue));

            if (inStock == true)
                spec = spec.And(new InStockSpecification());

            if (featured == true)
                spec = spec.And(new FeaturedSpecification());

            var products = await db.Products.Where(spec.Criteria).ToListAsync();
            return Results.Ok(products);
        });

        // Featured AND in-stock AND under £50 — a pre-composed spec for a common use case
        group.MapGet("/featured-deals", async (AppDbContext db) =>
        {
            var spec = new FeaturedSpecification()
                .And(new InStockSpecification())
                .And(new PriceRangeSpecification(0, 50));

            var products = await db.Products.Where(spec.Criteria).ToListAsync();
            return Results.Ok(products);
        });

        // In-stock AND NOT featured AND under £30 — clearance items only
        group.MapGet("/clearance", async (AppDbContext db) =>
        {
            var spec = new ActiveSpecification()
                .And(new InStockSpecification())
                .And(new FeaturedSpecification().Not())
                .And(new PriceRangeSpecification(0, 30));

            var products = await db.Products.Where(spec.Criteria).ToListAsync();
            return Results.Ok(products);
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });
    }
}
