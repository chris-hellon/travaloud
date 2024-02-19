using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Travaloud.Application.Common.Events;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.Persistence.Context;

public abstract class ApplicationDbFactoryContext<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class
{
    private readonly ICurrentUser _currentUser;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ISpecificationEvaluator _specificationEvaluator;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ISerializerService _serializer;
    private readonly IOptions<DatabaseSettings> _dbSettings;
    private readonly IEventPublisher _events;
    
    public ApplicationDbFactoryContext(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        IConfiguration configuration,
        ICurrentUser currentUser,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher events)
        : this(dbContextFactory, SpecificationEvaluator.Default, configuration, multiTenantContextAccessor, currentUser, serializer, dbSettings, events)
    {
    }

    public ApplicationDbFactoryContext(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        ISpecificationEvaluator specificationEvaluator,
        IConfiguration configuration,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        ICurrentUser currentUser,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher events)
    {
        _dbContextFactory = dbContextFactory;
        _specificationEvaluator = specificationEvaluator;
        _configuration = configuration;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _currentUser = currentUser;
        _serializer = serializer;
        _dbSettings = dbSettings;
        _events = events;
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull
    {
        await using var dbContext = CreateDbContext();
        return await dbContext.Set<TEntity>().FindAsync(new object[] {id}, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetBySpecAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await ApplySpecification(specification, dbContext).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResult?> GetBySpecAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await ApplySpecification(specification, dbContext).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await ApplySpecification(specification, dbContext).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await ApplySpecification(specification, dbContext).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TEntity?> SingleOrDefaultAsync(ISingleResultSpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await ApplySpecification(specification, dbContext).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResult?> SingleOrDefaultAsync<TResult>(
        ISingleResultSpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await ApplySpecification(specification, dbContext).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await dbContext.Set<TEntity>().ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext =  CreateDbContext();;
        var queryResult = await ApplySpecification(specification, dbContext).ToListAsync(cancellationToken);

        return specification.PostProcessingAction == null
            ? queryResult
            : specification.PostProcessingAction(queryResult).ToList();
    }

    /// <inheritdoc/>
    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext =  CreateDbContext();;
        var queryResult = await ApplySpecification(specification, dbContext).ToListAsync(cancellationToken);

        return specification.PostProcessingAction == null
            ? queryResult
            : specification.PostProcessingAction(queryResult).ToList();
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await ApplySpecification(specification, dbContext, true).CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await dbContext.Set<TEntity>().CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AnyAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await ApplySpecification(specification, dbContext, true).AnyAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        return await dbContext.Set<TEntity>().AnyAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<TEntity> AsAsyncEnumerable(ISpecification<TEntity> specification)
    {
        using var dbContext = CreateDbContext();
        return ApplySpecification(specification, dbContext).AsAsyncEnumerable();
    }
    
    /// <inheritdoc/>
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<TEntity>().Add(entity);

        await SaveChangesAsync(dbContext, cancellationToken);

        return entity;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<TEntity>().AddRange(entities);

        await SaveChangesAsync(dbContext, cancellationToken);

        return entities;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<TEntity>().Update(entity);
        dbContext.TenantNotSetMode = TenantNotSetMode.Overwrite;
        
        await SaveChangesAsync(dbContext, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<TEntity>().UpdateRange(entities);
        dbContext.TenantNotSetMode = TenantNotSetMode.Overwrite;
        
        await SaveChangesAsync(dbContext, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<TEntity>().Remove(entity);
        dbContext.TenantNotSetMode = TenantNotSetMode.Overwrite;
        
        await SaveChangesAsync(dbContext, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        dbContext.Set<TEntity>().RemoveRange(entities);
        dbContext.TenantNotSetMode = TenantNotSetMode.Overwrite;
        
        await SaveChangesAsync(dbContext, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteRangeAsync(ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = CreateDbContext();
        var query = ApplySpecification(specification, dbContext);
        dbContext.Set<TEntity>().RemoveRange(query);
        dbContext.TenantNotSetMode = TenantNotSetMode.Overwrite;
        
        await SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException();
    }

    public async Task<int> SaveChangesAsync(ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Filters the entities  of <typeparamref name="TEntity"/>, to those that match the encapsulated query logic of the
    /// <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification,
        ApplicationDbContext dbContext, bool evaluateCriteriaOnly = false)
    {
        return _specificationEvaluator.GetQuery(dbContext.Set<TEntity>().AsQueryable(), specification,
            evaluateCriteriaOnly);
    }

    /// <summary>
    /// Filters all entities of <typeparamref name="TEntity" />, that matches the encapsulated query logic of the
    /// <paramref name="specification"/>, from the database.
    /// <para>
    /// Projects each entity into a new form, being <typeparamref name="TResult" />.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered projected entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(ISpecification<TEntity, TResult> specification,
        ApplicationDbContext dbContext)
    {
        return _specificationEvaluator.GetQuery(dbContext.Set<TEntity>().AsQueryable(), specification);
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