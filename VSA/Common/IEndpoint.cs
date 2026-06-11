namespace VSADemo.Common;

// Each feature slice implements this interface and registers its own route.
// Program.cs discovers all implementations via reflection — it never needs to
// know about individual features.
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
