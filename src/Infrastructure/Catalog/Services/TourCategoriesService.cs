using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class TourCategoriesService : BaseService, ITourCategoriesService
{
    public TourCategoriesService(ISender mediator) : base(mediator)
    {
    }
    
    public Task<PaginationResponse<TourCategoryDto>> SearchAsync(SearchTourCategoriesRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<TourCategoryDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetTourCategoryRequest(id));
    }
    
    public Task<DefaultIdType> CreateAsync(CreateTourCategoryRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateTourCategoryRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeleteTourCategoryRequest(id));
    }
}