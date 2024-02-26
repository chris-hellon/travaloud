using Travaloud.Application.Catalog.Services.Commands;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IServicesService : ITransientService
{
    /// <summary>
    /// Search services using available filters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<PaginationResponse<ServiceDto>> SearchAsync(SearchServiceRequest request);

    /// <summary>
    /// Get service details.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ServiceDetailsDto> GetAsync(DefaultIdType id);

    Task<ServiceDetailsDto> GetByNameAsync(string name);
    
    /// <summary>
    /// Create a new service.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<DefaultIdType> CreateAsync(CreateServiceRequest request);

    /// <summary>
    /// Update a service.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateServiceRequest request);

    /// <summary>
    /// Delete a service.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}