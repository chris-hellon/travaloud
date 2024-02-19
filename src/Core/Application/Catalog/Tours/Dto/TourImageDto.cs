namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourImageDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string ImagePath { get; set; } = default!;
    public string ThumbnailImagePath { get; set; } = default!;
}