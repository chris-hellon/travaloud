using Finbuckle.MultiTenant;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Travaloud.Application.Auditing;
using Travaloud.Application.Common.Events;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence;
using Travaloud.Infrastructure.Persistence.Context;

namespace Travaloud.Infrastructure.Auditing;

public class AuditService : IAuditService
{
    private readonly ITenantInfo _currentTenant;
    private readonly ICurrentUser _currentUser;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ISerializerService _serializer;
    private readonly IOptions<DatabaseSettings> _dbSettings;
    private readonly IEventPublisher _events;
    
    public AuditService(
        IEventPublisher events,
        ITenantInfo currentTenant,
        ICurrentUser currentUser,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        IConfiguration configuration,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings)
    {
        _events = events;
        _currentTenant = currentTenant;
        _currentUser = currentUser;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _configuration = configuration;
        _serializer = serializer;
        _dbSettings = dbSettings;
    }

    public async Task<List<AuditDto>> GetUserTrailsAsync(DefaultIdType userId)
    {
        await using var dbContext = CreateDbContext();
        var trails = await dbContext.AuditTrails
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.DateTime)
            .Take(250)
            .ToListAsync();

        return trails.Adapt<List<AuditDto>>();
    }
    
    private ApplicationDbContext CreateDbContext()
    {
        var databaseSettings = _configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
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
        
        var tenantInfo = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo!;

        return new ApplicationDbContext(tenantInfo, optionsBuilder.Options, _currentUser, _serializer, _dbSettings, _events,
            _multiTenantContextAccessor)
        {
            TenantNotSetMode = TenantNotSetMode.Overwrite
        };
    }
}