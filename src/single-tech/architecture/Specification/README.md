# Specification Pattern

**Primary category:** Architecture
**Also relevant to:** Data

Demonstrates the Specification Pattern applied to a product catalogue. Business rules are encapsulated as composable specification objects — each holding an `Expression<Func<T, bool>>` that EF Core translates directly to SQL. Specifications are combined using fluent `.And()`, `.Or()`, and `.Not()` extension methods, letting endpoints build up complex queries from simple, reusable parts without duplicating filter logic.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite

## Run

```bash
cd src/single-tech/architecture/Specification
dotnet run
```

- API: `http://localhost:5203`
- Swagger UI: `http://localhost:5203/scalar/v1`

## Use cases

**What it solves:** Filter logic tends to spread across endpoints as ad-hoc `Where` clauses. When the same rule appears in multiple places — "active products", "in-stock items" — it gets duplicated, diverges, and becomes hard to test in isolation. The Specification Pattern gives each rule a name and a single home.

**When to reach for it:**
- A repository method grows a long parameter list of optional filters (`bool? inStock, string? category, decimal? maxPrice`)
- The same filter condition appears in multiple queries or validation checks
- Business rules need to be combined in different ways across different contexts — catalogue page, admin view, promotional campaign
- Filter logic needs unit tests independent of the database

**How it works:**

Each specification implements `ISpecification<T>` with a single `Expression<Func<T, bool>> Criteria` property. Because the criteria is an expression tree (not a compiled delegate), EF Core translates it to a SQL `WHERE` clause — no in-memory filtering.

Composite specifications (`AndSpecification`, `OrSpecification`, `NotSpecification`) rewrite the expression trees using a `ParameterReplacer` visitor to merge parameters before combining with `Expression.AndAlso` / `Expression.OrElse`. This keeps the SQL translation intact through composition.

```csharp
// Simple specs
ISpecification<Product> spec = new ActiveSpecification()
    .And(new CategorySpecification("Electronics"))
    .And(new PriceRangeSpecification(0, 50));

// Pre-composed for a specific use case
var featuredDeals = new FeaturedSpecification()
    .And(new InStockSpecification())
    .And(new PriceRangeSpecification(0, 50));

// Applied to EF Core — translates to SQL
var results = await db.Products.Where(spec.Criteria).ToListAsync();
```

**Trade-offs to be aware of:**
- Every composition creates a new object; for hot paths with many filters this is negligible but worth knowing
- The expression-tree approach requires a `ParameterReplacer` visitor to merge parameters correctly — `Expression.Invoke` is simpler to write but may not translate to SQL in all providers
- Specifications work well for `WHERE` clauses but do not handle ordering, pagination, or projection — those concerns belong elsewhere (a full Specification with `OrderBy`, `Skip`, `Take` is a heavier pattern)
- Can feel over-engineered for simple CRUD where the filter logic is truly one-off and unlikely to be reused

**Relationship to other patterns:** Specifications are a natural fit inside CQRS query handlers — see [CQRS](../CQRS/README.md). Each query handler builds up a specification from its query parameters and passes it to a repository, keeping the query logic testable and the handler thin.
