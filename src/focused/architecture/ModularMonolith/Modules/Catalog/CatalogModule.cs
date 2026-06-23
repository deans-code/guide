using Catalog.Contracts;
using Catalog.Data;
using Catalog.Features;
using Common;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog;

// The only public type in this assembly besides Contracts/*. The host
// references CatalogModule to wire it up, and other modules depend on
// IProductCatalog to talk to it.
public class CatalogModule : IModule
{
    public string Name => "Catalog";

    public void AddModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("CatalogDb") ?? "Data Source=catalog.db"));

        services.AddScoped<IProductCatalog, ProductCatalogService>();
    }

    public void EnsureDatabaseCreated(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        scope.ServiceProvider.GetRequiredService<CatalogDbContext>().Database.EnsureCreated();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        CreateProduct.MapEndpoint(app);
        ListProducts.MapEndpoint(app);
        GetProduct.MapEndpoint(app);
    }
}
