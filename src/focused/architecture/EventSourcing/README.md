# Event Sourcing

**Primary category:** Architecture
**Also relevant to:** Data, Distributed

Demonstrates event sourcing using a bank account aggregate. State is never stored as current values — instead, every change is appended as an immutable event (`AccountOpened`, `MoneyDeposited`, `MoneyWithdrawn`) to a SQLite event store. Current balance is derived on demand by replaying the event stream. Optimistic concurrency prevents conflicting concurrent writes via version checking.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite, System.Text.Json

## Run

```bash
cd src/focused/architecture/EventSourcing
dotnet run
```

- API: `http://localhost:5201`
- Swagger UI: `http://localhost:5201/scalar/v1`

## Use cases

**What it solves:** Systems that need a full, immutable history of how state changed over time — not just what the current state is. Traditional CRUD overwrites state; event sourcing preserves every transition so you can audit, replay, and project state into any shape at any point in time.

**When to reach for it:**
- Financial systems where every debit and credit must be preserved and auditable
- Order management where the full lifecycle (placed → confirmed → shipped → returned) matters, not just the current status
- Collaborative or conflict-prone domains where you need to understand the sequence of changes, not just the outcome
- Systems that need to rebuild or backfill read models when query requirements change

**Core concepts:**

| Concept | Role in this demo |
|---|---|
| Event | An immutable fact: `MoneyDeposited`, `MoneyWithdrawn` |
| Event store | Append-only SQLite table; the single source of truth |
| Aggregate | `BankAccount` — rebuilds its state by replaying its event stream |
| Rehydration | Loading an aggregate by replaying all events for its stream ID |
| Optimistic concurrency | Version check on append prevents two writers clobbering each other |

**Trade-offs to be aware of:**
- Replaying a long event stream on every read is slow — snapshots (periodically checkpointing current state) are the standard mitigation
- Querying across aggregates requires projections; the event store itself is not queryable like a relational table
- Evolving event schemas over time requires careful versioning — old events must still deserialise correctly after the shape changes

**Relationship to other patterns:** Event Sourcing pairs naturally with [CQRS](../CQRS/README.md) — the write side appends events, and the read side builds projections from those events. The [Outbox Pattern](../../distributed/Outbox/README.md) solves the problem of reliably publishing those events to other services after they are written to the store.
