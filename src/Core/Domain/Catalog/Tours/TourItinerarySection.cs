namespace Travaloud.Domain.Catalog.Tours;

public class TourItinerarySection : AuditableEntity, IAggregateRoot
{
    public DefaultIdType TourItineraryId { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? Highlights { get; set; }
    public int? SortOrder { get; set; }
    
    public IList<TourItinerarySectionImage>? Images { get; set; }

    public TourItinerarySection()
    {

    }

    public TourItinerarySection(string? title, string? subTitle, string? description, string? highlights, IList<TourItinerarySectionImage>? images, DefaultIdType? tourItineraryId = null)
    {
        if (tourItineraryId != null)
        {
            TourItineraryId = tourItineraryId.Value;
        }

        Title = title;
        SubTitle = subTitle;
        Description = description;
        Highlights = highlights;
        Images = images;
    }

    public TourItinerarySection Update(DefaultIdType? tourItineraryId, string? title, string? subTitle, string? description, string? highlights, IList<TourItinerarySectionImage>? images)
    {
        if (subTitle is not null && SubTitle?.Equals(subTitle) is not true) SubTitle = subTitle;
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (description is not null && Description?.Equals(description) is not true) Description = description.FormatTextEditorString();;
        if (highlights is not null && Highlights?.Equals(highlights) is not true) Highlights = highlights.FormatTextEditorString();;
        if (tourItineraryId.HasValue && tourItineraryId.Value != DefaultIdType.Empty && !TourItineraryId.Equals(tourItineraryId.Value)) TourItineraryId = tourItineraryId.Value;
        if (images != null && Images?.SequenceEqual(images) == false) Images = images;

        return this;
    }
}