using Travaloud.Application.Catalog.TourDates.Specification;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class SearchToursByDateRangeAndDestinationsRequest : IRequest<IEnumerable<TourDetailsDto>>
{
    public List<DefaultIdType> DestinationIds { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public SearchToursByDateRangeAndDestinationsRequest(List<DefaultIdType> destinationIds, DateTime fromDate, DateTime toDate)
    {
        DestinationIds = destinationIds;
        FromDate = fromDate;
        ToDate = toDate;
    }
}

internal class SearchToursByDateRangeAndDestinationsRequestHandler : IRequestHandler<
    SearchToursByDateRangeAndDestinationsRequest, IEnumerable<TourDetailsDto>>
{
    private readonly IRepositoryFactory<Tour> _repository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;

    public SearchToursByDateRangeAndDestinationsRequestHandler(IRepositoryFactory<Tour> repository,
        IRepositoryFactory<TourDate> tourDateRepository)
    {
        _repository = repository;
        _tourDateRepository = tourDateRepository;
    }

    public async Task<IEnumerable<TourDetailsDto>> Handle(SearchToursByDateRangeAndDestinationsRequest request, CancellationToken cancellationToken)
    {
        var toursWithoutDates = await _repository.ListAsync(new ToursByDateRangeAndDestinationsSpec(request), cancellationToken);
        var tourDetails = new List<TourDetailsDto>();

        if (toursWithoutDates.Count == 0) return tourDetails;
        
        tourDetails = toursWithoutDates.Adapt<List<TourDetailsDto>>();
            
        var tourIds = toursWithoutDates.Select(x => x.Id).ToList();
        var tourDates = await _tourDateRepository.ListAsync(
            new TourDatesByTourIdsWithinRangeSpec(tourIds, request.FromDate, request.ToDate), cancellationToken);

        if (tourDates.Count != 0)
        {
            tourDetails = tourDetails.Select(x =>
            {
                x.TourPrices = x.TourPrices.Where(tp => tp.PublishToWebsite.HasValue && tp.PublishToWebsite.Value).ToList();
                var tourPriceIds = x.TourPrices.Select(tp => tp.Id);
                x.TourDates = tourDates.Where(td => td.TourId == x.Id && tourPriceIds.Contains(td.TourPrice.Id) && td.StartDate > DateTime.Now).ToList();
                return x;
            }).ToList();
        }

        return tourDetails;
    }
}