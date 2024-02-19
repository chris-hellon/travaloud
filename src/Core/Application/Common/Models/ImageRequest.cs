namespace Travaloud.Application.Common.Models;

public class ImageRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public string Title { get; set; } = default!;
    public string SubTitle1 { get; set; } = default!;
    public string SubTitle2 { get; set; } = default!;
    public string Href { get; set; } = default!;
}