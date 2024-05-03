namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourPickupLocationDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType TourId { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string? PropertyName { get; set; }
}