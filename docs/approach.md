# Approaching a Challenge

## Start simple

Build the simplest thing that meets the requirements. A working API with a single database and no patterns is a valid — often ideal — starting point. It is easier to reason about, faster to change, and less likely to break.

Every pattern in this library adds complexity. That complexity only pays off when it solves a specific, real problem you can point to. If you cannot name the problem in one sentence, it is probably not time to introduce the pattern.

**A good first version typically has:**
- A single deployable unit
- A single database
- Direct request handling — no pipelines, mediators, or event buses
- No infrastructure beyond what the feature actually needs

## Recognise when to grow

Patterns earn their place when you feel a specific pain. The signals below map to the demos in this repo; follow the links for use cases, trade-offs, and implementation detail.

---

**Features are hard to navigate — changing one thing touches too many files.**
You have layers (controllers, services, repositories) that cut across features rather than containing them.
→ [Vertical Slice Architecture](../src/single-tech/architecture/VSA/README.md)

---

**Different parts of the system evolve at different rates, or teams step on each other.**
Some modules change frequently; others are stable. Shared databases create hidden coupling.
→ [Modular Monolith](../src/single-tech/architecture/ModularMonolith/README.md)

---

**Reads and writes are pulling in different directions.**
Queries need denormalised or aggregated data that does not match the write model. Read load significantly outweighs write load.
→ [CQRS](../src/single-tech/architecture/CQRS/README.md)

---

**You need a full history, not just current state.**
Auditing is a requirement. You need to know not just what the current state is, but how and when it got there. State must be reconstructable at any point in time.
→ [Event Sourcing](../src/single-tech/architecture/EventSourcing/README.md)

---

**Service methods throw exceptions for expected business failures — product not found, quota exceeded, invalid state.**
Exceptions cross-cut the call stack and are caught far from where the decision was made. Callers cannot tell from the method signature what can go wrong, and each failure path requires its own catch block.
→ [Result Pattern](../src/single-tech/architecture/ResultPattern/README.md)

---

**The same filter logic appears in multiple places, or a repository method accumulates optional parameters for every possible combination.**
Query conditions start as simple `Where` clauses but get duplicated across endpoints and diverge over time. A "featured and in-stock" rule appears in the catalogue, the promotional API, and the admin view — each slightly different.
→ [Specification Pattern](../src/single-tech/architecture/Specification/README.md)

---

**Downstream calls fail, and those failures propagate into your API.**
External services are unreliable. Timeouts hang threads. A single slow dependency brings down unrelated features.
→ [Polly](../src/single-tech/resilience/Polly/README.md)

---

**You write to a database and publish a message in the same operation.**
If either step fails independently, the system ends up in an inconsistent state. You cannot use a distributed transaction.
→ [Outbox Pattern](../src/single-tech/distributed/Outbox/README.md)

---

**A multi-step operation must either complete fully or cleanly undo.**
You call multiple services in sequence and a mid-flow failure leaves things in a partially-applied state with no automatic recovery.
→ [Saga Pattern](../src/single-tech/distributed/Saga/README.md)

---

**Clients retry on failure and duplicates are appearing.**
A timeout does not mean the operation failed — the client cannot tell. Retrying causes double charges, duplicate orders, or repeated notifications.
→ [Idempotency](../src/single-tech/resilience/Idempotency/README.md)

---

## Expand incrementally

When a signal appears, introduce the minimum change needed to address it. Do not apply multiple patterns at once unless you are solving multiple independent problems.

**Refactor, do not rewrite.** Most patterns can be introduced into an existing system gradually:
- Introduce VSA one slice at a time, leaving the rest of the codebase in place
- Add an outbox table to an existing service without restructuring everything around it
- Wrap one unreliable `HttpClient` in a Polly policy before wrapping all of them
- Add idempotency to payment endpoints first, then extend to other mutation endpoints

**Keep each change motivated.** Before introducing a pattern, write down the problem it solves. If you cannot do that clearly, wait until the pain is concrete.

## Patterns to avoid prematurely

| Pattern | Avoid when |
|---|---|
| CQRS | Reads and writes share the same shape; no meaningful performance difference between them |
| Event Sourcing | You have no audit, history, or time-travel requirement; CRUD is sufficient |
| Saga | The operation does not cross a service boundary; a database transaction is simpler and stronger |
| Outbox | There is no dual-write risk; the operation does not involve a message broker |
| Modular Monolith | The team is small and the domain is not yet well understood; premature module boundaries slow you down |
| Idempotency | The operation is naturally safe to repeat (reads, or writes with no side effects) |
