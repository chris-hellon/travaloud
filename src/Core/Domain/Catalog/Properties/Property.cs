using Travaloud.Domain.Catalog.Events;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Domain.Catalog.Properties;

public class Property : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? ShortDescription { get; private set; }
    public string? ImagePath { get; private set; }
    public string? ThumbnailImagePath { get; private set; }
    public string AddressLine1 { get; private set; } = default!;
    public string? AddressLine2 { get; private set; }
    public string TelephoneNumber { get; private set; } = default!;
    public string? EmailAddress { get; private set; }
    public string? GoogleMapsPlaceId { get; private set; }
    public string? PageTitle { get; private set; }
    public string? PageSubTitle { get; private set; }
    public string? CloudbedsKey { get; set; }
    public string? CloudbedsApiKey { get; set; }
    public string? CloudbedsPropertyId { get; set; }
    public string? MetaKeywords { get; private set; }
    public string? MetaDescription { get; private set; }
    public bool? PublishToSite { get; private set; }
    public string? UrlSlug { get; private set; }
    public string? H1 { get; private set; }
    public string? H2 { get; private set; }
    public string? VideoPath { get; private set; }
    public string? MobileVideoPath { get; private set; }
    public string? CustomSeoScripts { get; private set; }
    public string? SeoPageTitle { get; private set; }
    
    public virtual IList<TourPropertyLookup> TourPropertyLookups { get; set; } = default!;
    public virtual IList<PropertyDestinationLookup> PropertyDestinationLookups { get; set; } = default!;
    public virtual IList<PropertyDirection> Directions { get; set; } = default!;
    public virtual IList<PropertyRoom> Rooms { get; set; } = default!;
    public virtual IList<PropertyFacility> Facilities { get; set; } = default!;
    public virtual IList<Event> Events { get; set; } = default!;
    public virtual IList<PropertyImage>? Images { get; set; }

    public Property()
    {

    }

    public Property(string name,
        string description,
        string? shortDescription,
        string? imagePath,
        string? thumbnailImagePath,
        string addressLine1,
        string? addressLine2,
        string telephoneNumber,
        string? googleMapsPlaceId,
        string? pageTitle,
        string? pageSubTitle,
        string? cloudbedsKey,
        string? metaKeywords,
        string? metaDescription,
        string? emailAddress,
        bool? publishToSite,
        string? urlSlug,
        string? h1,
        string? h2,
        string? videoPath,
        string? mobileVideoPath,
        string? cloudbedsApiKey,
        string? cloudbedsPropertyId, 
        string? customSeoScripts,
        string? seoPageTitle)
    {
        Name = name;
        Description = description.FormatTextEditorString();;
        ShortDescription = shortDescription.FormatTextEditorString();;
        ImagePath = imagePath != null ? !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath : null;
        ThumbnailImagePath = thumbnailImagePath != null ? !thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath : null;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        TelephoneNumber = telephoneNumber;
        GoogleMapsPlaceId = googleMapsPlaceId;
        PageTitle = pageTitle;
        PageSubTitle = pageSubTitle;
        CloudbedsKey = cloudbedsKey;
        CloudbedsApiKey = cloudbedsApiKey;
        CloudbedsPropertyId = cloudbedsPropertyId;
        MetaKeywords = metaKeywords;
        MetaDescription = metaDescription;
        EmailAddress = emailAddress;
        PublishToSite = publishToSite;
        UrlSlug = urlSlug;
        H1 = h1;
        H2 = h2;
        VideoPath = videoPath;
        MobileVideoPath = mobileVideoPath;
        CustomSeoScripts = customSeoScripts;
        SeoPageTitle = seoPageTitle;
    }

    public Property Update(string? name,
        string? description,
        string? shortDescription,
        string? imagePath,
        string? thumbnailImagePath,
        string? addressLine1,
        string? addressLine2,
        string? telephoneNumber,
        string? googleMapsPlaceId,
        string? pageTitle,
        string? pageSubTitle,
        string? cloudbedsKey,
        string? metaKeywords,
        string? metaDescription,
        string? emailAddress,
        bool? publishToSite,
        string? urlSlug,
        string? h1,
        string? h2,
        string? videoPath,
        string? mobileVideoPath,
        string? cloudbedsApiKey,
        string? cloudbedsPropertyId, 
        string? customSeoScripts,
        string? seoPageTitle)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (description is not null && Description?.Equals(description) is not true) Description = description.FormatTextEditorString();;
        if (shortDescription is not null && ShortDescription?.Equals(shortDescription) is not true) ShortDescription = shortDescription.FormatTextEditorString();;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        if (thumbnailImagePath is not null && ThumbnailImagePath?.Equals(thumbnailImagePath) is not true) ThumbnailImagePath = thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
        if (addressLine1 is not null && AddressLine1?.Equals(addressLine1) is not true) AddressLine1 = addressLine1;
        if (addressLine2 is not null && AddressLine2?.Equals(addressLine2) is not true) AddressLine2 = addressLine2;
        if (addressLine1 is not null && AddressLine1?.Equals(addressLine1) is not true) AddressLine1 = addressLine1;
        if (telephoneNumber is not null && TelephoneNumber?.Equals(telephoneNumber) is not true) TelephoneNumber = telephoneNumber;
        if (googleMapsPlaceId is not null && GoogleMapsPlaceId?.Equals(googleMapsPlaceId) is not true) GoogleMapsPlaceId = googleMapsPlaceId;
        if (pageTitle is not null && PageTitle?.Equals(pageTitle) is not true) PageTitle = pageTitle;
        if (pageSubTitle is not null && PageSubTitle?.Equals(pageSubTitle) is not true) PageSubTitle = pageSubTitle;
        if (cloudbedsKey is not null && CloudbedsKey?.Equals(cloudbedsKey) is not true) CloudbedsKey = cloudbedsKey;
        if (cloudbedsApiKey is not null && CloudbedsApiKey?.Equals(cloudbedsApiKey) is not true) CloudbedsApiKey = cloudbedsApiKey;
        if (cloudbedsPropertyId is not null && CloudbedsPropertyId?.Equals(cloudbedsPropertyId) is not true) CloudbedsPropertyId = cloudbedsPropertyId;
        if (metaKeywords is not null && MetaKeywords?.Equals(metaKeywords) is not true) MetaKeywords = metaKeywords;
        if (metaDescription is not null && MetaDescription?.Equals(metaDescription) is not true) MetaDescription = metaDescription;
        if (emailAddress is not null && EmailAddress?.Equals(emailAddress) is not true) EmailAddress = emailAddress;
        if (urlSlug is not null && UrlSlug?.Equals(urlSlug) is not true) UrlSlug = urlSlug;
        if (h1 is not null && H1?.Equals(h1) is not true) H1 = h1;
        if (h2 is not null && H2?.Equals(h2) is not true) H2 = h2;
        if (videoPath is not null && VideoPath?.Equals(videoPath) is not true) VideoPath = videoPath;
        if (mobileVideoPath is not null && MobileVideoPath?.Equals(mobileVideoPath) is not true) MobileVideoPath = mobileVideoPath;
        if (customSeoScripts is not null && CustomSeoScripts?.Equals(customSeoScripts) is not true) CustomSeoScripts = customSeoScripts;
        if (seoPageTitle is not null && SeoPageTitle?.Equals(seoPageTitle) is not true) SeoPageTitle = seoPageTitle;
        
        PublishToSite = publishToSite;

        return this;
    }


    public Property ClearImagePath()
    {
        ImagePath = string.Empty;
        ThumbnailImagePath = string.Empty;
        return this;
    }

    public Property ClearVideoPath()
    {
        VideoPath = string.Empty;
        return this;
    }

    public Property ClearMobileVideoPath()
    {
        MobileVideoPath = string.Empty;
        return this;
    }
}