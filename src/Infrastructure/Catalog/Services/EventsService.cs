using MediatR;
using Travaloud.Application.Catalog.Events.Commands;
using Travaloud.Application.Catalog.Events.DTO;
using Travaloud.Application.Catalog.Events.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class EventsService : BaseService, IEventsService
{
    public EventsService(ISender mediator) : base(mediator)
    {
    }
    
    public Task<PaginationResponse<EventDto>> SearchAsync(SearchEventsRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<EventDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetEventRequest(id));
    }
    
    public Task<DefaultIdType> CreateAsync(CreateEventRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateEventRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeleteEventRequest(id));
    }
}