namespace Travaloud.Application.Catalog.Properties.Dto;

public class PropertyExportDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string TelephoneNumber { get; set; } = default!;
    public string? EmailAddress { get; set; }
    public string GoogleMapsPlaceId { get; set; } = default!;
    public string? PageTitle { get; set; }
    public string? PageSubTitle { get; set; }
    public string? CloudbedsKey { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
}