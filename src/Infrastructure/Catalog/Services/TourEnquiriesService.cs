using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TourEnquiries.Commands;
using Travaloud.Application.Catalog.TourEnquiries.Dto;
using Travaloud.Application.Catalog.TourEnquiries.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class TourEnquiriesService : BaseService, ITourEnquiriesService
{
    public TourEnquiriesService(ISender mediator) : base(mediator)
    {
    }
    
    public async Task<PaginationResponse<TourEnquiryDto>?> SearchAsync(SearchTourEnquiriesRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> CreateAsync(CreateTourEnquiryRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateTourEnquiryRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> DeleteAsync(DefaultIdType id)
    {
        return await Mediator.Send(new DeleteTourEnquiryRequest(id));
    }

    public async Task<TourEnquiryDto?> GetAsync(DefaultIdType id)
    {
        return await Mediator.Send(new GetTourEnquiryRequest(id));
    }
}