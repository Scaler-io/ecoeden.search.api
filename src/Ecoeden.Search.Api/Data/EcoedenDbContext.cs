using Ecoeden.Search.Api.Entities.Sql;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ecoeden.Search.Api.Data;

public class EcoedenDbContext(DbContextOptions<EcoedenDbContext> options) : DbContext(options)
{
    public DbSet<EventPublishHistory> EventPublishHistories { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach(var entry in ChangeTracker.Entries<EventPublishHistory>())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.State = EntityState.Modified; 
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasDefaultSchema("ecoeden.event");
        base.OnModelCreating(modelBuilder);
    }
}