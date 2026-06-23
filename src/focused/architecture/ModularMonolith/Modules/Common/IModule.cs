using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

// Each module implements this and is registered once in Api/Program.cs.
// The host only ever talks to a module through this interface — it never
// references a module's internal types (DbContexts, entities, handlers).
public interface IModule
{
    string Name { get; }

    void AddModule(IServiceCollection services, IConfiguration configuration);

    void EnsureDatabaseCreated(IServiceProvider services);

    void MapEndpoints(IEndpointRouteBuilder app);
}
