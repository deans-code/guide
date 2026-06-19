# Saga Pattern

**Primary category:** Distributed
**Also relevant to:** Resilience

Demonstrates the orchestration-based saga pattern for coordinating a multi-step distributed transaction. An `OrderFulfillmentSaga` sequences three steps — reserve inventory, process payment, schedule shipment — and automatically runs compensating transactions in reverse order if any step fails, leaving the system in a consistent state.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite

## Run

```bash
cd src/single-tech/distributed/Saga
dotnet run
```

- API: `http://localhost:5200`
- Swagger UI: `http://localhost:5200/scalar/v1`

## Use cases

**What it solves:** Long-running operations that span multiple services or steps, where a distributed transaction (2PC) is unavailable or impractical. Without a saga, a failure halfway through leaves the system in a partially-applied state with no automatic way to recover.

**When to reach for it:**
- An order must reserve stock, charge a card, and book a courier — any of which can fail independently
- A user registration must create an account, send a welcome email, and set up a billing profile across separate services
- A travel booking must confirm a flight, hotel, and car hire as a single logical operation

**Orchestration vs choreography:**

| Approach | How it works | When to use it |
|---|---|---|
| Orchestration (this demo) | A central coordinator calls each step in sequence and triggers compensations | Easier to reason about; good when the flow has a clear owner |
| Choreography | Each service emits events and reacts to others; no central coordinator | Better for loosely coupled services that should not know about each other |

The demo uses orchestration because it makes the saga flow and compensation logic explicit and easy to follow. Choreography is more scalable but harder to trace when things go wrong.

**Trade-offs to be aware of:**
- Sagas guarantee eventual consistency, not atomicity — the system is briefly inconsistent between steps
- Compensating transactions must be idempotent — they may be retried if the orchestrator crashes mid-compensation
- Compensation cannot always achieve a true rollback (e.g. a sent email cannot be unsent); design steps with this in mind

**Relationship to other patterns:** In a real deployment the orchestrator publishes commands to services via a message broker rather than calling them in-process. The [Outbox Pattern](../Outbox/README.md) ensures those commands are delivered reliably even if the broker is temporarily unavailable.
