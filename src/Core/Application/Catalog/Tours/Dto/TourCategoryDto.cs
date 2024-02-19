namespace Travaloud.Application.Catalog.Tours.Dto;

public class TourCategoryDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public bool? TopLevelCategory { get; set; }
    public DefaultIdType? ParentTourCategoryId { get; set; }
    public IList<TourCategoryDto>? ParentTourCategories { get; set; }
}