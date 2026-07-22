# Dapr

**Primary category:** Distributed
**Also relevant to:** Data, Resilience

Demonstrates two of Dapr's core building blocks — state management and publish/subscribe — through an ASP.NET Core Minimal API. Placing an order saves it to a Dapr state store and publishes an `order-placed` event; a co-located subscriber handler receives the event through the sidecar and records a notification, all without the application ever talking to Redis directly.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, Dapr CLI/runtime, Dapr.AspNetCore 1.18, Redis (state store + pub/sub broker, provisioned by `dapr init`)

## Run

Prereqs: [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/) installed. The first time, initialise it with `dapr init` — this pulls and starts the `dapr_redis`, `dapr_placement`, `dapr_scheduler`, and `dapr_zipkin` containers that the state store and pub/sub components below point at. If those containers already exist but are stopped (e.g. `docker ps -a` shows them as `Exited`), start them again with `docker start dapr_redis dapr_placement dapr_scheduler dapr_zipkin` instead of re-running `dapr init`.

```bash
cd src/focused/distributed/Dapr
dapr run --app-id dapr-demo --app-port 5210 --dapr-http-port 3500 --resources-path ./components -- dotnet run
```

- API: `http://localhost:5210`
- Swagger UI: `http://localhost:5210/scalar/v1`
- Dapr sidecar: `http://localhost:3500/v1.0/...`

## Use cases

**What it solves:** Talking to infrastructure — state stores, message brokers, secret stores — through vendor-specific SDKs couples application code to a specific technology and forces every service to reimplement retries, serialization, and connection handling itself. Dapr moves that concern into a sidecar process and exposes it as a stable HTTP/gRPC API, so the application only ever depends on Dapr's building block contracts.

**When to reach for it:**
- A polyglot or multi-team environment where every service would otherwise pick its own broker/state-store client
- You want to swap Redis for CosmosDB, or RabbitMQ for Kafka, via configuration rather than a code change
- You need built-in retries, mTLS, and observability across service calls without hand-rolling each one

**How the sidecar changes the picture:**

Every Dapr-enabled process runs with a sidecar alongside it. The application calls `localhost:<dapr-http-port>` (or the SDK, which wraps the same calls); the sidecar is the only thing that talks to Redis, the broker, or any other backing service. Swapping `components/statestore.yaml` from `state.redis` to `state.postgresql` requires no application code changes — `DaprClient.SaveStateAsync` has no idea what is behind it.

**Trade-offs to be aware of:**
- Every Dapr-enabled service needs a sidecar — extra operational surface, though it deploys automatically alongside the app as a container (Kubernetes) or process (Dapr CLI)
- An extra network hop (app → sidecar → backing service) adds latency compared to a direct SDK call
- Building block APIs are least-common-denominator — an advanced feature of a specific broker or store may not be exposed through the abstraction

**Relationship to other patterns:** Dapr's pub/sub building block solves the same "reliably notify other services" problem as the [Outbox Pattern](../Outbox/README.md), but delegates the delivery guarantee to the sidecar and broker instead of a hand-rolled polling loop. Its declarative retry policies (`components/resiliency.yaml` in this demo) cover the same need as [Polly](../../resilience/Polly/README.md), configured outside application code instead of inside it.
