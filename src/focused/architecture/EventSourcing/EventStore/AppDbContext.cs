using Microsoft.EntityFrameworkCore;

namespace EventSourcingDemo.EventStore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<StoredEvent> Events => Set<StoredEvent>();
}
