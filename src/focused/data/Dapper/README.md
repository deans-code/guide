# Dapper

**Primary category:** Data

Demonstrates Dapper as a micro-ORM alternative to EF Core: parameterized CRUD against raw SQL, multi-mapping a JOIN into a nested object graph (an order with its line items) in a single round trip, and a hand-written GROUP BY report. There is deliberately no `DbContext` here — Dapper works directly against `IDbConnection`, with no change tracking and no query translation layer between the SQL you write and the SQL that runs.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, Dapper 2.1, Microsoft.Data.Sqlite 10, SQLite

## Run

```bash
cd src/focused/data/Dapper
dotnet run
```

- API: `http://localhost:5213`
- Swagger UI: `http://localhost:5213/scalar/v1`

## Use cases

**What it solves:** An ORM's change tracking, LINQ translation, and lazy loading earn their keep for typical CRUD, but they get in the way for reporting queries, bulk operations, or anywhere you need exact control over the SQL that executes. Dapper gives you that control while still eliminating the boilerplate of manually reading an `IDataReader` column by column — you write the SQL, Dapper maps the result set onto your objects.

**When to reach for it:**
- Reporting and analytics endpoints where the query is naturally SQL-shaped (joins, aggregates, window functions) and an ORM's query builder fights you
- Performance-sensitive read paths where you've profiled EF Core's overhead and need the fastest path from query to object
- Existing stored-procedure-heavy databases, or teams that already think in SQL and want a thin mapping layer, not a full ORM
- Bulk or set-based operations (`UPDATE ... WHERE`, batched inserts) that are awkward to express through change-tracked entities

**Dapper vs EF Core:**

| | Dapper | EF Core |
|---|---|---|
| You write | Raw SQL | LINQ, translated to SQL |
| Change tracking | None — you write the `UPDATE` | Automatic, via tracked entities |
| Migrations | None built in — manage schema yourself | Built-in migrations |
| Best for | Reads, reports, precise query control | Writes with rich domain models, rapid CRUD |

It's common to use both in the same codebase: EF Core for the transactional write side, Dapper for read-heavy or reporting endpoints where raw SQL is clearer and faster.

**Trade-offs to be aware of:**
- No change tracking means every write is a SQL statement you author and are responsible for getting right — no `SaveChanges()` diffing entities for you
- No migrations — this demo creates its schema with `CREATE TABLE IF NOT EXISTS` at startup; a real project needs its own migration strategy (e.g. DbUp, Flyway, or hand-rolled scripts)
- Multi-mapping (`Query<T1, T2, TReturn>`) is powerful but brittle: column order and `splitOn` must line up exactly with the SQL, and refactoring the query silently breaks the mapping if you're not careful
- SQLite's type affinity matters here in a way it doesn't with EF Core: a `DECIMAL`/`NUMERIC` column silently stores whole-number values as `INTEGER` and fractional ones as `REAL`, and Dapper's per-column deserializer throws if a later row's runtime type doesn't match the first row's — this demo uses `REAL` columns specifically to avoid that

**Relationship to other patterns:** Dapper is the classic choice for the query side of [CQRS](../../architecture/CQRS/README.md) — a hand-tuned read model doesn't need change tracking, only a fast, precise projection from storage to DTO. This demo doesn't implement CQRS itself, but the `top-products` report endpoint is exactly the shape of query that pattern's read side is built around.
