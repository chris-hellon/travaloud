namespace Travaloud.Domain.Catalog.Images;

public class Image : AuditableEntity, IAggregateRoot
{
    public Image(string? imagePath,
        string? thumbnailImagePath,
        string title,
        string subTitle1,
        string subTitle2,
        string href)
    {
        ImagePath = imagePath;
        ThumbnailImagePath = thumbnailImagePath;
        Title = title;
        SubTitle1 = subTitle1;
        SubTitle2 = subTitle2;
        Href = href;
    }

    public string? ImagePath { get; private set; }
    public string? ThumbnailImagePath { get; private set; }
    public string Title { get; private set; }
    public string SubTitle1 { get; private set; }
    public string SubTitle2 { get; private set; }
    public string Href { get; private set; }

    public DefaultIdType? TourId { get; set; }
    public DefaultIdType? PropertyId { get; set; }
    public DefaultIdType? PropertyRoomId { get; set; }
    public DefaultIdType? DestinationId { get; set; }

    public Image Update(string? imagePath, string? thumbnailImagePath, string? title, string? subTitle1, string? subTitle2, string? href)
    {
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = imagePath;
        if (thumbnailImagePath is not null && ThumbnailImagePath?.Equals(thumbnailImagePath) is not true) ThumbnailImagePath = thumbnailImagePath;
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (subTitle1 is not null && SubTitle1?.Equals(subTitle1) is not true) SubTitle1 = subTitle1;
        if (subTitle2 is not null && SubTitle2?.Equals(subTitle2) is not true) SubTitle2 = subTitle2;
        if (href is not null && Href?.Equals(href) is not true) Href = href;
        return this;
    }
}