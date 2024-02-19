namespace Travaloud.Domain.Catalog.Pages;

public class PageSorting : AuditableEntity, IAggregateRoot
{
    public PageSorting(DefaultIdType? pageId,
        DefaultIdType? parentTourCategoryId,
        DefaultIdType? tourCategoryId,
        DefaultIdType? tourId,
        DefaultIdType? propertyId,
        DefaultIdType? destinationId)
    {
        PageId = pageId;
        ParentTourCategoryId = parentTourCategoryId;
        TourId = tourId;
        TourCategoryId = tourCategoryId;
        PropertyId = propertyId;
        DestinationId = destinationId;
    }

    public DefaultIdType? PageId { get; private set; }
    public DefaultIdType? ParentTourCategoryId { get; private set; }
    public DefaultIdType? TourId { get; private set; }
    public DefaultIdType? TourCategoryId { get; private set; }
    public DefaultIdType? PropertyId { get; private set; }
    public DefaultIdType? DestinationId { get; private set; }
}