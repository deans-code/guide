using Microsoft.EntityFrameworkCore;

namespace SagaDemo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SagaInstance> Sagas => Set<SagaInstance>();
    public DbSet<SagaStepRecord> Steps => Set<SagaStepRecord>();
}
