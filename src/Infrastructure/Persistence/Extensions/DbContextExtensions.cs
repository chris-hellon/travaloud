using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Travaloud.Application.Common.Events;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence.Context;

namespace Travaloud.Infrastructure.Persistence.Extensions;

public static class DbContextExtensions
{
    public static ApplicationDbContext CreateDbContext(
        this IDbContextFactory<ApplicationDbContext> dbContextFactory,
        IConfiguration configuration,
        ICurrentUser currentUser,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher eventPublisher)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        var rootConnectionString = databaseSettings?.ConnectionString;
        if (string.IsNullOrEmpty(rootConnectionString))
        {
            throw new InvalidOperationException("DB ConnectionString is not configured.");
        }

        var dbProvider = databaseSettings?.DBProvider;
        if (string.IsNullOrEmpty(dbProvider))
        {
            throw new InvalidOperationException("DB Provider is not configured.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(rootConnectionString,
            e => e.MigrationsAssembly("Migrators.SqlServer"));
        
        var tenantInfo = multiTenantContextAccessor.MultiTenantContext?.TenantInfo!;

        return new ApplicationDbContext(tenantInfo, optionsBuilder.Options, currentUser, serializer, dbSettings, eventPublisher, multiTenantContextAccessor);
    }

    // public static async Task<bool> CreateUserAsync(this ApplicationDbContext dbContext, ApplicationUser user, string password)
    // {
    //     
    // }
}