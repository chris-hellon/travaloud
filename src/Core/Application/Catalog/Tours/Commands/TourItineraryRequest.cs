namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourItineraryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Header { get; set; } = default!;
    public DefaultIdType TourId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsCreate { get; set; }

    public IList<TourItinerarySectionRequest>? Sections { get; set; }
}