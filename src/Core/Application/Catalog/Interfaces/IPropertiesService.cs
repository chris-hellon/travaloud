using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Common.Exporters;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IPropertiesService : ITransientService
{
    Task<PaginationResponse<PropertyDto>?> SearchAsync(SearchPropertiesRequest request);

    Task<PropertyDetailsDto?> GetAsync(DefaultIdType id);
    
    Task<DefaultIdType?> CreateAsync(CreatePropertyRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePropertyRequest request);

    Task<DefaultIdType?> DeleteAsync(DefaultIdType id);

    Task<FileResponse?> ExportAsync(ExportPropertiesRequest filter);
}
