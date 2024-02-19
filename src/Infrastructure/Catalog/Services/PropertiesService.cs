using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Properties.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class PropertiesService : BaseService, IPropertiesService
{
    public PropertiesService(ISender mediator) : base(mediator)
    {
    }

    public async Task<PaginationResponse<PropertyDto>?> SearchAsync(SearchPropertiesRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> CreateAsync(CreatePropertyRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePropertyRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> DeleteAsync(DefaultIdType id)
    {
        return await Mediator.Send(new DeletePropertyRequest(id));
    }

    public async Task<PropertyDetailsDto?> GetAsync(DefaultIdType id)
    {
        return await Mediator.Send(new GetPropertyRequest(id));
    }

    public async Task<FileResponse?> ExportAsync(ExportPropertiesRequest filter)
    {
        var response = await Mediator.Send(filter);
        
        return new FileResponse(response);
    }
}