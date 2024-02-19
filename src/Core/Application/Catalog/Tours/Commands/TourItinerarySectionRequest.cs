namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourItinerarySectionRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? TourItineraryId { get; set; }
    public string Title { get; set; } = default!;
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? Highlights { get; set; }
    public bool IsCreate { get; set; }
    public IList<TourItinerarySectionImageRequest>? Images { get; set; }
}