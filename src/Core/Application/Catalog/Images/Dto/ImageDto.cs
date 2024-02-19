namespace Travaloud.Application.Catalog.Images.Dto;

public class ImageDto
{
    public DefaultIdType Id { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public string Title { get; set; } = default!;
    public string SubTitle1 { get; set; } = default!;
    public string SubTitle2 { get; set; } = default!;
    public string Href { get; set; } = default!;
    public string Html { get; set; } = default!;
}