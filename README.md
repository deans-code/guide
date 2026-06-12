# Cram
Demo code for technologies discussed during interviewing in 2026.

- [Single-Tech Projects](docs/single-tech-projects.md) — how to run each single-technology demo
- [Combined-Tech Projects](docs/combined-tech-projects.md) — how to run each multi-technology demo
- [Use Cases](docs/use-cases.md) — when and why to use each technology

## Suggested topics to explore next

**Patterns**
- Event Sourcing — storing state as an immutable sequence of events rather than current values; natural extension of CQRS
- Saga Pattern — coordinating multi-step distributed transactions without 2PC (choreography vs orchestration)
- Domain-Driven Design — aggregates, value objects, bounded contexts, domain events

**Messaging & integration**
- MassTransit — message bus abstraction over RabbitMQ / Azure Service Bus; includes saga support and the outbox pattern built-in
- Minimal APIs — endpoint filters, output caching, typed results, route groups

**Observability**
- OpenTelemetry — distributed tracing, metrics, and structured logging across services
- ASP.NET Core Health Checks — liveness and readiness probes for container orchestration

**Infrastructure**
- API Versioning — URL segment, query string, and header strategies
- Rate Limiting — ASP.NET Core built-in middleware (`AddRateLimiter`), fixed/sliding/token-bucket/concurrency limiters
- Background Jobs — Hangfire or Quartz.NET for reliable scheduled and recurring work
- Feature Flags — `Microsoft.FeatureManagement` for gradual rollouts and kill switches
