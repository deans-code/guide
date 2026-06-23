using System.Text.Json;
using EventSourcingDemo.Accounts;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingDemo.EventStore;

public class EventStoreRepository(AppDbContext db)
{
    private static readonly Dictionary<string, Type> KnownTypes = new()
    {
        [nameof(AccountOpened)]   = typeof(AccountOpened),
        [nameof(MoneyDeposited)]  = typeof(MoneyDeposited),
        [nameof(MoneyWithdrawn)]  = typeof(MoneyWithdrawn)
    };

    public async Task AppendAsync(string streamId, IEnumerable<object> events, int expectedVersion, CancellationToken ct = default)
    {
        var currentVersion = await db.Events.CountAsync(e => e.StreamId == streamId, ct);

        if (currentVersion != expectedVersion)
            throw new InvalidOperationException(
                $"Concurrency conflict on '{streamId}': expected version {expectedVersion}, found {currentVersion}.");

        var version = currentVersion;
        foreach (var @event in events)
        {
            db.Events.Add(new StoredEvent
            {
                StreamId  = streamId,
                EventType = @event.GetType().Name,
                Payload   = JsonSerializer.Serialize(@event, @event.GetType()),
                Version   = ++version
            });
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<object>> LoadEventsAsync(string streamId, CancellationToken ct = default)
    {
        var stored = await db.Events
            .Where(e => e.StreamId == streamId)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        return stored
            .Select(e => JsonSerializer.Deserialize(e.Payload, KnownTypes[e.EventType])!)
            .ToList();
    }

    public async Task<IReadOnlyList<StoredEvent>> GetRawAsync(string streamId, CancellationToken ct = default) =>
        await db.Events
            .Where(e => e.StreamId == streamId)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);
}
