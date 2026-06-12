using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Ordering.Data;

namespace Ordering.Features;

internal static class GetOrder
{
    public record LineResponse(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);

    public record Response(Guid Id, DateTimeOffset CreatedAt, decimal Total, List<LineResponse> Lines);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/orders/{id:guid}", async (Guid id, OrderingDbContext db) =>
        {
            var order = await db.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id);
            if (order is null)
                return Results.NotFound();

            var response = new Response(
                order.Id,
                order.CreatedAt,
                order.Total,
                order.Lines.Select(l => new LineResponse(l.ProductId, l.ProductName, l.UnitPrice, l.Quantity)).ToList());

            return Results.Ok(response);
        })
        .WithTags("Orders")
        .WithSummary("Get an order by id");
}
