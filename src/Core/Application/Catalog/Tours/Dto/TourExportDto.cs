namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourExportDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    public int MaxCapacity { get; set; }
    public int MinCapacity { get; set; }
    public int DayDuration { get; set; }
    public int NightDuration { get; set; }
    public string? Address { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? WhatsIncluded { get; set; }
    public string? WhatsNotIncluded { get; set; }
    public string? AdditionalInformation { get; set; }
}