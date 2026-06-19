namespace EventSourcingDemo.Accounts;

public class BankAccount
{
    public Guid Id { get; private set; }
    public string Owner { get; private set; } = "";
    public decimal Balance { get; private set; }

    private readonly List<object> _pendingEvents = [];
    public IReadOnlyList<object> PendingEvents => _pendingEvents;

    private BankAccount() { }

    public static BankAccount Open(string owner, decimal initialDeposit)
    {
        if (initialDeposit < 0) throw new ArgumentException("Initial deposit cannot be negative.");
        var account = new BankAccount();
        account.Raise(new AccountOpened(Guid.NewGuid(), owner, initialDeposit, DateTime.UtcNow));
        return account;
    }

    public void Deposit(decimal amount, string description)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        Raise(new MoneyDeposited(Id, amount, description, DateTime.UtcNow));
    }

    public void Withdraw(decimal amount, string description)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        if (amount > Balance) throw new InvalidOperationException($"Insufficient funds. Balance: {Balance:C}");
        Raise(new MoneyWithdrawn(Id, amount, description, DateTime.UtcNow));
    }

    // Rebuilds state by replaying persisted events — no current-value columns needed.
    public static BankAccount Rehydrate(IEnumerable<object> events)
    {
        var account = new BankAccount();
        foreach (var e in events) account.Apply(e);
        return account;
    }

    // New events go through Raise so they are both applied and queued for persistence.
    private void Raise(object @event)
    {
        Apply(@event);
        _pendingEvents.Add(@event);
    }

    private void Apply(object @event)
    {
        switch (@event)
        {
            case AccountOpened e:
                Id      = e.AccountId;
                Owner   = e.Owner;
                Balance = e.InitialDeposit;
                break;
            case MoneyDeposited e:
                Balance += e.Amount;
                break;
            case MoneyWithdrawn e:
                Balance -= e.Amount;
                break;
        }
    }
}
