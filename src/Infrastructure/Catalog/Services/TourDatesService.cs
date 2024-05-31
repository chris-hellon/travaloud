using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TourDates.Queries;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class TourDatesService : BaseService, ITourDatesService
{
    public TourDatesService(ISender mediator) : base(mediator)
    {
    }
    
    public Task<PaginationResponse<TourDateDto>> SearchAsync(SearchTourDatesRequest request)
    {
        return Mediator.Send(request);
    }
}