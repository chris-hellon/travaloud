namespace Travaloud.Application.Catalog.PageSorting.Dto;

public class PageSortingDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? PageId { get; set; }
    public DefaultIdType? ParentTourCategoryId { get; set; }
    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? TourCategoryId { get; set; }
    public DefaultIdType? PropertyId { get; set; }
    public DefaultIdType? DestinationId { get; set; }
    public int SortOrder { get; set; }

    public PageSortingDto()
    {

    }
}