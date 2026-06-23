using MediatR;
using CQRSDemo.Common;

namespace CQRSDemo.Read;

// ── Query ─────────────────────────────────────────────────────────────────
// Reads only from ReadDbContext — never touches the write tables.

public record GetOrderQuery(Guid Id) : IRequest<OrderSummary?>;

public class GetOrderHandler(ReadDbContext db) : IRequestHandler<GetOrderQuery, OrderSummary?>
{
    public async Task<OrderSummary?> Handle(GetOrderQuery query, CancellationToken ct) =>
        await db.OrderSummaries.FindAsync([query.Id], ct);
}

public class GetOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/orders/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetOrderQuery(id), ct) is { } order
                ? Results.Ok(order)
                : Results.NotFound())
        .WithTags("Read — Queries")
        .WithSummary("Get an order summary (reads from the denormalised read model only)");
}
