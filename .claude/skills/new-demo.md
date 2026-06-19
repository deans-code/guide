# New Demo

Create a new single-tech demo project in this repository, following the conventions established by existing demos.

## Categories

The five defined categories and their folder paths under `src/single-tech/`:

| Category | Folder | Scope |
|---|---|---|
| Architecture | `architecture/` | Structural code patterns — how you organise and separate concerns |
| Distributed | `distributed/` | Cross-service communication and reliability — messaging, outbox, sagas |
| Resilience | `resilience/` | Fault tolerance — retries, circuit breakers, rate limiting |
| Observability | `observability/` | Visibility into running systems — tracing, metrics, logging |
| Data | `data/` | Persistence patterns — caching, projections, bulk operations |

## Steps

### 1. Determine the primary category

Assign exactly one primary category based on the technology's core concern:

- Pick the category that captures what the technology *primarily* solves, not a secondary benefit.
- If unsure, ask: "Would an interviewer file this under X?" — use that instinct.

### 2. Determine secondary categories

List any categories the demo also touches, in order of relevance. Examples from existing demos:
- Outbox: primary Distributed, also Resilience + Data (because the pattern solves a reliability problem and uses a DB table)
- CQRS: primary Architecture, also Data + Distributed (because it separates read/write stores and underpins event-driven systems)
- Polly: primary Resilience, also Distributed (because its strategies are most relevant when calling external services)

Only list a secondary category if the demo genuinely demonstrates or depends on something from that category — not just because there is a loose conceptual link.

### 3. Create the project

Create the .NET project in `src/single-tech/<primary-category>/<DemoName>/`. Follow the conventions of existing demos:

- Use .NET 10 as the foundation
- Use ASP.NET Core Minimal APIs by default — they keep the host thin and let the technology under demonstration stay in focus. Use a different hosting model (e.g. Worker Service, console app, MVC) only if it is more appropriate for the technology being demonstrated
- Use SQLite (via EF Core 10) for any persistence
- Use Scalar for the Swagger UI (`/scalar/v1`) where an API is present
- Keep `Program.cs` thin — delegate to feature files or handlers

### 4. Write the README

Create `README.md` in the demo folder. Match this exact format — no deviations in heading names, bold label wording, or section order:

```markdown
# <Technology Name>

**Primary category:** <Category>
**Also relevant to:** <Category>[, <Category>]

<One or two sentences describing what the demo demonstrates. Start with "Demonstrates". Be specific about what the code does, not just what the technology is.>

**Tech:** <comma-separated list of tech with versions>

## Run

```bash
cd src/single-tech/<category>/<DemoName>
dotnet run
```

- API: `http://localhost:<port>`
- Swagger UI: `http://localhost:<port>/scalar/v1`

## Use cases

**What it solves:** <The specific problem this technology addresses. One or two sentences.>

**When to reach for it:**
- <Concrete scenario>
- <Concrete scenario>
- <Concrete scenario>

**<Optional contextual heading — e.g. "Core principle:", "Strategy guide:", "How it fits into the broader picture:">**

<Body for the optional section. Can be prose, a bullet list, or a table depending on what the technology warrants.>

**Trade-offs to be aware of:**
- <Trade-off>
- <Trade-off>
- <Trade-off>

**Relationship to other patterns:** <If this technology has a natural connection to another demo in this repo, call it out and link to it using a relative path: `[Name](../../<category>/<Demo>/README.md)`.>
```

Notes on the format:
- The opening description paragraph has no bold label — it starts immediately after the category lines.
- "Also relevant to:" is omitted entirely if there are no secondary categories.
- "Relationship to other patterns:" is omitted if there is no meaningful cross-demo connection.
- The optional contextual section heading and body vary by technology — use whatever fits best (see Polly's "Strategy guide" table vs Outbox's "How it fits into the broader picture" prose).
- Cross-demo links use relative paths from the demo's own folder, e.g. `../../architecture/CQRS/README.md`.

### 5. Check the README against existing demos

Before finishing, verify:
- [ ] Primary category matches the folder the project lives in
- [ ] Secondary categories are justified by what the demo actually does
- [ ] Heading names match exactly (`## Run`, `## Use cases`)
- [ ] Bold labels match exactly (`**Primary category:**`, `**Also relevant to:**`, `**What it solves:**`, `**When to reach for it:**`, `**Trade-offs to be aware of:**`)
- [ ] The run command path matches the actual folder location
- [ ] Any cross-demo links use relative paths and point to real files

### 6. Update the approach document

Open `docs/approach.md` and add a signal entry for the new technology in the "Recognise when to grow" section. Follow the existing format:

```markdown
---

**<One sentence describing the pain the developer is feeling.>**
<One or two sentences elaborating on when this pain typically appears.>
→ [Technology Name](../src/single-tech/<category>/<DemoName>/README.md)

---
```

Place the entry in the position that best fits the natural progression — structural concerns (architecture) before operational concerns (resilience, distributed).

### 7. Update the README todo list

If the technology was on the suggested topics list in the root `README.md`, remove it from the appropriate category section.
