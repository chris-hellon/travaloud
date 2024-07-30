namespace Travaloud.Application.Catalog.TourDates.Dto;

public class TourDailyManifestDto
{
    public DefaultIdType DestinationId { get; set; }
    public DefaultIdType TourId { get; set; }
    public DefaultIdType TourDateId { get; set; }
    public DateTime StartDate { get; set; }
    public string? TourName { get; set; }
}