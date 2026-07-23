namespace MassTransitDemo.Services;

// Controls how many times UpdateInventoryConsumer fails before succeeding,
// so the built-in retry middleware (configured in Program.cs) has something
// to actually retry. Singleton so the setting persists across requests.
public class FlakyInventoryService
{
    public int FailuresBeforeSuccess { get; set; } = 2;
}
