# Outbox Pattern

**Primary category:** Distributed
**Also relevant to:** Resilience, Data

Demonstrates the transactional outbox pattern using EF Core and SQLite. Placing an order writes the domain row and an outbox message in a single transaction, eliminating the dual-write problem. A `BackgroundService` polls the outbox table every 5 seconds and forwards messages to a simulated broker with configurable failure rate and automatic retry (dead-letter after 3 attempts).

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite

## Run

```bash
cd src/focused/distributed/Outbox
dotnet run
```

- API: `http://localhost:5194`
- Swagger UI: `http://localhost:5194/scalar/v1`

## Use cases

**What it solves:** The dual-write problem. When a service needs to both save data to a database and publish an event to a message broker, doing them as two separate operations risks one succeeding and the other failing — leaving the system in an inconsistent state.

**When to reach for it:**
- A domain event must be published reliably after a database write (e.g. order placed → notify fulfilment)
- The message broker is occasionally unavailable and you need at-least-once delivery guarantees
- You cannot use distributed transactions (2PC) — which is most modern microservice deployments

**How it fits into the broader picture:**

The outbox pattern is often a prerequisite for [CQRS](../../architecture/CQRS/README.md) and event sourcing. Before you can project events into a read model or publish domain events to other services, you need a reliable way to capture those events at the point of the write. The outbox provides that.

**Trade-offs to be aware of:**
- Introduces eventual consistency — the broker receives the event shortly after the commit, not atomically with it
- Requires a polling loop or change-data-capture mechanism to drain the outbox
- Consumers must be idempotent — at-least-once delivery means duplicates are possible
