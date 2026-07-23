# MassTransit

**Primary category:** Distributed
**Also relevant to:** Resilience

Demonstrates MassTransit's publish/subscribe messaging model and built-in retry middleware over its in-memory transport. Placing an order publishes a single `OrderPlaced` event that two independent consumers — `SendConfirmationEmailConsumer` and `UpdateInventoryConsumer` — each receive and process on their own. The inventory consumer is deliberately flaky; MassTransit's declarative `UseMessageRetry` policy redelivers the message automatically, with no hand-rolled attempt-counting in application code.

**Tech:** .NET 10, ASP.NET Core Minimal APIs, MassTransit 8.5, EF Core 10, SQLite

## Run

```bash
cd src/focused/distributed/MassTransit
dotnet run
```

- API: `http://localhost:5211`
- Swagger UI: `http://localhost:5211/scalar/v1`

## Use cases

**What it solves:** Talking to a message broker directly means every service hand-rolls its own connection handling, topology setup, retry logic, and consumer dispatch. MassTransit provides a typed, testable abstraction over that broker — `IPublishEndpoint`/`IConsumer<T>` and declarative middleware — so application code deals in messages and consumers, not queues and channels.

**When to reach for it:**
- Multiple independent parts of the system need to react to the same event (fan-out) without the publisher knowing who's listening
- You want production-grade retry, redelivery, and fault handling without writing attempt-counting and backoff logic by hand
- You expect to run against a real broker (RabbitMQ, Azure Service Bus, Amazon SQS) in production but want to develop and test against an in-memory transport with the same code

**Publish/subscribe vs point-to-point:**

| Style | How it works | This demo |
|---|---|---|
| Publish/subscribe | One event, any number of subscribers, each independent | `OrderPlaced` → `SendConfirmationEmailConsumer` + `UpdateInventoryConsumer` |
| Point-to-point (send) | One command, exactly one consumer, routed to a specific queue | Not shown here — use `ISendEndpoint`/`IRequestClient<T>` for commands and request/response |

**Trade-offs to be aware of:**
- The in-memory transport used here has no persistence — messages in flight are lost on process restart; production deployments need a real broker
- Retry middleware masks transient failures but still delays the eventual outcome — a consumer that always fails exhausts its retries and faults, it doesn't succeed
- Consumers run concurrently and independently; ordering across different consumer types is not guaranteed, only within a single consumer's queue

**Relationship to other patterns:** The [Outbox Pattern](../Outbox/README.md) and [Inbox Pattern](../Inbox/README.md) demos hand-roll reliable publish and exactly-once consumption on top of a simulated broker. MassTransit's own EF Core outbox integration and built-in retry/redelivery provide the same guarantees out of the box — worth reaching for once the hand-rolled version's maintenance cost outweighs taking on the library. The [Saga Pattern](../Saga/README.md) demo orchestrates steps in-process; MassTransit's saga state machine (`MassTransitStateMachine<T>`) is the natural next step for coordinating the same workflow over real messages.
