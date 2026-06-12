using Catalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Features;

internal static class ListProducts
{
    public record Response(Guid Id, string Name, decimal Price, int StockQuantity);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/catalog/products", async (CatalogDbContext db) =>
            await db.Products
                .Select(p => new Response(p.Id, p.Name, p.Price, p.StockQuantity))
                .ToListAsync())
        .WithTags("Catalog")
        .WithSummary("List products");
}
