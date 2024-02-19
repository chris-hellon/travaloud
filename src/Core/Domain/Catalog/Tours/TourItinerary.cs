namespace Travaloud.Domain.Catalog.Tours;

public class TourItinerary : AuditableEntity, IAggregateRoot
{
    public string Header { get; set; } = default!;
    public DefaultIdType TourId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }

    public virtual IList<TourItinerarySection> Sections { get; set; } = default!;

    public TourItinerary()
    {

    }

    public TourItinerary(string header, string? title, string? description, IList<TourItinerarySection>? sections, DefaultIdType? tourId = null)
    {
        Header = header;
        Title = title;
        Description = description;

        if (sections != null)
        {
            Sections = sections;
        }

        if (tourId != null)
        {
            TourId = tourId.Value;
        }
    }

    public TourItinerary Update(string? header, string? title, string? description, DefaultIdType? tourId, IList<TourItinerarySection>? sections)
    {
        if (header is not null && Header?.Equals(header) is not true) Header = header;
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (tourId.HasValue && tourId.Value != DefaultIdType.Empty && !TourId.Equals(tourId.Value)) TourId = tourId.Value;
        if (sections != null && Sections?.SequenceEqual(sections) == false) Sections = sections;

        return this;
    }
}