using Travaloud.Application.Catalog.ServicesEnquiries.Commands;
using Travaloud.Application.Catalog.ServicesEnquiries.Dto;
using Travaloud.Application.Catalog.ServicesEnquiries.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IServicesEnquiriesService : ITransientService
{
    Task<PaginationResponse<ServiceEnquiryDto>?> SearchAsync(SearchServiceEnquiriesRequest request);

    Task<DefaultIdType?> CreateAsync(CreateServiceEnquiryRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateServiceEnquiryRequest request);

    Task<DefaultIdType?> DeleteAsync(DefaultIdType id);

    Task<ServiceEnquiryDetailsDto?> GetAsync(DefaultIdType id);
}