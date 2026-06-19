# Modular Monolith

**Primary category:** Architecture
**Also relevant to:** Data

Demonstrates a modular monolith made of independently deployable-looking modules within a single host. `Catalog` and `Ordering` are separate class library projects, each owning its own SQLite database and internal types (`internal` DbContexts, entities, feature handlers). The only way `Ordering` can read or update catalog data is through `IProductCatalog` — a public contract exposed by `Catalog` — so the module boundary is enforced by the compiler, not just convention. `Api` is a thin host that wires modules up via a shared `IModule` interface.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite (separate database per module)

## Run

```bash
cd src/single-tech/architecture/ModularMonolith/Api
dotnet run
```

- API: `http://localhost:5288`
- Swagger UI: `http://localhost:5288/scalar/v1`

## Use cases

**What it solves:** The false choice between a tangled monolith and the operational overhead of microservices. A modular monolith organises code into independently-owned modules — each with its own data and a narrow public contract — while still deploying as a single unit.

**When to reach for it:**
- Early-to-mid-stage products that need clear module boundaries but can't yet justify separate deployments, networking, and observability for each service
- Teams that want the option to extract a module into its own service later, without a rewrite
- Codebases where shared-database "big ball of mud" coupling is already causing pain

**Core principle:** Each module owns its data and exposes a small set of public contracts (interfaces + DTOs); everything else — DbContext, entities, handlers — is `internal`. Other modules can only depend on those contracts, so the compiler enforces the boundary that a microservice would otherwise enforce with a network call.

**Trade-offs to be aware of:**
- Discipline is required to keep contracts narrow — it's easy to leak internal types if `internal` isn't used consistently
- Still a single deployable, so a bug in one module can crash the whole process
- Cross-module transactions are awkward by design (mirrors the eventual-consistency problem you'd face with real microservices)

**Relationship to other patterns:** A module's public contract is a natural seam for extracting it into its own service later — at that point the in-process call becomes an HTTP/gRPC call, and the [Outbox Pattern](../../distributed/Outbox/README.md) becomes relevant for keeping data in sync across the new service boundary.
