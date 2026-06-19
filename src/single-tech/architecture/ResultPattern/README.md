# Result Pattern

**Primary category:** Architecture

Demonstrates the Result Pattern using the `ErrorOr` library on an order placement API. Service methods return `ErrorOr<T>` instead of throwing exceptions for expected business failures — product not found, insufficient stock, invalid quantity, already cancelled. Each failure is a typed `Error` with a category (`NotFound`, `Validation`, `Conflict`) that the endpoint maps to the correct HTTP status code via `.Match()`.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite, ErrorOr 2.1.1

## Run

```bash
cd src/single-tech/architecture/ResultPattern
dotnet run
```

- API: `http://localhost:5204`
- Swagger UI: `http://localhost:5204/scalar/v1`

## Use cases

**What it solves:** Exceptions are expensive and semantically wrong for predictable business failures. When a product isn't found or stock is insufficient, that is an expected outcome — not an exceptional one. Throwing exceptions for these cases couples error handling to try/catch blocks, loses type information about what went wrong, and makes it harder to see all the ways a method can fail.

**When to reach for it:**
- Service methods have multiple expected failure paths that callers need to handle differently
- You want the compiler to force callers to handle both success and failure — not just the happy path
- Mapping business failures to HTTP status codes needs to be consistent across endpoints
- Unit testing error paths without catching exceptions

**How it works:**

Service methods return `ErrorOr<T>`. The value is either the success result or one or more typed errors. Errors carry a category (`NotFound`, `Validation`, `Conflict`) that drives the HTTP response, and a code + description for the response body.

```csharp
// Service — returns either an Order or a typed error
public async Task<ErrorOr<Order>> PlaceOrderAsync(int productId, int quantity)
{
    if (quantity <= 0)
        return ProductErrors.InvalidQuantity;       // 422

    var product = await db.Products.FindAsync(productId);
    if (product is null)
        return ProductErrors.NotFound(productId);   // 404

    if (product.Stock < quantity)
        return ProductErrors.InsufficientStock(...); // 409

    // ... create and return the order
    return order;
}

// Endpoint — .Match() forces handling of both paths
var result = await service.PlaceOrderAsync(request.ProductId, request.Quantity);
return result.Match(
    order => Results.Created($"/orders/{order.Id}", order),
    errors => errors.ToProblemResult());
```

Errors are defined as static members on dedicated error classes, keeping the vocabulary close to the domain:

```csharp
public static class ProductErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Product.NotFound", $"Product {id} was not found.");

    public static Error InsufficientStock(string name, int available) =>
        Error.Conflict("Product.InsufficientStock", $"'{name}' only has {available} unit(s) in stock.");
}
```

**Trade-offs to be aware of:**
- Adds a dependency on a library (`ErrorOr`, `FluentResults`, `OneOf`) — the pattern itself is language-agnostic but the ergonomics depend on the library
- Return types become more verbose; callers must always handle the error branch
- Not a replacement for exceptions — infrastructure failures (database unreachable, null reference) should still throw; the Result Pattern is for *business* failures
- Teams new to the pattern may reach for `.Value` directly, bypassing the error check — code review or analyser rules help enforce the discipline

**Relationship to other patterns:** Result Pattern pairs naturally with CQRS — see [CQRS](../CQRS/README.md). Command handlers return `ErrorOr<T>`, and the dispatch layer maps errors to responses uniformly rather than each endpoint handling them independently.
