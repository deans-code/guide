using MediatR;
using VSADemo.Common;

namespace VSADemo.Features.Products;

// ── Request / Response ────────────────────────────────────────────────────

public record GetProductQuery(Guid Id) : IRequest<GetProductResponse?>;

public record GetProductResponse(Guid Id, string Name, string Category, decimal Price, int Stock, DateTime CreatedAt);

// ── Handler ───────────────────────────────────────────────────────────────

public class GetProductHandler(AppDbContext db)
    : IRequestHandler<GetProductQuery, GetProductResponse?>
{
    public async Task<GetProductResponse?> Handle(GetProductQuery query, CancellationToken ct)
    {
        var p = await db.Products.FindAsync([query.Id], ct);
        return p is null ? null
            : new GetProductResponse(p.Id, p.Name, p.Category, p.Price, p.Stock, p.CreatedAt);
    }
}

// ── Endpoint ──────────────────────────────────────────────────────────────

public class GetProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/products/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new GetProductQuery(id), ct) is { } product
                ? Results.Ok(product)
                : Results.NotFound())
        .WithTags("Products")
        .WithSummary("Get a product by ID");
}
