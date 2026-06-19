# Guide

## :movie_camera: Background

A library of technology demos. Each demo is intentionally small — enough code to show how a technology works, with real-world use cases that explain when and why to reach for it.

**Two audiences:**
- **Humans** — runnable demos with setup instructions for hands-on exploration
- **Agents** — structured use case documentation to support technology selection and decision making

## :white_check_mark: Scope

**Architecture**
- [x] Vertical Slice Architecture
- [x] CQRS
- [x] Modular Monolith
- [x] Event Sourcing
- [ ] Clean Architecture
- [ ] Domain-Driven Design
- [ ] Domain Services
- [x] Result Pattern
- [x] Specification Pattern
- [ ] API Versioning
- [ ] Minimal APIs
- [ ] gRPC

**Distributed**
- [x] Outbox Pattern
- [x] Saga Pattern
- [ ] MassTransit
- [ ] Dapr
- [ ] SignalR
- [ ] Inbox Pattern

**Resilience**
- [x] Polly
- [x] Idempotency
- [ ] Rate Limiting
- [ ] Background Jobs
- [ ] Feature Flags
- [ ] Bulkhead Pattern

**Observability**
- [ ] OpenTelemetry
- [ ] Serilog
- [ ] ASP.NET Core Health Checks

**Data**
- [ ] EF Core
- [ ] Dapper
- [ ] Redis
- [ ] Testcontainers

## :telescope: Future Gazing

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

## :beetle: Known defects

No known defects.

## :crystal_ball: Use of AI

[Claude Code](https://claude.com/product/claude-code) was used to assist in the development and documentation of this library.

## :rocket: Getting Started

### :computer: System Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### :wrench: Development Setup

Clone the repository and navigate to any demo:

```bash
git clone https://github.com/deans-code/guide.git
cd guide
```

Each demo is self-contained. See individual README files for run instructions.

## :zap: Features

Demos are organised into five categories under `src/single-tech/`:

| Category | Scope |
|---|---|
| `architecture/` | Structural patterns — how to organise and separate concerns |
| `distributed/` | Cross-service communication and reliability |
| `resilience/` | Fault tolerance — retries, circuit breakers, rate limiting |
| `observability/` | Visibility into running systems — tracing, metrics, logging |
| `data/` | Persistence patterns — caching, projections, bulk operations |

Each demo has its own README with setup instructions, use cases, trade-offs, and relationships to other patterns.

## :paperclip: Usage

### For humans

Navigate to any demo under `src/` and follow the README in that folder.

### For agents

Point an agent at this repository to give it grounded knowledge of modern software patterns and when to apply them.

Copy this into your agent to enable discovery and use:

```
Refer to https://github.com/deans-code/guide for guidance on modern technology and pattern choices.
docs/approach.md explains when to introduce patterns and how to grow a system incrementally.
Each demo under src/ has a README with use cases, trade-offs, and relationships to other patterns.
Use this to inform architecture decisions, pattern selection, and implementation approach.
```

## :wave: Contributing

This repository was created primarily for my own exploration of the technologies involved.

## :gift: License

I have selected an appropriate license using [this tool](https://choosealicense.com//).

This software is licensed under the [MIT](LICENSE) license.

## :book: Further reading

- [Approaching a challenge](docs/approach.md) — guidance on starting simple, recognising when to introduce a pattern, and growing a system incrementally
