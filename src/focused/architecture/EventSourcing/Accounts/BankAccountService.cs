using EventSourcingDemo.EventStore;

namespace EventSourcingDemo.Accounts;

public class BankAccountService(EventStoreRepository store)
{
    private static string StreamId(Guid id) => $"account-{id}";

    public async Task<BankAccount> OpenAsync(string owner, decimal initialDeposit, CancellationToken ct = default)
    {
        var account = BankAccount.Open(owner, initialDeposit);
        await store.AppendAsync(StreamId(account.Id), account.PendingEvents, expectedVersion: 0, ct);
        return account;
    }

    public async Task<BankAccount> DepositAsync(Guid accountId, decimal amount, string description, CancellationToken ct = default)
    {
        var (account, version) = await LoadAsync(accountId, ct);
        account.Deposit(amount, description);
        await store.AppendAsync(StreamId(accountId), account.PendingEvents, expectedVersion: version, ct);
        return account;
    }

    public async Task<BankAccount> WithdrawAsync(Guid accountId, decimal amount, string description, CancellationToken ct = default)
    {
        var (account, version) = await LoadAsync(accountId, ct);
        account.Withdraw(amount, description);
        await store.AppendAsync(StreamId(accountId), account.PendingEvents, expectedVersion: version, ct);
        return account;
    }

    public async Task<BankAccount?> GetAsync(Guid accountId, CancellationToken ct = default)
    {
        var events = await store.LoadEventsAsync(StreamId(accountId), ct);
        return events.Count == 0 ? null : BankAccount.Rehydrate(events);
    }

    private async Task<(BankAccount Account, int Version)> LoadAsync(Guid accountId, CancellationToken ct)
    {
        var events = await store.LoadEventsAsync(StreamId(accountId), ct);
        if (events.Count == 0) throw new KeyNotFoundException($"Account {accountId} not found.");
        return (BankAccount.Rehydrate(events), events.Count);
    }
}
