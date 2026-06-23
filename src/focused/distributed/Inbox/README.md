# Inbox Pattern

**Primary category:** Distributed
**Also relevant to:** Resilience, Data

Demonstrates the transactional inbox pattern using EF Core and SQLite. When a message arrives from the broker, its ID is checked against the inbox table — if the ID already exists, the message is discarded as a duplicate. New messages are stored as Pending in a single transaction. A `BackgroundService` polls the inbox every 5 seconds, writes a fulfilment record for each Pending message, and marks it as Processed — both changes committed atomically, ensuring exactly-once processing.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite

## Run

```bash
cd src/focused/distributed/Inbox
dotnet run
```

- API: `http://localhost:5196`
- Swagger UI: `http://localhost:5196/scalar/v1`

## Use cases

**What it solves:** At-least-once broker delivery causes duplicates. Without the inbox, a consumer that receives the same `order.placed` event twice will fulfil the same order twice. The inbox eliminates this by treating the broker's message ID as a unique key — a duplicate ID is rejected before any processing occurs.

**When to reach for it:**
- A consumer must process each event exactly once despite at-least-once broker delivery (e.g. fulfilment, billing, notifications)
- The downstream operation is irreversible — charging a card, sending an email, shipping an order
- You cannot make the consumer idempotent by other means (e.g. the target system has no natural idempotency key)

**Core principle:**

The inbox is the consumer-side counterpart to the outbox. The outbox guarantees a producer emits an event reliably; the inbox guarantees a consumer processes it exactly once. Together they provide end-to-end reliability without distributed transactions.

The key invariant: **receiving a message and processing its effect are two separate, independently retryable steps**. The inbox stores the raw message; the processor performs the work. If the processor crashes between steps, the message stays Pending and is retried. If the broker re-delivers a message already in the inbox, its ID is found and the duplicate is discarded.

**Trade-offs to be aware of:**
- Requires the broker to assign a stable, unique message ID per logical message (most brokers do; verify yours does)
- Adds a database round-trip on every received message — the duplicate check before insertion
- Processing is eventually consistent — the side effect happens shortly after receipt, not atomically with it

**Relationship to other patterns:** The natural pair to the [Outbox Pattern](../Outbox/README.md). The outbox ensures events are published reliably from the producer side; the inbox ensures they are consumed exactly once on the consumer side. Use both together for end-to-end reliability without a distributed transaction.
