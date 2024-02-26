using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TravelGuides.Commands;
using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class TravelGuidesService : BaseService, ITravelGuidesService
{
    public TravelGuidesService(ISender mediator) : base(mediator)
    {
    }

    public Task<PaginationResponse<TravelGuideDto>> SearchAsync(SearchTravelGuidesRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<TravelGuideDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetTravelGuideRequest(id));
    }
    
    public Task<TravelGuideDto> GetByFriendlyTitleAsync(string title)
    {
        return Mediator.Send(new GetTravelGuideByFriendlyTitleRequest(title));
    }
    
    public Task<DefaultIdType> CreateAsync(CreateTravelGuideRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateTravelGuideRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeleteTravelGuideRequest(id));
    }
}