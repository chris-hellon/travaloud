using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.Persistence.Initialization;

internal interface IDatabaseInitializer
{
    Task InitializeDatabasesAsync(CancellationToken cancellationToken);
    Task InitializeApplicationDbForTenantAsync(TravaloudTenantInfo tenant, CancellationToken cancellationToken);
}