using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class ToursByDateRangeAndDestinationsSpec : Specification<Tour, TourWithoutDatesDto>
{
    public ToursByDateRangeAndDestinationsSpec(SearchToursByDateRangeAndDestinationsRequest request) =>
        Query
            .Include(p => p.Images)
            .Include(p => p.TourPrices)
            .Where(p => p.TourDestinationLookups != null && p.TourDestinationLookups.Any(lookup => request.DestinationIds.Contains(lookup.DestinationId)))
            .Where(x => x.TourDates != null &&
                        x.TourDates.Any(tourDate =>
                            tourDate.StartDate >= request.FromDate && tourDate.StartDate <= request.ToDate))
            .Where(x => x.TourPrices != null && x.TourPrices.Any(p => p.PublishToWebsite.HasValue && p.PublishToWebsite.Value))
            .OrderBy(x => x.Name);
}