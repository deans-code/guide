namespace EventSourcingDemo.Accounts;

public record AccountOpened(Guid AccountId, string Owner, decimal InitialDeposit, DateTime OccurredAt);
public record MoneyDeposited(Guid AccountId, decimal Amount, string Description, DateTime OccurredAt);
public record MoneyWithdrawn(Guid AccountId, decimal Amount, string Description, DateTime OccurredAt);
