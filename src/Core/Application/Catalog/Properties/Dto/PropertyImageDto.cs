namespace Travaloud.Application.Catalog.Properties.Dto;

public class PropertyImageDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string ImagePath { get; set; } = default!;
    public string ThumbnailImagePath { get; set; } = default!;
    public int? SortOrder { get; set; }
}