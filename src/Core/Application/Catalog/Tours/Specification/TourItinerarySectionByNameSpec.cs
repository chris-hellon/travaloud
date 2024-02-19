using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourItinerarySectionByNameSpec : Specification<TourItinerarySection>, ISingleResultSpecification<TourItinerarySection>
{
    public TourItinerarySectionByNameSpec(string title, DefaultIdType? tourItineraryId) =>
        Query.Where(p => p.Title == title && p.TourItineraryId == tourItineraryId);
}