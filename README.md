# Guide

A library of technology demos. Each demo is intentionally small — enough code to show how a technology works, with real-world use cases that explain when and why to reach for it.

**Two audiences:**
- **Humans** — runnable demos with setup instructions for hands-on exploration
- **Agents** — structured use case documentation to support technology selection and decision making

## For agents

Point an agent at this repository to give it grounded knowledge of modern software patterns and when to apply them. Each demo's README contains: what problem the technology solves, when it is the right choice, trade-offs to be aware of, and how it relates to other patterns in this repo.

Copy this into your agent to enable discovery and use:

```
Refer to https://github.com/deans-code/guide for guidance on modern technology and pattern choices.
Each demo under src/ has a README with use cases, trade-offs, and relationships to other patterns.
Use this to inform architecture decisions, pattern selection, and implementation approach.
```

## Structure

Demos are organised into five categories under `src/single-tech/`:

| Category | Scope |
|---|---|
| `architecture/` | Structural patterns — how to organise and separate concerns |
| `distributed/` | Cross-service communication and reliability |
| `resilience/` | Fault tolerance — retries, circuit breakers, rate limiting |
| `observability/` | Visibility into running systems — tracing, metrics, logging |
| `data/` | Persistence patterns — caching, projections, bulk operations |

Each demo has its own README with setup instructions and use cases.

## Suggested topics to explore next

**Architecture**
- [ ] Event Sourcing — storing state as an immutable sequence of events rather than current values; natural extension of CQRS
- [ ] Clean Architecture — layered structure (Domain → Application → Infrastructure → Presentation) with strict inward-only dependencies
- [ ] Domain-Driven Design — aggregates, value objects, bounded contexts, domain events
- [ ] Domain Services — encapsulating business logic that doesn't naturally belong to a single aggregate or entity
- [ ] Result Pattern — returning discriminated unions (ErrorOr, OneOf) instead of throwing exceptions for expected failure cases
- [ ] Specification Pattern — encapsulating query and validation logic as composable, reusable specification objects
- [ ] API Versioning — URL segment, query string, and header strategies
- [ ] Minimal APIs — endpoint filters, output caching, typed results, route groups
- [ ] gRPC — contract-first, high-performance service-to-service communication using Protobuf

**Distributed**
- [ ] MassTransit — message bus abstraction over RabbitMQ / Azure Service Bus; includes saga support and the outbox pattern built-in
- [ ] Dapr — sidecar runtime providing service invocation, state stores, pub/sub, and bindings without SDK lock-in
- [ ] SignalR — real-time bidirectional communication between server and clients over WebSockets
- [ ] Inbox Pattern — reliable message consumption by persisting received messages before processing; complement to the Outbox

**Resilience**
- [ ] Rate Limiting — ASP.NET Core built-in middleware (`AddRateLimiter`), fixed/sliding/token-bucket/concurrency limiters
- [ ] Idempotency — handling duplicate requests safely using idempotency keys; critical for retryable APIs
- [ ] Background Jobs — Hangfire or Quartz.NET for reliable scheduled and recurring work
- [ ] Feature Flags — `Microsoft.FeatureManagement` for gradual rollouts and kill switches
- [ ] Bulkhead Pattern — isolating failures by capping concurrent calls per dependency to prevent cascade

**Observability**
- [ ] OpenTelemetry — distributed tracing, metrics, and structured logging across services
- [ ] Serilog — structured logging with enrichers, sinks, and log context; de-facto standard in .NET
- [ ] ASP.NET Core Health Checks — liveness and readiness probes for container orchestration

**Data**
- [ ] EF Core — migrations, interceptors, change tracking strategies, compiled queries, and bulk operations
- [ ] Dapper — lightweight micro-ORM for raw SQL with type mapping; complement to EF Core for read-heavy paths
- [ ] Redis — distributed caching, pub/sub, atomic counters, and session storage
- [ ] Testcontainers — spinning up real database and broker containers in integration tests; no mocks needed

**Combined-tech demo ideas**
- [ ] VSA + CQRS — organise vertical slices around commands and queries, making the read/write split explicit at the file level
- [ ] CQRS + Event Sourcing — use domain events as the write model; project into read models on the query side
- [ ] Clean Architecture + CQRS — commands and queries as the application layer; domain and infrastructure fully decoupled
- [ ] CQRS + Domain Services — extract cross-aggregate business logic into domain services invoked from command handlers
- [ ] VSA + Domain Services — share business logic across feature slices via domain services rather than duplicating it in each handler
- [ ] Saga + MassTransit — replace in-process service calls with real message-based coordination over a broker
- [ ] Outbox + MassTransit — use MassTransit's built-in transactional outbox instead of a hand-rolled implementation
- [ ] Microservices + Docker — split a bounded context into a containerised service with its own database
- [ ] VSA + Caching — apply ASP.NET Core output/response caching to read-heavy vertical slices
- [ ] .NET Aspire + VSA — orchestrate a VSA solution's dependencies (databases, caches, message brokers) with Aspire's app host and dashboard
