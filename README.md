# Cram
Demo code for technologies discussed during interviewing in 2026.

- Each demo in `src/` has its own README with setup instructions and use cases

## Suggested topics to explore next

**Architecture**
- [ ] Event Sourcing — storing state as an immutable sequence of events rather than current values; natural extension of CQRS
- [ ] Saga Pattern — coordinating multi-step distributed transactions without 2PC (choreography vs orchestration)
- [ ] Domain-Driven Design — aggregates, value objects, bounded contexts, domain events
- [ ] Domain Services — encapsulating business logic that doesn't naturally belong to a single aggregate or entity
- [ ] Minimal APIs — endpoint filters, output caching, typed results, route groups

**Distributed**
- [ ] MassTransit — message bus abstraction over RabbitMQ / Azure Service Bus; includes saga support and the outbox pattern built-in
- [ ] Dapr — sidecar runtime providing service invocation, state stores, pub/sub, and bindings without SDK lock-in

**Resilience**
- [ ] Rate Limiting — ASP.NET Core built-in middleware (`AddRateLimiter`), fixed/sliding/token-bucket/concurrency limiters
- [ ] Background Jobs — Hangfire or Quartz.NET for reliable scheduled and recurring work
- [ ] Feature Flags — `Microsoft.FeatureManagement` for gradual rollouts and kill switches
- [ ] API Versioning — URL segment, query string, and header strategies

**Observability**
- [ ] OpenTelemetry — distributed tracing, metrics, and structured logging across services
- [ ] ASP.NET Core Health Checks — liveness and readiness probes for container orchestration

**Data**
- [ ] EF Core — migrations, interceptors, change tracking strategies, compiled queries, and bulk operations

**Frontend**
- [ ] Micro-frontends — composing a UI from independently deployed frontend apps (module federation, web components, or server-side composition)

**Combined-tech demo ideas**
- [ ] VSA + CQRS — organise vertical slices around commands and queries, making the read/write split explicit at the file level
- [ ] CQRS + Domain Services — extract cross-aggregate business logic into domain services invoked from command handlers
- [ ] VSA + Domain Services — share business logic across feature slices via domain services rather than duplicating it in each handler
- [ ] Microservices + Docker — split a bounded context into a containerized service with its own database
- [ ] VSA + Caching — apply ASP.NET Core output/response caching to read-heavy vertical slices
- [ ] .NET Aspire + VSA — orchestrate a VSA solution's dependencies (databases, caches, message brokers) with Aspire's app host and dashboard
