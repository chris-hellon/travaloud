using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Interfaces;

public interface ITenantWebsiteService : ITransientService
{
    Task<IEnumerable<PropertyDto>?> GetProperties(CancellationToken cancellationToken);
    
    Task<IEnumerable<TourDto>?> GetTours(CancellationToken cancellationToken);

    Task<IEnumerable<ServiceDto>?> GetServices(CancellationToken cancellationToken);

    Task<IEnumerable<DestinationDto>?> GetDestinations(CancellationToken cancellationToken);
}