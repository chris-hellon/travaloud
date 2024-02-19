using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;
using Travaloud.Infrastructure.Persistence.Configuration;

namespace Travaloud.Infrastructure.Multitenancy;

public class TenantDbContext : EFCoreStoreDbContext<TravaloudTenantInfo>
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TravaloudTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
    }
}