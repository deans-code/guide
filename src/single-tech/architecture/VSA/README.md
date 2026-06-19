# Vertical Slice Architecture

**Primary category:** Architecture
**Also relevant to:** Data

Demonstrates vertical slice architecture using MediatR and FluentValidation. Each feature slice (`Features/Products/*.cs`) is entirely self-contained — request, validator, handler, and endpoint registration all live in one file. `Program.cs` stays thin via reflection-based endpoint discovery. A MediatR pipeline behaviour runs validation automatically before every handler.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, MediatR 14, FluentValidation 12, EF Core 10, SQLite

## Run

```bash
cd src/single-tech/architecture/VSA
dotnet run
```

- API: `http://localhost:5173`
- Swagger UI: `http://localhost:5173/scalar/v1`

## Use cases

**What it solves:** The tendency for layered (N-tier) codebases to accumulate coupling across features. In a traditional Controllers → Services → Repositories structure, adding a new feature touches multiple layers; shared services grow into god classes; changes to one feature risk breaking another.

**When to reach for it:**
- Medium-to-large applications where features evolve independently and at different rates
- Teams where ownership of features matters more than ownership of layers
- Codebases where a single service class has accumulated unrelated responsibilities

**Core principle:** Organise by what changes together, not by what is technically similar. A `CreateOrder` slice will always change for `CreateOrder` reasons — it should live with its validator, handler, and endpoint, not spread across four directories.

**Trade-offs to be aware of:**
- Can feel like over-engineering for simple CRUD with no diverging business logic
- Shared infrastructure (DbContext, auth, cross-cutting behaviours) still needs a home
- Some duplication between slices is intentional — resist the urge to abstract it away prematurely

**Relationship to CQRS:** VSA and CQRS complement each other naturally. Organising by slice makes the command/query split explicit at the file level — a `PlaceOrder.cs` in `Write/` and a `ListOrders.cs` in `Read/` are both slices, just on different sides of the [CQRS](../CQRS/README.md) boundary.
