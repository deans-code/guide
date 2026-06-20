using Microsoft.EntityFrameworkCore;

namespace InboxDemo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<FulfillmentRecord> FulfillmentRecords => Set<FulfillmentRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InboxMessage>().HasKey(m => m.MessageId);
    }
}
