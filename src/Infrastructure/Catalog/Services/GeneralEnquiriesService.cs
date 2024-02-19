using MediatR;
using Travaloud.Application.Catalog.Enquiries.Commands;
using Travaloud.Application.Catalog.Enquiries.Dto;
using Travaloud.Application.Catalog.Enquiries.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class GeneralEnquiriesService : BaseService, IGeneralEnquiriesService
{
    public GeneralEnquiriesService(ISender mediator) : base(mediator)
    {
    }

    public async Task<PaginationResponse<GeneralEnquiryDto>?> SearchAsync(SearchGeneralEnquiriesRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> CreateAsync(CreateGeneralEnquiryRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateGeneralEnquiryRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> DeleteAsync(DefaultIdType id)
    {
        return await Mediator.Send(new DeleteGeneralEnquiryRequest(id));
    }

    public async Task<GeneralEnquiryDto?> GetAsync(DefaultIdType id)
    {
        return await Mediator.Send(new GetGeneralEnquiryRequest(id));
    }
}