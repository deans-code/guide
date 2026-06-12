# Single-Tech Projects

## Modular Monolith — .NET 10 Minimal API
Demonstrates a modular monolith made of independently deployable-looking modules within a single host. `Catalog` and `Ordering` are separate class library projects, each owning its own SQLite database and internal types (`internal` DbContexts, entities, feature handlers). The only way `Ordering` can read or update catalog data is through `IProductCatalog` — a public contract exposed by `Catalog` — so the module boundary is enforced by the compiler, not just convention. `Api` is a thin host that wires modules up via a shared `IModule` interface.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite (separate database per module)

```bash
cd src/single-tech/ModularMonolith/Api
dotnet run
```

- API: `http://localhost:5288`
- Swagger UI: `http://localhost:5288/scalar/v1`

---

## Polly — .NET 10 Minimal API
Demonstrates common Polly v8 resilience strategies: retry (fixed + exponential backoff), circuit breaker, timeout, fallback, hedging, and combined pipelines. Includes `HttpClient` integration via `AddResilienceHandler` and `AddStandardResilienceHandler`.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, Polly v8, Microsoft.Extensions.Http.Resilience

```bash
cd src/single-tech/Polly
dotnet run
```

- API: `http://localhost:5138`
- Swagger UI: `http://localhost:5138/scalar/v1`

---

## Outbox Pattern — .NET 10 Minimal API
Demonstrates the transactional outbox pattern using EF Core and SQLite. Placing an order writes the domain row and an outbox message in a single transaction, eliminating the dual-write problem. A `BackgroundService` polls the outbox table every 5 seconds and forwards messages to a simulated broker with configurable failure rate and automatic retry (dead-letter after 3 attempts).

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite

```bash
cd src/single-tech/Outbox
dotnet run
```

- API: `http://localhost:5194`
- Swagger UI: `http://localhost:5194/scalar/v1`

---

## Vertical Slice Architecture — .NET 10 Minimal API
Demonstrates vertical slice architecture using MediatR and FluentValidation. Each feature slice (`Features/Products/*.cs`) is entirely self-contained — request, validator, handler, and endpoint registration all live in one file. `Program.cs` stays thin via reflection-based endpoint discovery. A MediatR pipeline behaviour runs validation automatically before every handler.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, MediatR 14, FluentValidation 12, EF Core 10, SQLite

```bash
cd src/single-tech/VSA
dotnet run
```

- API: `http://localhost:5173`
- Swagger UI: `http://localhost:5173/scalar/v1`

---

## CQRS — .NET 10 Minimal API
Demonstrates CQRS with separate read and write models. Commands mutate a normalised write database (`Orders`, `OrderLines`); domain events trigger projection handlers that update a denormalised read database (`OrderSummaries`). Queries never touch the write store. Uses MediatR 12 for command/query dispatch and `INotification` for in-process event propagation.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, MediatR 12, EF Core 10, SQLite (separate write/read databases)

```bash
cd src/single-tech/CQRS
dotnet run
```

- API: `http://localhost:5001`
- Swagger UI: `http://localhost:5001/scalar/v1`
