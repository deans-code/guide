using Microsoft.EntityFrameworkCore;

namespace MassTransitDemo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<ConsumerLog> ConsumerLogs => Set<ConsumerLog>();
}
