using Travaloud.Application.Catalog.TourEnquiries.Commands;
using Travaloud.Application.Catalog.TourEnquiries.Dto;
using Travaloud.Application.Catalog.TourEnquiries.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface ITourEnquiriesService : ITransientService
{
    Task<PaginationResponse<TourEnquiryDto>?> SearchAsync(SearchTourEnquiriesRequest request);

    Task<DefaultIdType?> CreateAsync(CreateTourEnquiryRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateTourEnquiryRequest request);

    Task<DefaultIdType?> DeleteAsync(DefaultIdType id);

    Task<TourEnquiryDto?> GetAsync(DefaultIdType id);
}