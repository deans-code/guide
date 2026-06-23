using Catalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Features;

internal static class GetProduct
{
    public record Response(Guid Id, string Name, decimal Price, int StockQuantity);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/catalog/products/{id:guid}", async (Guid id, CatalogDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            return product is null
                ? Results.NotFound()
                : Results.Ok(new Response(product.Id, product.Name, product.Price, product.StockQuantity));
        })
        .WithTags("Catalog")
        .WithSummary("Get a product by id");
}
