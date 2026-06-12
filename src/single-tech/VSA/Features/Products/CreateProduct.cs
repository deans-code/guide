using FluentValidation;
using MediatR;
using VSADemo.Common;

namespace VSADemo.Features.Products;

// ── Request / Response ────────────────────────────────────────────────────

public record CreateProductCommand(string Name, string Category, decimal Price, int Stock)
    : IRequest<CreateProductResponse>;

public record CreateProductResponse(Guid Id, string Name, string Category, decimal Price, int Stock, DateTime CreatedAt);

// ── Validation ────────────────────────────────────────────────────────────

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

// ── Handler ───────────────────────────────────────────────────────────────

public class CreateProductHandler(AppDbContext db)
    : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<CreateProductResponse> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var product = new Product
        {
            Id        = Guid.NewGuid(),
            Name      = cmd.Name,
            Category  = cmd.Category,
            Price     = cmd.Price,
            Stock     = cmd.Stock,
            CreatedAt = DateTime.UtcNow
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);

        return new CreateProductResponse(
            product.Id, product.Name, product.Category, product.Price, product.Stock, product.CreatedAt);
    }
}

// ── Endpoint ──────────────────────────────────────────────────────────────

public class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/products", async (CreateProductCommand cmd, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(cmd, ct);
            return Results.Created($"/products/{result.Id}", result);
        })
        .WithTags("Products")
        .WithSummary("Create a product");
}
