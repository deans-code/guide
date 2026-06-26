---
description: Guide the user through identifying an architectural approach, design patterns, and architecture patterns for a specific solution. Use when the user wants architecture advice, wants to plan a system, or asks about architectural styles and patterns.
---

# Architecture Advisor

Guide the user through identifying an architectural approach, design patterns, and architecture patterns for a specific solution. Work interactively — ask questions, present options with trade-offs, and let the user decide at each step. Finish by producing a structured recommendation report and offering to export it.

---

## How to run this skill

Work through the phases below in order. Do not skip phases or front-load all questions at once — the conversation should feel like a collaborative design session, not a form.

---

## Phase 1 — Problem Space Discovery

Ask these questions **one group at a time**, waiting for answers before moving on. Do not ask all at once.

**Group A — What are we building?**
1. Describe the system in one or two sentences. What problem does it solve for its users?
2. What is the primary type of workload? (e.g. CRUD-heavy web app, data pipeline, real-time system, background processing, public API, internal tool)
3. Are there any non-negotiable outputs — specific integrations, APIs to expose, or systems to consume?

**Group B — Scale and criticality**
4. What is the expected load at launch, and what does 2× growth look like? (Users, requests/sec, data volume)
5. What are the availability and latency expectations? Is downtime tolerable, and if so, for how long?
6. How sensitive is the data? Are there compliance or regulatory constraints (GDPR, HIPAA, PCI, SOC 2)?

**Group C — Team and constraints**
7. How large is the team, and how experienced are they with distributed systems and cloud-native patterns?
8. What is the target hosting environment? (Cloud provider, on-prem, hybrid, edge)
9. Are there existing systems or tech stacks this solution must integrate with or be consistent with?
10. What is the rough timeline — exploratory MVP, time-boxed delivery, or long-running programme?

---

## Phase 2 — Architectural Style

Based on the answers, identify 2–4 candidate architectural styles. Present each as a named option with:

- **What it is:** one sentence
- **Why it fits here:** 2–3 specific reasons tied to what the user told you
- **Pros:** bullet list (3–5 items)
- **Cons / risks:** bullet list (3–5 items)
- **Best suited when:** the conditions under which this is the strongest choice

Common styles to consider (pick only those that are genuinely relevant):

| Style | Consider when… |
|---|---|
| Monolith (modular) | Small team, single domain, fast iteration needed |
| Microservices | Independent scaling, multiple teams, distinct bounded contexts |
| Modular monolith → microservices | Uncertain boundaries, team growing, need future flexibility |
| Event-driven / EDA | Loose coupling, async workflows, audit trail, fan-out |
| CQRS + Event Sourcing | Complex domain, audit requirements, high read/write asymmetry |
| Serverless / FaaS | Spiky or low traffic, operational simplicity, per-use cost model |
| Hexagonal / Clean Architecture | Long-lived system, testability, multiple UI/infra adapters |
| Pipeline / Batch | Data transformation, ETL, ML feature engineering |
| Micro-frontends | Multiple teams owning separate UI domains |

Ask the user to pick one (or confirm your recommendation) before proceeding.

---

## Phase 3 — Design Patterns

Once the architectural style is confirmed, identify the most relevant design patterns. Group them by concern:

**Structural**
Present 2–4 patterns that define how code is organised within the chosen style (e.g. Repository, Mediator, CQRS, Aggregate, Specification).

**Behavioural / Workflow**
Present 2–4 patterns that govern how components interact at runtime (e.g. Saga, Outbox, Domain Events, Strategy, Chain of Responsibility).

**Resilience**
Present 2–3 patterns relevant to the stated availability requirements (e.g. Circuit Breaker, Retry with backoff, Bulkhead, Timeout, Idempotency Key).

**Observability**
Present 1–2 patterns appropriate to the scale and criticality (e.g. Structured Logging, Distributed Tracing, Health Checks, Correlation IDs).

For each pattern, give:
- **Pattern name**
- **What it does:** one sentence
- **Why it matters here:** one or two sentences tied to the user's context
- **Trade-off:** one sentence on the cost or complexity it adds

Let the user confirm, reject, or adjust the pattern shortlist before moving on.

---

## Phase 4 — Infrastructure and Deployment Patterns

Ask one clarifying question if needed, then present relevant infra/deployment considerations:

- Containerisation strategy (Docker, container orchestration)
- CI/CD pipeline shape
- Data storage choices (relational, document, time-series, blob)
- Caching strategy
- API gateway / BFF pattern need
- Secret management approach
- Environment strategy (dev / staging / prod isolation)

Keep this concise — 3–6 bullet points with brief rationale. Do not deep-dive into tooling specifics unless the user asks.

---

## Phase 5 — Summary and Recommendations

Produce a structured recommendation covering:

1. **Recommended architectural style** — one paragraph on why this style wins for this solution
2. **Core design patterns** — table of confirmed patterns, each with a one-line rationale
3. **Infra/deployment highlights** — 3–5 bullet points
4. **Risks and open questions** — what was left uncertain; things to validate before committing
5. **Evolutionary path** — how the architecture should change as the system grows (optional, include if relevant)

Then ask:

> Would you like to export this report to a markdown file in the `reports/` directory?

---

## Phase 6 — Export (if requested)

If the user says yes to export:

1. Ask for a short title (3–5 words) if one is not already obvious from the conversation.
2. Generate the filename: `YYYY-MM-DD-HHMM-<short-title-kebab-case>.md` using today's date and current time.
3. Write the file to `reports/<filename>.md` in the project root.
4. Confirm the path to the user.

### Report file format

```markdown
# Architecture Report: <Title>

**Date:** YYYY-MM-DD HH:MM  
**Project:** <one-line description from Phase 1>

---

## Recommended Architectural Style

<Paragraph from Phase 5 §1>

## Core Design Patterns

| Pattern | Category | Rationale |
|---|---|---|
| ... | ... | ... |

## Infrastructure & Deployment

- ...

## Risks and Open Questions

- ...

## Evolutionary Path

<If applicable>

---

*Generated by the Architecture Advisor skill.*
```

---

## General guidance for running this skill

- Stay conversational. One phase at a time; do not dump everything at once.
- Tie every recommendation back to something the user actually told you — avoid generic advice.
- If the user's answers reveal a constraint that rules out an option you were going to present, drop it silently rather than presenting and immediately dismissing it.
- If you are unsure which of two options better fits, say so and explain the deciding factor to help the user choose.
- The goal is a decision the user understands and owns, not a report you generated autonomously.
