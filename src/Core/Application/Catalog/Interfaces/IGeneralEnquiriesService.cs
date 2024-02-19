using Travaloud.Application.Catalog.Enquiries.Commands;
using Travaloud.Application.Catalog.Enquiries.Dto;
using Travaloud.Application.Catalog.Enquiries.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IGeneralEnquiriesService : ITransientService
{
    Task<PaginationResponse<GeneralEnquiryDto>?> SearchAsync(SearchGeneralEnquiriesRequest request);

    Task<DefaultIdType?> CreateAsync(CreateGeneralEnquiryRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateGeneralEnquiryRequest request);

    Task<DefaultIdType?> DeleteAsync(DefaultIdType id);

    Task<GeneralEnquiryDto?> GetAsync(DefaultIdType id);
}