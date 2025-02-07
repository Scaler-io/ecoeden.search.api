using Ecoeden.Search.Api.Entities.Sql;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ecoeden.Search.Api.Data;

public class EcoedenStockDbContext(DbContextOptions<EcoedenStockDbContext> options) : DbContext(options)
{
    public DbSet<EventPublishHistory> EventPublishHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasDefaultSchema("ecoeden.event");
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<SqlBaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdateAt = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
