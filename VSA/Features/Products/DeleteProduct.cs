using MediatR;
using VSADemo.Common;

namespace VSADemo.Features.Products;

// ── Request / Response ────────────────────────────────────────────────────

public record DeleteProductCommand(Guid Id) : IRequest<bool>;

// ── Handler ───────────────────────────────────────────────────────────────

public class DeleteProductHandler(AppDbContext db)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand cmd, CancellationToken ct)
    {
        var product = await db.Products.FindAsync([cmd.Id], ct);
        if (product is null) return false;

        db.Products.Remove(product);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

// ── Endpoint ──────────────────────────────────────────────────────────────

public class DeleteProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("/products/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
            await mediator.Send(new DeleteProductCommand(id), ct)
                ? Results.NoContent()
                : Results.NotFound())
        .WithTags("Products")
        .WithSummary("Delete a product");
}
