# Idempotency

**Primary category:** Resilience
**Also relevant to:** Distributed, Architecture

Demonstrates idempotent API design using idempotency keys on a payment endpoint. Clients supply an `Idempotency-Key` header with each request; the server stores the key and response in SQLite. Retrying with the same key returns the cached response without processing the operation again. Supplying the same key with a different request body returns 422, distinguishing a retry from a different request.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, EF Core 10, SQLite, System.Security.Cryptography

## Run

```bash
cd src/focused/resilience/Idempotency
dotnet run
```

- API: `http://localhost:5202`
- Swagger UI: `http://localhost:5202/scalar/v1`

## Use cases

**What it solves:** In distributed systems, clients cannot always tell whether a request succeeded — a timeout might mean the server never received it, or that it processed it but the response was lost. Without idempotency, retrying creates duplicates: double charges, duplicate orders, duplicate notifications.

**When to reach for it:**
- Payment and financial APIs where a duplicate charge is unacceptable
- Order placement endpoints that clients retry on network timeout
- Any state-mutating operation (`POST`, `PATCH`, `DELETE`) exposed over an unreliable network
- Webhook delivery endpoints where the sender retries until it receives a 2xx

**How it works:**

| Scenario | Behaviour |
|---|---|
| First request with key | Processed normally; result cached against the key |
| Retry with same key and same body | Cached response returned; operation not re-executed |
| Same key, different body | 422 — likely a client bug; the key should identify one logical operation |
| No key supplied | Processed normally; no caching |

Keys expire after 24 hours. The `Idempotency-Replayed: true` response header signals to the client that it received a cached response.

**Trade-offs to be aware of:**
- Idempotency keys must be generated client-side before the request is sent — the server cannot assign them after the fact
- The key store itself must be durable; an in-memory cache loses keys on restart and enables duplicates after a crash
- Keys add storage overhead; expiry windows must balance safety (long enough for retries) against cost (short enough to reclaim space)

**Relationship to other patterns:** Idempotency is a prerequisite for safe retries with [Polly](../Polly/README.md) — without it, a retry policy on a payment endpoint risks charging the customer twice. The [Saga Pattern](../../distributed/Saga/README.md) also depends on idempotent compensating transactions, since a compensation step may be retried if the orchestrator crashes mid-execution.
