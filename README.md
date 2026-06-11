# Cram
Demo code for technologies discussed during interviewing in 2026.

## Projects

### Polly — .NET 10 Minimal API
Demonstrates common Polly v8 resilience strategies: retry (fixed + exponential backoff), circuit breaker, timeout, fallback, hedging, and combined pipelines. Includes `HttpClient` integration via `AddResilienceHandler` and `AddStandardResilienceHandler`.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, Polly v8, Microsoft.Extensions.Http.Resilience

```bash
cd Polly
dotnet run
```

- API: `http://localhost:5138`
- Swagger UI: `http://localhost:5138/scalar/v1`

### Outbox Pattern — .NET 10 Minimal API
Demonstrates the transactional outbox pattern using EF Core and SQLite. Placing an order writes the domain row and an outbox message in a single transaction, eliminating the dual-write problem. A `BackgroundService` polls the outbox table every 5 seconds and forwards messages to a simulated broker with configurable failure rate and automatic retry (dead-letter after 3 attempts).

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite

```bash
cd Outbox
dotnet run
```

- API: `http://localhost:5194`
- Swagger UI: `http://localhost:5194/scalar/v1`
