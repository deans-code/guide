using Catalog.Data;
using Catalog.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Features;

internal static class CreateProduct
{
    public record Request(string Name, decimal Price, int StockQuantity);

    public record Response(Guid Id, string Name, decimal Price, int StockQuantity);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/catalog/products", async (Request request, CatalogDbContext db) =>
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            var response = new Response(product.Id, product.Name, product.Price, product.StockQuantity);
            return Results.Created($"/catalog/products/{product.Id}", response);
        })
        .WithTags("Catalog")
        .WithSummary("Create a product");
}
