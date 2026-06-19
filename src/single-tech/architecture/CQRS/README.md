# CQRS

**Primary category:** Architecture
**Also relevant to:** Data, Distributed

Demonstrates CQRS with separate read and write models. Commands mutate a normalised write database (`Orders`, `OrderLines`); domain events trigger projection handlers that update a denormalised read database (`OrderSummaries`). Queries never touch the write store. Uses MediatR 12 for command/query dispatch and `INotification` for in-process event propagation.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, MediatR 12, EF Core 10, SQLite (separate write/read databases)

## Run

```bash
cd src/single-tech/architecture/CQRS
dotnet run
```

- API: `http://localhost:5001`
- Swagger UI: `http://localhost:5001/scalar/v1`

## Use cases

**What it solves:** Read and write workloads have fundamentally different shapes. Writes need consistency, validation, and transactional integrity. Reads need speed, denormalisation, and shapes tailored to specific UI views. A single model trying to serve both ends up poorly optimised for each.

**When to reach for it:**
- Read load significantly outweighs write load and requires independent scaling
- Query shapes diverge substantially from the write model (e.g. aggregated totals, joined projections)
- You need an audit trail or event history — CQRS pairs naturally with event sourcing
- Different consistency requirements: reads can tolerate slight staleness, writes cannot

**When it is overkill:**
- Simple CRUD applications where reads and writes share the same shape
- Small teams or early-stage products where the added complexity outweighs the benefit
- Applications without a meaningful performance differential between read and write paths

**Consistency model to understand:**

In the in-process demo here, events are published synchronously so the read model is immediately consistent. In a real distributed deployment — where write and read services are separate processes communicating via a broker — there is a propagation delay. Queries must tolerate reading stale data, and the UI may need to handle this (e.g. optimistic updates, polling).

**Relationship to the Outbox Pattern:** In production CQRS, domain events published after a command are the mechanism that drives projections. If the event publish fails, the read model falls behind. The [Outbox Pattern](../../distributed/Outbox/README.md) is the standard solution — persist events to an outbox table in the same transaction as the write, then drain reliably to the broker.
