using FluentValidation;
using MediatR;
using VSADemo.Common;

namespace VSADemo.Features.Products;

// ── Request / Response ────────────────────────────────────────────────────

// The body record excludes Id — the route parameter supplies it.
public record UpdateProductBody(string Name, string Category, decimal Price, int Stock);

public record UpdateProductCommand(Guid Id, string Name, string Category, decimal Price, int Stock)
    : IRequest<UpdateProductResponse?>;

public record UpdateProductResponse(Guid Id, string Name, string Category, decimal Price, int Stock);

// ── Validation ────────────────────────────────────────────────────────────

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

// ── Handler ───────────────────────────────────────────────────────────────

public class UpdateProductHandler(AppDbContext db)
    : IRequestHandler<UpdateProductCommand, UpdateProductResponse?>
{
    public async Task<UpdateProductResponse?> Handle(UpdateProductCommand cmd, CancellationToken ct)
    {
        var product = await db.Products.FindAsync([cmd.Id], ct);
        if (product is null) return null;

        product.Name     = cmd.Name;
        product.Category = cmd.Category;
        product.Price    = cmd.Price;
        product.Stock    = cmd.Stock;

        await db.SaveChangesAsync(ct);

        return new UpdateProductResponse(product.Id, product.Name, product.Category, product.Price, product.Stock);
    }
}

// ── Endpoint ──────────────────────────────────────────────────────────────

public class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("/products/{id:guid}", async (
            Guid id,
            UpdateProductBody body,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new UpdateProductCommand(id, body.Name, body.Category, body.Price, body.Stock), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithTags("Products")
        .WithSummary("Update a product");
}
