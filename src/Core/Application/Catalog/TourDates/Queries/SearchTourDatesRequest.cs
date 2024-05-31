using Travaloud.Application.Catalog.TourDates.Specification;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.TourDates.Queries;

public class SearchTourDatesRequest : PaginationFilter, IRequest<PaginationResponse<TourDateDto>>
{
    public DefaultIdType? TourId { get; set; }
    
    public DefaultIdType? PriceId { get; set; }
    
    public int? RequestedSpaces { get; set; }
    
    public DateTime? EndDate { get; set; }
}

public class SearchTourDatesRequestHandler : IRequestHandler<SearchTourDatesRequest, PaginationResponse<TourDateDto>>
{
    private readonly IStringLocalizer<SearchTourDatesRequestHandler> _localizer;
    private readonly IRepositoryFactory<TourDate> _repository;

    public SearchTourDatesRequestHandler(
        IRepositoryFactory<TourDate> repository,
        IStringLocalizer<SearchTourDatesRequestHandler> localizer)
    {
        _repository = repository; 
        _localizer = localizer;
    }

    public async Task<PaginationResponse<TourDateDto>> Handle(SearchTourDatesRequest request, CancellationToken cancellationToken)
    {
        return await _repository.PaginatedListAsync(new SearchTourDatesSpec(request), 1, 99999, cancellationToken: cancellationToken);
    }
}