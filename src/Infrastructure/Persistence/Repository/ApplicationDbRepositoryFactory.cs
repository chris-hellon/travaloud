using Ardalis.Specification;
using Finbuckle.MultiTenant;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Travaloud.Application.Common.Events;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Application.Common.Persistence;
using Travaloud.Domain.Common.Contracts;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence.Context;

namespace Travaloud.Infrastructure.Persistence.Repository;

public class ApplicationDbRepositoryFactory<TEntity> :
    ApplicationDbFactoryContext<TEntity>, IReadRepository<TEntity>, IRepository<TEntity>, IRepositoryFactory<TEntity>
    where TEntity : class, IAggregateRoot
{
    public ApplicationDbRepositoryFactory(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        IConfiguration configuration,
        ICurrentUser currentUser,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher events)
        : base(dbContextFactory, multiTenantContextAccessor, configuration, currentUser, serializer, dbSettings, events)
    {
    }

    public ApplicationDbRepositoryFactory(IDbContextFactory<ApplicationDbContext> dbContextFactory,
        ISpecificationEvaluator specificationEvaluator,
        IConfiguration configuration,
        IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor,
        ICurrentUser currentUser,
        ISerializerService serializer,
        IOptions<DatabaseSettings> dbSettings,
        IEventPublisher events)
        : base(dbContextFactory, specificationEvaluator, configuration, multiTenantContextAccessor, currentUser, serializer, dbSettings, events)
    {
    }
    
    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<TEntity, TResult> specification, ApplicationDbContext dbContext) =>
        ApplySpecification(specification, dbContext, false)
            .ProjectToType<TResult>();
}