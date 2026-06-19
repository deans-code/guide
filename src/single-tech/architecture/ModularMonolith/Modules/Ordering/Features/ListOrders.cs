using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Ordering.Data;

namespace Ordering.Features;

internal static class ListOrders
{
    public record Response(Guid Id, DateTimeOffset CreatedAt, decimal Total, int LineCount);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/orders", async (OrderingDbContext db) =>
            await db.Orders
                .Select(o => new Response(o.Id, o.CreatedAt, o.Total, o.Lines.Count))
                .ToListAsync())
        .WithTags("Orders")
        .WithSummary("List orders");
}
