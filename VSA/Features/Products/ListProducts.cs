using MediatR;
using Microsoft.EntityFrameworkCore;
using VSADemo.Common;

namespace VSADemo.Features.Products;

// ── Request / Response ────────────────────────────────────────────────────

public record ListProductsQuery(string? Category, decimal? MaxPrice) : IRequest<List<ListProductsItem>>;

public record ListProductsItem(Guid Id, string Name, string Category, decimal Price, int Stock);

// ── Handler ───────────────────────────────────────────────────────────────

public class ListProductsHandler(AppDbContext db)
    : IRequestHandler<ListProductsQuery, List<ListProductsItem>>
{
    public async Task<List<ListProductsItem>> Handle(ListProductsQuery query, CancellationToken ct)
    {
        var q = db.Products.AsQueryable();

        if (query.Category is not null)
            q = q.Where(p => p.Category == query.Category);

        if (query.MaxPrice is not null)
            q = q.Where(p => p.Price <= query.MaxPrice);

        return await q
            .OrderBy(p => p.Name)
            .Select(p => new ListProductsItem(p.Id, p.Name, p.Category, p.Price, p.Stock))
            .ToListAsync(ct);
    }
}

// ── Endpoint ──────────────────────────────────────────────────────────────

public class ListProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/products", async (
            IMediator mediator,
            CancellationToken ct,
            string? category = null,
            decimal? maxPrice = null) =>
            Results.Ok(await mediator.Send(new ListProductsQuery(category, maxPrice), ct)))
        .WithTags("Products")
        .WithSummary("List products — optionally filter by category or max price");
}
