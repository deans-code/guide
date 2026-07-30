using Microsoft.EntityFrameworkCore;

namespace HangfireDemo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
    public DbSet<DigestRun> DigestRuns => Set<DigestRun>();
}
