namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourCategoryLookupDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType TourCategoryId { get; set; }
    public DefaultIdType? ParentTourCategoryId { get; set; }
}