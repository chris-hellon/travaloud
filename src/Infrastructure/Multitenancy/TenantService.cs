using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Travaloud.Application.Common.Persistence;
using Travaloud.Application.Multitenancy;
using Travaloud.Infrastructure.Persistence;
using Travaloud.Infrastructure.Persistence.Initialization;

namespace Travaloud.Infrastructure.Multitenancy;

internal class TenantService : ITenantService
{
    private readonly IConnectionStringSecurer _csSecurer;
    private readonly IDatabaseInitializer _dbInitializer;
    private readonly IStringLocalizer<TenantService> _localizer;
    private readonly DatabaseSettings _dbSettings;
    private readonly IConfiguration _configuration;
    
    public TenantService(
        IConnectionStringSecurer csSecurer,
        IDatabaseInitializer dbInitializer,
        IStringLocalizer<TenantService> localizer,
        IOptions<DatabaseSettings> dbSettings, IConfiguration configuration)
    {
        _csSecurer = csSecurer;
        _dbInitializer = dbInitializer;
        _localizer = localizer;
        _configuration = configuration;
        _dbSettings = dbSettings.Value;
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        await using var dbContext = CreateTenantDbContext();
        
        var tenants = (await dbContext.TenantInfo.AsNoTracking().ToListAsync()).Adapt<List<TenantDto>>();
        tenants.ForEach(t => t.ConnectionString = _csSecurer.MakeSecure(t.ConnectionString));
        return tenants;
    }

    public async Task<bool> ExistsWithIdAsync(string id)
    {
        await using var dbContext = CreateTenantDbContext();

        return dbContext.TenantInfo?.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id) != null;
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        await using var dbContext = CreateTenantDbContext();

        return dbContext.TenantInfo.AsNoTracking().Any(x => x.Name == name);
    }
        

    public async Task<TenantDto> GetByIdAsync(string id)
    {
        await using var dbContext = CreateTenantDbContext();
        
        return (await GetTenantInfoAsync(dbContext, id))
            .Adapt<TenantDto>();
    }
    
    public async Task<TenantDto> GetByUrlAsync(string url)
    {
        await using var dbContext = CreateTenantDbContext();
        
        var tenants = await dbContext.TenantInfo.AsNoTracking().ToListAsync();
        var tenant = tenants.FirstOrDefault(x => x.Url == url);

        if (tenant != null)
            return tenant.Adapt<TenantDto>();

        throw new NotFoundException(string.Format(_localizer["entity.notfound"], typeof(TravaloudTenantInfo).Name, url));
    }

    public async Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        if(request.ConnectionString?.Trim() == _dbSettings.ConnectionString?.Trim()) request.ConnectionString = string.Empty;

        await using var dbContext = CreateTenantDbContext();
        
        var tenant = new TravaloudTenantInfo(request.Id, request.Name, request.ConnectionString, request.AdminEmail, request.Issuer, request.Url, request.WebsiteUrl, request.LogoImageUrl, request.PrimaryColor, request.PrimaryHoverColor, request.SecondaryColor, request.SecondaryHoverColor, request.TeritaryColor, request.TeritaryHoverColor, request.HeaderFontWoffUrl, request.HeaderFontWoff2Url, request.HeaderFont, request.BodyFontWoffUrl, request.BodyFontWoff2Url, request.BodyFont);

        await dbContext.TenantInfo.AddAsync(tenant, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        // TODO: run this in a hangfire job? will then have to send mail when it's ready or not
        try
        {
            await _dbInitializer.InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
            
        }
        catch
        { 
            dbContext.TenantInfo.Remove(tenant);
            
            await dbContext.SaveChangesAsync(cancellationToken);
            
            throw;
        }

        return tenant.Id;
    }

    public async Task<string> UpdateAsync(UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        if (request.ConnectionString?.Trim() == _dbSettings.ConnectionString?.Trim()) request.ConnectionString = string.Empty;
        await using var dbContext = CreateTenantDbContext();
        var tenant = await GetTenantInfoAsync(dbContext, request.Id);

        if (tenant == null)
        {
            throw new NotFoundException("Tenant does not exist");
        }

        tenant.Update(request.Name, request.ConnectionString, request.AdminEmail, request.Issuer, request.Url, request.WebsiteUrl, request.LogoImageUrl, request.PrimaryColor, request.PrimaryHoverColor, request.SecondaryColor, request.SecondaryHoverColor, request.TeritaryColor, request.TeritaryHoverColor, request.HeaderFontWoffUrl, request.HeaderFontWoff2Url, request.HeaderFont, request.BodyFontWoffUrl, request.BodyFontWoff2Url, request.BodyFont);
        dbContext.TenantInfo.Update(tenant);

        await dbContext.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }

    public async Task<string> ActivateAsync(string id)
    {
        await using var dbContext = CreateTenantDbContext();
        var tenant = await GetTenantInfoAsync(dbContext, id);

        if (tenant.IsActive)
        {
            throw new ConflictException("Tenant is already Activated.");
        }

        tenant.Activate();

        dbContext.TenantInfo.Update(tenant);

        await dbContext.SaveChangesAsync();

        return $"Tenant {id} is now Activated.";
    }

    public async Task<string> DeactivateAsync(string id)
    {
        await using var dbContext = CreateTenantDbContext();
        var tenant = await GetTenantInfoAsync(dbContext, id);

        if (!tenant.IsActive)
        {
            throw new ConflictException("Tenant is already Deactivated.");
        }

        tenant.Deactivate();

        dbContext.TenantInfo.Update(tenant);

        await dbContext.SaveChangesAsync();

        return $"Tenant {id} is now Deactivated.";
    }

    public async Task<string> UpdateSubscription(string id, DateTime extendedExpiryDate)
    {
        await using var dbContext = CreateTenantDbContext();
        var tenant = await GetTenantInfoAsync(dbContext, id);

        tenant.SetValidity(extendedExpiryDate);

        dbContext.TenantInfo.Update(tenant);

        await dbContext.SaveChangesAsync();
        
        return $"Tenant {id}'s Subscription Upgraded. Now Valid till {tenant.ValidUpto}.";
    }

    private async Task<TravaloudTenantInfo> GetTenantInfoAsync(TenantDbContext dbContext, string id)
    {
        return await dbContext.TenantInfo.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
               ?? throw new NotFoundException(string.Format(_localizer["entity.notfound"], typeof(TravaloudTenantInfo).Name, id));
    }

    private TenantDbContext CreateTenantDbContext()
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
        
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseSqlServer(rootConnectionString,
            e => e.MigrationsAssembly("Migrators.SqlServer"));
        
        return new TenantDbContext(optionsBuilder.Options);
    }
}