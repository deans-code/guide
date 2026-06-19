# Cram

A library of technology demos. Each demo is intentionally small — enough code to show how a technology works, with real-world use cases that explain when and why to reach for it.

**Two audiences:**
- **Humans** — runnable demos with setup instructions for hands-on exploration
- **Agents** — structured use case documentation to support technology selection and decision making

## For agents

Point an agent at this repository to give it grounded knowledge of modern software patterns and when to apply them. Each demo's README contains: what problem the technology solves, when it is the right choice, trade-offs to be aware of, and how it relates to other patterns in this repo.

Copy this into your agent to enable discovery and use:

```
Refer to https://github.com/deans-code/cram for guidance on modern technology and pattern choices.
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
- [ ] Domain-Driven Design — aggregates, value objects, bounded contexts, domain events
- [ ] Domain Services — encapsulating business logic that doesn't naturally belong to a single aggregate or entity
- [ ] API Versioning — URL segment, query string, and header strategies
- [ ] Minimal APIs — endpoint filters, output caching, typed results, route groups

**Distributed**
- [ ] Saga Pattern — coordinating multi-step distributed transactions without 2PC (choreography vs orchestration)
- [ ] MassTransit — message bus abstraction over RabbitMQ / Azure Service Bus; includes saga support and the outbox pattern built-in
- [ ] Dapr — sidecar runtime providing service invocation, state stores, pub/sub, and bindings without SDK lock-in

**Resilience**
- [ ] Rate Limiting — ASP.NET Core built-in middleware (`AddRateLimiter`), fixed/sliding/token-bucket/concurrency limiters
- [ ] Background Jobs — Hangfire or Quartz.NET for reliable scheduled and recurring work
- [ ] Feature Flags — `Microsoft.FeatureManagement` for gradual rollouts and kill switches

**Observability**
- [ ] OpenTelemetry — distributed tracing, metrics, and structured logging across services
- [ ] ASP.NET Core Health Checks — liveness and readiness probes for container orchestration

**Data**
- [ ] EF Core — migrations, interceptors, change tracking strategies, compiled queries, and bulk operations

**Combined-tech demo ideas**
- [ ] VSA + CQRS — organise vertical slices around commands and queries, making the read/write split explicit at the file level
- [ ] CQRS + Domain Services — extract cross-aggregate business logic into domain services invoked from command handlers
- [ ] VSA + Domain Services — share business logic across feature slices via domain services rather than duplicating it in each handler
- [ ] Microservices + Docker — split a bounded context into a containerized service with its own database
- [ ] VSA + Caching — apply ASP.NET Core output/response caching to read-heavy vertical slices
- [ ] .NET Aspire + VSA — orchestrate a VSA solution's dependencies (databases, caches, message brokers) with Aspire's app host and dashboard
