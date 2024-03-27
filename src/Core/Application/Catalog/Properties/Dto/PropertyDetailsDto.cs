using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Properties.Dto;

public class PropertyDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string TelephoneNumber { get; set; } = default!;
    public string? EmailAddress { get; set; }
    public string? GoogleMapsPlaceId { get; set; }
    public string? PageTitle { get; set; }
    public string? PageSubTitle { get; set; }
    public string? CloudbedsKey { get; set; }
    public string? CloudbedsApiKey { get; set; }
    public string? CloudbedsPropertyId { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string FriendlyUrl => Name.UrlFriendly();
    public bool? PublishToSite { get; set; }
    public string? UrlSlug { get; set; }
    public string? H1 { get; set; }
    public string? H2 { get; set; }
    public string? VideoPath { get; set; }

    public IList<PropertyDestinationLookupDto>? PropertyDestinationLookups { get; set; }
    public IList<PropertyDirectionDto>? Directions { get; set; }
    public IList<PropertyRoomDto>? Rooms { get; set; }
    public IList<PropertyFacilityDto>? Facilities { get; set; }
    public IList<TourDto>? Tours { get; set; }
    public IList<PropertyImageDto>? Images { get; set; }
}