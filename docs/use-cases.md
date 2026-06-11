# Use Cases

## Polly

**What it solves:** Transient failures in distributed systems. Network calls, database connections, and downstream HTTP services fail intermittently. Without resilience policies, a single bad response propagates into an unhandled exception or a hung request.

**When to reach for it:**
- Any outbound HTTP call to a third-party API or microservice
- Database connections subject to connection pool exhaustion or brief unavailability
- Message broker publish calls that can experience transient network errors

**Strategy guide:**

| Strategy | Use when |
|---|---|
| Retry (fixed) | Failures are random and brief — transient network blips |
| Retry (exponential + jitter) | Downstream is overloaded — back off to avoid thundering herd |
| Circuit breaker | A dependency is reliably broken — fail fast rather than queue up timeouts |
| Timeout | A call may hang indefinitely — enforce a maximum wait per attempt |
| Fallback | A stale cached response is acceptable when the live service is unavailable |
| Hedging | Tail latency matters — fire parallel requests and take the first to respond |
| Combined pipeline | Production HTTP clients — layer timeout → retry → circuit breaker together |

**Real-world fit:** Any .NET service making outbound calls. The `AddStandardResilienceHandler()` extension is a sensible starting point for most `HttpClient` registrations.

---

## Outbox Pattern

**What it solves:** The dual-write problem. When a service needs to both save data to a database and publish an event to a message broker, doing them as two separate operations risks one succeeding and the other failing — leaving the system in an inconsistent state.

**When to reach for it:**
- A domain event must be published reliably after a database write (e.g. order placed → notify fulfilment)
- The message broker is occasionally unavailable and you need at-least-once delivery guarantees
- You cannot use distributed transactions (2PC) — which is most modern microservice deployments

**How it fits into the broader picture:**

The outbox pattern is often a prerequisite for CQRS and event sourcing. Before you can project events into a read model or publish domain events to other services, you need a reliable way to capture those events at the point of the write. The outbox provides that.

**Trade-offs to be aware of:**
- Introduces eventual consistency — the broker receives the event shortly after the commit, not atomically with it
- Requires a polling loop or change-data-capture mechanism to drain the outbox
- Consumers must be idempotent — at-least-once delivery means duplicates are possible

---

## Vertical Slice Architecture

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

**Relationship to CQRS:** VSA and CQRS complement each other naturally. Organising by slice makes the command/query split explicit at the file level — a `PlaceOrder.cs` in `Write/` and a `ListOrders.cs` in `Read/` are both slices, just on different sides of the CQRS boundary.

---

## CQRS

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

**Relationship to the Outbox Pattern:** In production CQRS, domain events published after a command are the mechanism that drives projections. If the event publish fails, the read model falls behind. The outbox pattern is the standard solution — persist events to an outbox table in the same transaction as the write, then drain reliably to the broker.
