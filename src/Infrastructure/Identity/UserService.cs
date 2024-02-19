using Ardalis.Specification.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Travaloud.Application.Common.Caching;
using Travaloud.Application.Common.Events;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Common.Models;
using Travaloud.Application.Common.Specification;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Identity;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence;
using Travaloud.Infrastructure.Persistence.Context;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.Identity;

internal partial class UserService : IUserService
{
    private readonly IStringLocalizer<UserService> _localizer;
    // private readonly IMailService _mailService;
    // private readonly MailSettings _mailSettings;
    // private readonly SecuritySettings _securitySettings;
    // private readonly IEmailTemplateService _templateService;
    private readonly IFileStorageService _fileStorage;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;
    private readonly ITenantInfo _currentTenant;
    private readonly ICurrentUser _currentUser;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ISerializerService _serializer;
    private readonly IOptions<DatabaseSettings> _dbSettings;
    private readonly IEventPublisher _events;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    
    public UserService(
        IStringLocalizer<UserService> localizer,
        // IMailService mailService,
        // IOptions<MailSettings> mailSettings,
        // IEmailTemplateService templateService,
        IFileStorageService fileStorage,
        IEventPublisher events,
        ICacheService cache,
        ICacheKeyService cacheKeys,
        ITenantInfo currentTenant,
        // IOptions<SecuritySettings> securitySettings,
        ICurrentUser currentUser,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        IConfiguration configuration,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings, IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _localizer = localizer;
        // _mailService = mailService;
        // _mailSettings = mailSettings.Value;
        // _templateService = templateService;
        _fileStorage = fileStorage;
        _events = events;
        _cache = cache;
        _cacheKeys = cacheKeys;
        _currentTenant = currentTenant;
        _currentUser = currentUser;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _configuration = configuration;
        _serializer = serializer;
        _dbSettings = dbSettings;
        _passwordHasher = passwordHasher;
        // _securitySettings = securitySettings.Value;
    }

    public async Task<PaginationResponse<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken)
    {
        await using var dbContext = CreateDbContext();
        var spec = new EntitiesByPaginationFilterSpec<ApplicationUser>(filter);

        var users = await dbContext.Users
            .AsNoTracking()
            .WithSpecification(spec)
            .ProjectToType<UserDetailsDto>()
            .ToListAsync(cancellationToken);
        var count = await dbContext.Users
            .CountAsync(cancellationToken);

        return new PaginationResponse<UserDetailsDto>(users, count, filter.PageNumber, filter.PageSize);
    }

    public async Task<bool> ExistsWithNameAsync(string name)
    {
        EnsureValidTenant();
        
        await using var dbContext = CreateDbContext();
        return await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == name) != null;
    }

    public async Task<bool> ExistsWithEmailAsync(string email, string? exceptId = null)
    {
        EnsureValidTenant();
        
        await using var dbContext = CreateDbContext();

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email.Normalize());
        return !string.IsNullOrEmpty(exceptId) && user != null && user.Id != exceptId || string.IsNullOrEmpty(exceptId) && user != null;
    }

    public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
    {
        EnsureValidTenant();
        
        await using var dbContext = CreateDbContext();
        
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        return !string.IsNullOrEmpty(exceptId) && user != null && user.Id != exceptId || string.IsNullOrEmpty(exceptId) && user != null;
    }

    private void EnsureValidTenant()
    {
        if (string.IsNullOrWhiteSpace(_currentTenant?.Id))
        {
            throw new UnauthorizedException(_localizer["tenant.invalid"]);
        }
    }

    public async Task<List<UserDetailsDto>> GetListAsync(CancellationToken cancellationToken, string? role = null)
    {
        await using var dbContext = CreateDbContext();
        
        var usersInRole = await GetUsersInRole(dbContext, role);
        return usersInRole.Adapt<List<UserDetailsDto>>().OrderBy(x => x.FirstName).ToList();
    }

    public async Task<List<UserDetailsDto>> GetListAsync(string? role = null)
    {
        await using var dbContext = CreateDbContext();
        
        var usersInRole = await GetUsersInRole(dbContext, role);
        return usersInRole.Adapt<List<UserDetailsDto>>().OrderBy(x => x.FirstName).ToList();
    }
    
    public async Task<int> GetCountAsync(CancellationToken cancellationToken, string? role = null)
    {
        await using var dbContext = CreateDbContext();
        
        if (role == null) return await dbContext.Users.AsNoTracking().CountAsync(cancellationToken);
        var users =  await GetUsersInRole(dbContext, role);
        
        return users.Count;
    }

    public async Task<UserDetailsDto> GetAsync(string userId, CancellationToken cancellationToken)
    {
        await using var dbContext = CreateDbContext();
        
        var user = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        return user.Adapt<UserDetailsDto>();
    }
    
    public async Task<UserDetailsDto> GetAsync(string userId)
    {
        await using var dbContext = CreateDbContext();
        
        var user = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        return user.Adapt<UserDetailsDto>();
    }

    public async Task ToggleStatusAsync(ToggleUserStatusRequest request)
    {
        await using var dbContext = CreateDbContext();
        
        var user = await dbContext.Users.AsNoTracking().Where(u => u.Id == request.UserId).FirstOrDefaultAsync();

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var isAdmin = await IsUserInRole(dbContext, user, TravaloudRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException(_localizer["Administrators Profile's Status cannot be toggled"]);
        }

        user.IsActive = request.ActivateUser;
        dbContext.Users.Update(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }
    
    public async Task ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        await using var dbContext = CreateDbContext();
        
        var user = await dbContext.Users.AsNoTracking().Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var isAdmin = await IsUserInRole(dbContext, user, TravaloudRoles.Admin);
        if (isAdmin)
        {
            throw new ConflictException(_localizer["Administrators Profile's Status cannot be toggled"]);
        }

        user.IsActive = request.ActivateUser;
        dbContext.Users.Update(user);

        await _events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
    }

    private static async Task<bool> IsUserInRole(ApplicationDbContext dbContext, ApplicationUser user, string roleName)
    {
        var role = await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == roleName);
        var userInRole = false;

        if (role == null) return userInRole;
        {
            var userRoles = await dbContext.UserRoles.AsNoTracking().FirstOrDefaultAsync(x => x.RoleId == role.Id && x.UserId == user.Id);
            userInRole = userRoles != null;
        }
        
        return userInRole;
    }
    
    private static async Task<List<ApplicationUser>> GetUsersInRole(ApplicationDbContext dbContext, string? roleName = null)
    {
        if (roleName != null)
        {
            var role = await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == roleName);
            var userRoles = await dbContext.UserRoles.AsNoTracking().Where(x => x.RoleId == role!.Id).ToListAsync();
            var users = await dbContext.Users.AsNoTracking().ToListAsync();

            return users.Where(x => userRoles.Any(ur => ur.UserId == x.Id)).ToList();
        }
        else
        {
            // Get all users that aren't guests
            var roles = await dbContext.Roles.AsNoTracking().Where(x => x.Name != TravaloudRoles.Guest).ToListAsync();
            var roleIds = roles.Select(x => x.Id);
            var userRoles = await dbContext.UserRoles.AsNoTracking().Where(x => roleIds.Contains(x.RoleId)).ToListAsync();
            var users = await dbContext.Users.AsNoTracking().ToListAsync();
            
            return users.Where(x => userRoles.Any(ur => ur.UserId == x.Id)).ToList();
        }
    }

    private static async Task<ApplicationUser> GetUserById(ApplicationDbContext dbContext, string userId)
    {
        return await dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == userId);
    }

    private static async Task<IEnumerable<IdentityUserRole<string>>> GetUserRoles(ApplicationDbContext dbContext, string userId)
    {
        return await dbContext.UserRoles.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
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