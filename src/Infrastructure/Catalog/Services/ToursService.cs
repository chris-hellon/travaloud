using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class ToursService : BaseService, IToursService
{
    public ToursService(ISender mediator) : base(mediator)
    {
    }

    public async Task<PaginationResponse<TourDto>?> SearchAsync(SearchToursRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> CreateAsync(CreateTourRequest request)
    {
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateTourRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> DeleteAsync(DefaultIdType id)
    {
        return await Mediator.Send(new DeleteTourRequest(id));
    }

    public async Task<TourDetailsDto?> GetAsync(DefaultIdType id)
    {
        return await Mediator.Send(new GetTourRequest(id));
    }

    public Task<IEnumerable<TourCategoryDto>> GetCategoriesAsync()
    {
        return Mediator.Send(new GetTourCategoriesRequest());
    }
    
    public Task<IEnumerable<TourCategoryDto>> GetParentCategoriesAsync()
    {
        return Mediator.Send(new GetParentTourCategoriesRequest());
    }
    
    public Task<PaginationResponse<TourDateDto>> GetTourDatesAsync(DefaultIdType tourId, int requestedSpaces)
    {
        return Mediator.Send(new GetTourDatesRequest(tourId, requestedSpaces));
    }
    
    public Task<bool> GetTourDatesByPriceAsync(GetTourDatesByPriceRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<FileResponse?> ExportAsync(ExportToursRequest filter)
    {
        var response = await Mediator.Send(filter);
        
        return new FileResponse(response);
    }

    public Task<IEnumerable<TourWithoutDatesDto>> GetToursWithDetails(GetToursWithDetailsRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<IEnumerable<TourWithoutDatesDto>> GetToursByDestinations(GetToursByDestinationsRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<IEnumerable<TourDetailsDto>> GetToursByDestinations(GetToursByDestinationsWithDatesRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<IEnumerable<TourPickupLocationDto>> GetTourPickupLocations(DefaultIdType id)
    {
        return Mediator.Send(new GetTourPickupLocationsRequest(id));
    }

    public Task<IEnumerable<TourDetailsDto>> SearchToursByDateRangeAndDestinations(SearchToursByDateRangeAndDestinationsRequest request)
    {
        return Mediator.Send(request);
    }
}