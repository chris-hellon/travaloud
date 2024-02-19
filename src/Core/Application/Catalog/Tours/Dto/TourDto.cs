namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public decimal? Price { get; set; }
    public string? PriceLabel { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public int? MaxCapacity { get; set; }
    public int? MinCapacity { get; set; }
    public string? DayDuration { get; set; }
    public string? NightDuration { get; set; }
    public string? HourDuration { get; set; }
    public string? Address { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? WhatsIncluded { get; set; }
    public string? WhatsNotIncluded { get; set; }
    public string? AdditionalInformation { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string FriendlyUrl => Name.UrlFriendly();
}