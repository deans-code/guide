using EventSourcingDemo.Accounts;
using EventSourcingDemo.EventStore;

namespace EventSourcingDemo.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapPost("/accounts", async (OpenAccountRequest req, BankAccountService svc, CancellationToken ct) =>
        {
            var account = await svc.OpenAsync(req.Owner, req.InitialDeposit, ct);
            return Results.Ok(ToResponse(account));
        })
        .WithName("OpenAccount")
        .WithSummary("Open a new bank account — appends an AccountOpened event");

        app.MapPost("/accounts/{id:guid}/deposits", async (Guid id, TransactionRequest req, BankAccountService svc, CancellationToken ct) =>
        {
            var account = await svc.DepositAsync(id, req.Amount, req.Description, ct);
            return Results.Ok(ToResponse(account));
        })
        .WithName("Deposit")
        .WithSummary("Deposit money — appends a MoneyDeposited event; balance is derived by replaying all events");

        app.MapPost("/accounts/{id:guid}/withdrawals", async (Guid id, TransactionRequest req, BankAccountService svc, CancellationToken ct) =>
        {
            var account = await svc.WithdrawAsync(id, req.Amount, req.Description, ct);
            return Results.Ok(ToResponse(account));
        })
        .WithName("Withdraw")
        .WithSummary("Withdraw money — appends a MoneyWithdrawn event; overdraft checked against replayed state");

        app.MapGet("/accounts/{id:guid}", async (Guid id, BankAccountService svc, CancellationToken ct) =>
        {
            var account = await svc.GetAsync(id, ct);
            return account is null ? Results.NotFound() : Results.Ok(ToResponse(account));
        })
        .WithName("GetAccount")
        .WithSummary("Get current account state, rebuilt by replaying every event in the stream");

        app.MapGet("/accounts/{id:guid}/events", async (Guid id, EventStoreRepository store, CancellationToken ct) =>
        {
            var events = await store.GetRawAsync($"account-{id}", ct);
            return events.Count == 0 ? Results.NotFound() : Results.Ok(events);
        })
        .WithName("GetAccountEvents")
        .WithSummary("Get the raw event stream — the append-only source of truth");
    }

    private static object ToResponse(BankAccount a) => new { a.Id, a.Owner, a.Balance };
}

public record OpenAccountRequest(string Owner, decimal InitialDeposit);
public record TransactionRequest(decimal Amount, string Description);
