using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Partners.Commands;
using Travaloud.Application.Catalog.Partners.Dto;
using Travaloud.Application.Catalog.Partners.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class PartnersService : BaseService, IPartnersService
{
    public PartnersService(ISender mediator) : base(mediator)
    {
    }
    
    public Task<PaginationResponse<PartnerDto>> SearchAsync(SearchPartnersRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<PartnerDetailsDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetPartnerRequest(id));
    }
    
    public Task<DefaultIdType> CreateAsync(CreatePartnerRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePartnerRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeletePartnerRequest(id));
    }
}