using MediatR;
using Travaloud.Application.Catalog.Destinations.Commands;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Destinations.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class DestinationsService : BaseService, IDestinationsService
{
    public DestinationsService(ISender mediator) : base(mediator)
    {
    }

    public async Task<PaginationResponse<DestinationDto>?> SearchAsync(SearchDestinationsRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> CreateAsync(CreateDestinationRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateDestinationRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> DeleteAsync(DefaultIdType id)
    {
        return await Mediator.Send(new DeleteDestinationRequest(id));
    }

    public async Task<DestinationDetailsDto?> GetAsync(DefaultIdType id)
    {
        return await Mediator.Send(new GetDestinationRequest(id));
    }

    public async Task<FileResponse?> ExportAsync(ExportDestinationsRequest filter)
    {
        var response = await Mediator.Send(filter);
        
        return new FileResponse(response);
    }
}