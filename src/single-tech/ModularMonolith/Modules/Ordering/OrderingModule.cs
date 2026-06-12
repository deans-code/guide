using Common;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Data;
using Ordering.Features;

namespace Ordering;

public class OrderingModule : IModule
{
    public string Name => "Ordering";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderingDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("OrderingDb") ?? "Data Source=ordering.db"));
    }

    public void EnsureDatabaseCreated(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        scope.ServiceProvider.GetRequiredService<OrderingDbContext>().Database.EnsureCreated();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        PlaceOrder.MapEndpoint(app);
        GetOrder.MapEndpoint(app);
        ListOrders.MapEndpoint(app);
    }
}
