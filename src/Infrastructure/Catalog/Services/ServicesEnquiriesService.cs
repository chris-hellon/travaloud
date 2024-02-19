using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.ServicesEnquiries.Commands;
using Travaloud.Application.Catalog.ServicesEnquiries.Dto;
using Travaloud.Application.Catalog.ServicesEnquiries.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class ServicesEnquiriesService : BaseService, IServicesEnquiriesService
{
    public ServicesEnquiriesService(ISender mediator) : base(mediator)
    {
    }
    
    public async Task<PaginationResponse<ServiceEnquiryDto>?> SearchAsync(SearchServiceEnquiriesRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> CreateAsync(CreateServiceEnquiryRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateServiceEnquiryRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> DeleteAsync(DefaultIdType id)
    {
        return await Mediator.Send(new DeleteServiceEnquiryRequest(id));
    }

    public async Task<ServiceEnquiryDetailsDto?> GetAsync(DefaultIdType id)
    {
        return await Mediator.Send(new GetServiceEnquiryRequest(id));
    }
}