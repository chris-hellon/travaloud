namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourCategoryLookupRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType TourCategoryId { get; set; }
    public DefaultIdType? ParentTourCategoryId { get; set; }
}