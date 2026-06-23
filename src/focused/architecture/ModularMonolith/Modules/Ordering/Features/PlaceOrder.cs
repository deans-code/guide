using Catalog.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Ordering.Data;
using Ordering.Domain;

namespace Ordering.Features;

internal static class PlaceOrder
{
    public record LineRequest(Guid ProductId, int Quantity);

    public record Request(List<LineRequest> Lines);

    public record LineResponse(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);

    public record Response(Guid Id, DateTimeOffset CreatedAt, decimal Total, List<LineResponse> Lines);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/orders", async (Request request, OrderingDbContext db, IProductCatalog catalog) =>
        {
            if (request.Lines.Count == 0)
                return Results.BadRequest("An order must contain at least one line.");

            var order = new Order { Id = Guid.NewGuid(), CreatedAt = DateTimeOffset.UtcNow };

            // The Ordering module never opens CatalogDbContext or queries the
            // Products table — it only goes through IProductCatalog, the
            // contract the Catalog module chooses to expose.
            foreach (var line in request.Lines)
            {
                var product = await catalog.GetProductAsync(line.ProductId);
                if (product is null)
                    return Results.BadRequest($"Product {line.ProductId} not found.");

                if (!await catalog.TryReserveStockAsync(line.ProductId, line.Quantity))
                    return Results.BadRequest($"Insufficient stock for '{product.Name}'.");

                order.Lines.Add(new OrderLine
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = line.Quantity
                });
            }

            order.Total = order.Lines.Sum(l => l.UnitPrice * l.Quantity);

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var response = new Response(
                order.Id,
                order.CreatedAt,
                order.Total,
                order.Lines.Select(l => new LineResponse(l.ProductId, l.ProductName, l.UnitPrice, l.Quantity)).ToList());

            return Results.Created($"/orders/{order.Id}", response);
        })
        .WithTags("Orders")
        .WithSummary("Place an order, reserving stock via the Catalog module");
}
