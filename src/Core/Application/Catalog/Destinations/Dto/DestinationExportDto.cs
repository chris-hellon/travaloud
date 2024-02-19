namespace Travaloud.Application.Catalog.Destinations.Dto;

public class DestinationExportDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Directions { get; set; }
    public string? ImagePath { get; set; }
    public string? GoogleMapsKey { get; set; }
}