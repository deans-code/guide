using MediatR;
using Microsoft.EntityFrameworkCore;
using CQRSDemo.Common;

namespace CQRSDemo.Read;

// ── Query ─────────────────────────────────────────────────────────────────

public record ListOrdersQuery(string? Status) : IRequest<List<OrderSummary>>;

public class ListOrdersHandler(ReadDbContext db) : IRequestHandler<ListOrdersQuery, List<OrderSummary>>
{
    public async Task<List<OrderSummary>> Handle(ListOrdersQuery query, CancellationToken ct)
    {
        var q = db.OrderSummaries.AsQueryable();

        if (query.Status is not null)
            q = q.Where(o => o.Status == query.Status);

        return await q.OrderByDescending(o => o.PlacedAt).ToListAsync(ct);
    }
}

public class ListOrdersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/orders", async (
            IMediator mediator,
            CancellationToken ct,
            string? status = null) =>
            Results.Ok(await mediator.Send(new ListOrdersQuery(status), ct)))
        .WithTags("Read — Queries")
        .WithSummary("List order summaries — optionally filter by status (Pending, Confirmed, Cancelled)");
}
