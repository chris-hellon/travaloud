using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Travaloud.Application.Common.Events;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Common.Persistence;
using Travaloud.Domain.Common.Contracts;
using Travaloud.Infrastructure.Common;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence.ConnectionString;
using Travaloud.Infrastructure.Persistence.Context;
using Travaloud.Infrastructure.Persistence.Initialization;
using Travaloud.Infrastructure.Persistence.Repository;
using Travaloud.Infrastructure.Persistence.Extensions;

namespace Travaloud.Infrastructure.Persistence;

internal static class Startup
{
    private static readonly ILogger Logger = Log.ForContext(typeof(Startup));

    internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        // TODO: there must be a cleaner way to do IOptions validation...
        var databaseSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
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

        Logger.Information("Current DB Provider : {DbProvider}", dbProvider);

        return services
            .Configure<DatabaseSettings>(config.GetSection(nameof(DatabaseSettings)))
            .AddDbContextFactory<ApplicationDbContext>(options =>
            {
                options.UseDatabase(dbProvider, rootConnectionString);
            }, ServiceLifetime.Scoped)
            .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
            .AddTransient<ApplicationDbInitializer>()
            .AddTransient<ApplicationDbSeeder>()
            .AddServices(typeof(ICustomSeeder), ServiceLifetime.Transient)
            .AddTransient<CustomSeederRunner>()
            .AddTransient<IConnectionStringSecurer, ConnectionStringSecurer>()
            .AddTransient<IConnectionStringValidator, ConnectionStringValidator>()
            .AddRepositories(config);
    }

    internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString)
    {
        return dbProvider.ToLowerInvariant() switch
        {
            DbProviderKeys.SqlServer => builder.UseSqlServer(connectionString,
                e => e.MigrationsAssembly("Migrators.SqlServer")),
            _ => throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.")
        };
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
    {
        // Add Repositories
        services.AddScoped(typeof(IRepository<>), typeof(ApplicationDbRepository<>));
        
        var dbContextFactoryType = typeof(IDbContextFactory<>).MakeGenericType(typeof(ApplicationDbContext));
        var multiTenantAccessorType = typeof(IMultiTenantContextAccessor<TravaloudTenantInfo>);
        var currentUserType = typeof(ICurrentUser);
        var serializerServiceType = typeof(ISerializerService);
        var dbOptionsType = typeof(IOptions<DatabaseSettings>);
        var eventPublisherType = typeof(IEventPublisher);

        services.AddScoped<ApplicationDbContext>(sp =>
        {
            var multiTenantAccessor = sp.GetRequiredService(multiTenantAccessorType);
            var currentUser = sp.GetRequiredService(currentUserType);
            var serializerService = sp.GetRequiredService(serializerServiceType);
            var dbOptions = sp.GetRequiredService(dbOptionsType);
            var eventPublisher = sp.GetRequiredService(eventPublisherType);
            
            return sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext(
                config, 
                (ICurrentUser) currentUser,
                (IMultiTenantContextAccessor<TravaloudTenantInfo>) multiTenantAccessor,
                (ISerializerService) serializerService,
                (IOptions<DatabaseSettings>) dbOptions,
                (IEventPublisher) eventPublisher);
        });
        
        foreach (var aggregateRootType in
            typeof(IAggregateRoot).Assembly.GetExportedTypes()
                .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
                .ToList())
        {
            // Add ReadRepositories.
            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
                sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));

            var repositoryFactoryType = typeof(IRepositoryFactory<>).MakeGenericType(aggregateRootType);
            
            services.AddScoped(repositoryFactoryType, sp =>
            {
                var dbContextFactory = sp.GetRequiredService(dbContextFactoryType);
                var multiTenantAccessor = sp.GetRequiredService(multiTenantAccessorType);
                var currentUser = sp.GetRequiredService(currentUserType);
                var serializerService = sp.GetRequiredService(serializerServiceType);
                var dbOptions = sp.GetRequiredService(dbOptionsType);
                var eventPublisher = sp.GetRequiredService(eventPublisherType);

                var concreteFactoryType = typeof(ApplicationDbRepositoryFactory<>).MakeGenericType(aggregateRootType);
                return Activator.CreateInstance(concreteFactoryType, dbContextFactory, multiTenantAccessor, config, currentUser, serializerService, dbOptions, eventPublisher)
                       ?? throw new InvalidOperationException($"Couldn't create {concreteFactoryType.Name}");
            });
            
            // Decorate the repositories with EventAddingRepositoryDecorators and expose them as IRepositoryWithEvents.
            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), sp =>
                Activator.CreateInstance(
                    typeof(EventAddingRepositoryDecorator<>).MakeGenericType(aggregateRootType),
                    sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)))
                ?? throw new InvalidOperationException($"Couldn't create EventAddingRepositoryDecorator for aggregateRootType {aggregateRootType.Name}"));
        }

        return services;
    }
}