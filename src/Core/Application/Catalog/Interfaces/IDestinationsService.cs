using Travaloud.Application.Catalog.Destinations.Commands;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Common.Exporters;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IDestinationsService : ITransientService
{
    Task<PaginationResponse<DestinationDto>?> SearchAsync(SearchDestinationsRequest request);

    Task<DefaultIdType?> CreateAsync(CreateDestinationRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateDestinationRequest request);

    Task<DefaultIdType?> DeleteAsync(DefaultIdType id);

    Task<DestinationDetailsDto?> GetAsync(DefaultIdType id);

    Task<FileResponse?> ExportAsync(ExportDestinationsRequest filter);
}