namespace PollyDemo.Services;

public interface IUnreliableService
{
    ValueTask<string> CallAsync(double failureRate = 0.7, CancellationToken ct = default);
    ValueTask<string> SlowCallAsync(int delayMs = 3000, CancellationToken ct = default);
    int CallCount { get; }
    void Reset();
}

public class UnreliableService : IUnreliableService
{
    private int _callCount;
    public int CallCount => _callCount;

    public async ValueTask<string> CallAsync(double failureRate = 0.7, CancellationToken ct = default)
    {
        var attempt = Interlocked.Increment(ref _callCount);
        await Task.Delay(50, ct);

        if (Random.Shared.NextDouble() < failureRate)
            throw new HttpRequestException($"Service failure on call #{attempt}");

        return $"Success on call #{attempt}";
    }

    public async ValueTask<string> SlowCallAsync(int delayMs = 3000, CancellationToken ct = default)
    {
        var attempt = Interlocked.Increment(ref _callCount);
        await Task.Delay(delayMs, ct);
        return $"Slow success on call #{attempt} after {delayMs}ms";
    }

    public void Reset() => Interlocked.Exchange(ref _callCount, 0);
}
