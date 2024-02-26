using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Services.Commands;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Services.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class ServicesService : BaseService, IServicesService
{
    public ServicesService(ISender mediator) : base(mediator)
    {
    }

    public Task<PaginationResponse<ServiceDto>> SearchAsync(SearchServiceRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<ServiceDetailsDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetServiceRequest(id));
    }
    
    public Task<ServiceDetailsDto> GetByNameAsync(string name)
    {
        return Mediator.Send(new GetServiceByNameRequest(name));
    }
    
    public Task<DefaultIdType> CreateAsync(CreateServiceRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateServiceRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeleteServiceRequest(id));
    }
}