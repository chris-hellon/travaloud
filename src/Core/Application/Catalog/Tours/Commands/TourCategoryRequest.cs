namespace Travaloud.Application.Catalog.Tours.Commands;

public class TourCategoryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public FileUploadRequest? Image { get; set; }
    public bool IsCreate { get; set; }
    public bool? TopLevelCategory { get; set; }
}