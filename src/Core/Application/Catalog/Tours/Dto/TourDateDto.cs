namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourDateDto
{
    public DefaultIdType Id { get; set; }
    public DateTime StartDate { get; set; }
    public DefaultIdType TourId { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableSpaces { get; set; }
    public DefaultIdType TourPriceId { get; set; }
    public int ConcurrencyVersion { get; set; } = default!;
    public TourPriceDto? TourPrice { get; set; }
}