using System.Data;
using Dapper;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Travaloud.Application.Common.Events;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Common.Persistence;
using Travaloud.Domain.Common.Contracts;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence.Context;
using Travaloud.Infrastructure.Persistence.Extensions;

namespace Travaloud.Infrastructure.Persistence.Repository;

public class DapperRepository : IDapperRepository
{
    private readonly ICurrentUser _currentUser;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ISerializerService _serializer;
    private readonly IOptions<DatabaseSettings> _dbSettings;
    private readonly IEventPublisher _events;

    public DapperRepository(IConfiguration configuration,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        ICurrentUser currentUser,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher events)
    {
        _configuration = configuration;
        _multiTenantContextAccessor = multiTenantContextAccessor;
        _currentUser = currentUser;
        _serializer = serializer;
        _dbSettings = dbSettings;
        _events = events;
    }

    public async Task ExecuteAsync(string sql, object? param = null,
        IDbTransaction? transaction = null, CommandType? commandType = null,
        CancellationToken cancellationToken = default) =>
        await CreateDbContext().Connection.ExecuteAsync(sql, param, transaction, commandType: commandType);
    
    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null,
        IDbTransaction? transaction = null, CommandType? commandType = null,
        CancellationToken cancellationToken = default)
        where T : class =>
        (await CreateDbContext().Connection.QueryAsync<T>(sql, param, transaction, commandType: commandType))
        .AsList();

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null,
        IDbTransaction? transaction = null, CommandType? commandType = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        if (CreateDbContext().Model.GetMultiTenantEntityTypes().All(t => t.ClrType != typeof(T)))
        {
            sql = sql.Replace("@tenant", CreateDbContext().TenantInfo.Id);
        }

        var entity =
            await CreateDbContext().Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandType: commandType);

        return entity ?? throw new NotFoundException(string.Empty);
    }

    public Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        CommandType? commandType = null, CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        if (CreateDbContext().Model.GetMultiTenantEntityTypes().All(t => t.ClrType != typeof(T)))
        {
            sql = sql.Replace("@tenant", CreateDbContext().TenantInfo.Id);
        }

        return CreateDbContext().Connection.QuerySingleAsync<T>(sql, param, transaction, commandType: commandType);
    }

    public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param = null,
        IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        if (sql.Contains("@tenant"))
        {
            sql = sql.Replace("@tenant", CreateDbContext().TenantInfo.Id);
        }

        return CreateDbContext().Connection.QueryMultipleAsync(sql, param, transaction);
    }

    public async
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>,
            IEnumerable<T7>, Tuple<IEnumerable<T8>, IEnumerable<T9>>>>
        GetMultipleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string query, object request,
            Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2,
            Func<SqlMapper.GridReader, IEnumerable<T3>> func3, Func<SqlMapper.GridReader, IEnumerable<T4>> func4,
            Func<SqlMapper.GridReader, IEnumerable<T5>> func5, Func<SqlMapper.GridReader, IEnumerable<T6>> func6,
            Func<SqlMapper.GridReader, IEnumerable<T7>> func7, Func<SqlMapper.GridReader, IEnumerable<T8>> func8,
            Func<SqlMapper.GridReader, IEnumerable<T9>> func9, CommandType commandType = CommandType.StoredProcedure)
    {
        var objs = await CreateDbContext().Connection.GetMultipleAsync(query, request, commandType, func1, func2, func3, func4,
            func5, func6, func7, func8, func9);

        return Common.Extensions.TupleExtensions.CreateIEnumerableTuple(
            (IEnumerable<T1>) objs[0],
            (IEnumerable<T2>) objs[1],
            (IEnumerable<T3>) objs[2],
            (IEnumerable<T4>) objs[3],
            (IEnumerable<T5>) objs[4],
            (IEnumerable<T6>) objs[5],
            (IEnumerable<T7>) objs[6],
            (IEnumerable<T8>) objs[7],
            (IEnumerable<T9>) objs[8]);
    }
    
    public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>> GetMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(string query, object request, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3, Func<SqlMapper.GridReader, IEnumerable<T4>> func4, Func<SqlMapper.GridReader, IEnumerable<T5>> func5, Func<SqlMapper.GridReader, IEnumerable<T6>> func6, Func<SqlMapper.GridReader, IEnumerable<T7>> func7, CommandType commandType = CommandType.StoredProcedure)
    {
        var objs = await CreateDbContext().Connection.GetMultipleAsync(query, request, commandType, func1, func2, func3, func4, func5, func6, func7);

        return Tuple.Create(
            (IEnumerable<T1>) objs[0],
            (IEnumerable<T2>) objs[1],
            (IEnumerable<T3>) objs[2],
            (IEnumerable<T4>) objs[3],
            (IEnumerable<T5>) objs[4],
            (IEnumerable<T6>) objs[5],
            (IEnumerable<T7>) objs[6]);
    }


    public async
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>,
            IEnumerable<T6>>> GetMultipleAsync<T1, T2, T3, T4, T5, T6>(string query, object request,
            Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2,
            Func<SqlMapper.GridReader, IEnumerable<T3>> func3, Func<SqlMapper.GridReader, IEnumerable<T4>> func4,
            Func<SqlMapper.GridReader, IEnumerable<T5>> func5, Func<SqlMapper.GridReader, IEnumerable<T6>> func6,
            CommandType commandType = CommandType.StoredProcedure)
    {
        var objs = await CreateDbContext().Connection.GetMultipleAsync(query, request, commandType, func1, func2, func3, func4,
            func5, func6);

        return Tuple.Create(
            (IEnumerable<T1>) objs[0],
            (IEnumerable<T2>) objs[1],
            (IEnumerable<T3>) objs[2],
            (IEnumerable<T4>) objs[3],
            (IEnumerable<T5>) objs[4],
            (IEnumerable<T6>) objs[5]);
    }

    public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>>
        GetMultipleAsync<T1, T2, T3, T4, T5>(string query, object request,
            Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2,
            Func<SqlMapper.GridReader, IEnumerable<T3>> func3, Func<SqlMapper.GridReader, IEnumerable<T4>> func4,
            Func<SqlMapper.GridReader, IEnumerable<T5>> func5, CommandType commandType = CommandType.StoredProcedure)
    {
        var objs = await CreateDbContext().Connection.GetMultipleAsync(query, request, commandType, func1, func2, func3, func4,
            func5);

        return Tuple.Create(
            (IEnumerable<T1>) objs[0],
            (IEnumerable<T2>) objs[1],
            (IEnumerable<T3>) objs[2],
            (IEnumerable<T4>) objs[3],
            (IEnumerable<T5>) objs[4]);
    }

    public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> GetMultipleAsync<T1, T2, T3>(
        string query, object request, Func<SqlMapper.GridReader, IEnumerable<T1>> func1,
        Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3,
        CommandType commandType = CommandType.StoredProcedure)
    {
        var objs = await CreateDbContext().Connection.GetMultipleAsync(query, request, commandType, func1, func2, func3);

        return Tuple.Create(
            (IEnumerable<T1>) objs[0],
            (IEnumerable<T2>) objs[1],
            (IEnumerable<T3>) objs[2]);
    }

    public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> GetMultipleAsync<T1, T2>(string query, object request,
        Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2,
        CommandType commandType = CommandType.StoredProcedure)
    {
        var objs = await CreateDbContext().Connection.GetMultipleAsync(query, request, commandType, func1, func2);

        return Tuple.Create(
            (IEnumerable<T1>) objs[0],
            (IEnumerable<T2>) objs[1]);
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