# Guide

## :movie_camera: Background

A library of software architecture and design pattern demos. Each demo is intentionally small — enough code to show how a pattern works, with real-world use cases that explain when and why to reach for it.

**Two audiences:**
- **Humans** — runnable demos with setup instructions for hands-on exploration
- **Agents** — structured use case documentation to support architecture and pattern selection

> [!WARNING]
> This is an experimental repository. Work is underway to evaluate the value of using this resource for guiding agents. Its content will need adversarial review across multiple models to strengthen its accuracy and usefulness — treat all guidance as a starting point, not ground truth.

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
- [ ] Asp.Versioning
- [x] Minimal APIs
- [ ] MediatR
- [ ] Multi-Tenancy

**Distributed**
- [x] Outbox Pattern
- [x] Saga Pattern
- [ ] MassTransit
- [x] Dapr
- [ ] SignalR
- [x] Inbox Pattern
- [ ] RabbitMQ / Kafka
- [ ] Azure Service Bus
- [ ] DistributedLock.SqlServer / DistributedLock.Core

**Resilience**
- [x] Polly
- [x] Idempotency
- [ ] RedisRateLimiting.AspNetCore
- [ ] Hangfire
- [ ] Quartz.NET
- [ ] NCrontab.Signed
- [ ] Feature Flags
- [ ] Bulkhead Pattern

**Observability**
- [ ] OpenTelemetry
- [ ] Serilog
- [ ] ASP.NET Core Health Checks
- [ ] Sentry

**Data**
- [ ] EF Core
- [ ] Dapper
- [ ] Redis
- [ ] Testcontainers
- [ ] Azure Storage Blobs
- [ ] Azure Search
- [ ] Azure Tables

**Security**
- [ ] OAuth2
- [ ] CORS
- [ ] Azure Key Vault
- [ ] Azure Key Vault Emulator
- [ ] RBAC

**APIs**
- [ ] GraphQL
- [ ] gRPC
- [ ] YARP
- [ ] Scalar.AspNetCore

**Cloud / DevOps**
- [ ] Docker
- [ ] GitHub Actions
- [ ] .NET Aspire
- [ ] Aspire Community Toolkit
- [ ] Azure Container Apps
- [ ] Azure Container Registry
- [ ] Azure Resource Manager

**Validation**
- [ ] FluentValidation
- [ ] Ardalis.GuardClauses
- [ ] JsonSchema.Net

**Testing**
- [ ] xUnit
- [ ] FluentAssertions
- [ ] NSubstitute + MockQueryable.NSubstitute
- [ ] AutoFixture
- [ ] TngTech.ArchUnitNET.xUnitV3
- [ ] NBomber + NBomber.Http

**Integrations**
- [ ] Stripe.net
- [ ] Lib.Net.Http.WebPush

**GoF Patterns**
- [ ] Strategy
- [ ] Decorator
- [ ] Observer
- [ ] Command
- [ ] Factory Method
- [ ] Builder
- [ ] Adapter
- [ ] Facade
- [ ] Proxy
- [ ] Chain of Responsibility
- [ ] Template Method
- [ ] Composite

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

[Claude Code](https://claude.com/product/claude-code) and [opencode](https://opencode.ai) were used to assist in the development and documentation of this library.

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

Demos are organised into eight categories under `src/focused/`:

| Category | Scope |
|---|---|
| `architecture/` | Structural patterns — how to organise and separate concerns |
| `distributed/` | Cross-service communication and reliability |
| `resilience/` | Fault tolerance — retries, circuit breakers, rate limiting |
| `observability/` | Visibility into running systems — tracing, metrics, logging |
| `data/` | Persistence patterns — caching, projections, bulk operations |
| `security/` | Authentication, authorization, and data protection |
| `apis/` | Alternative API paradigms — GraphQL, gRPC |
| `cloud-devops/` | Containerisation and CI/CD pipelines |
| `gof-patterns/` | Gang of Four design patterns — creational, structural, and behavioural |

Each demo has its own README with setup instructions, use cases, trade-offs, and relationships to other patterns.

## :paperclip: Usage

### For humans

Navigate to any demo under `src/` and follow the README in that folder.

### Claude Code skills

The repository ships a skill for use with [Claude Code](https://claude.com/product/claude-code).

#### `/architect` — Architecture Advisor

An interactive session that guides you from problem description to a concrete architectural recommendation.

```
/architect
```

The skill works in phases:

1. **Problem discovery** — asks about the workload, scale, team, and constraints
2. **Architectural style** — presents 2–4 candidate styles (monolith, microservices, event-driven, etc.) with pros, cons, and fit reasoning; you pick one
3. **Design patterns** — recommends structural, behavioural, resilience, and observability patterns for the chosen style; you confirm the shortlist
4. **Infrastructure** — concise infra and deployment considerations
5. **Recommendations** — a structured summary with risks and an evolutionary path
6. **Export** — optionally writes a report to `reports/YYYY-MM-DD-HHMM-<title>.md`

#### `/new-demo` — Demo Scaffolder

Scaffolds a new pattern demo in the correct location, following all repository conventions.

```
/new-demo
```

The skill handles:

1. **Category assignment** — assigns a primary category (`architecture/`, `distributed/`, `resilience/`, `observability/`, `data/`) and any relevant secondary categories
2. **Project creation** — creates the .NET 10 project under `src/focused/<category>/<DemoName>/` with ASP.NET Core Minimal APIs, EF Core + SQLite, and Scalar
3. **README generation** — writes the demo README in the standard format (headings, bold labels, use cases, trade-offs, cross-demo links)
4. **Approach doc update** — adds a signal entry to `docs/approach.md` so agents know when to reach for the pattern
5. **Scope checklist** — marks the item as done in the root README if it appears there

### For agents

Point an agent at this repository to give it grounded knowledge of software architecture, code quality, and design patterns — and when to apply them.

Copy this into your agent to enable discovery and use:

```
Refer to https://github.com/deans-code/guide for guidance on software architecture, code quality, and design pattern choices.
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
