namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class HeaderBannerComponent
{
    public string? Title { get; private set; }
    public string? SubTitle { get; private set; }
    public string? Body { get; private set; }
    public string? ImageSrc { get; private set; }
    public decimal? BackgroundTop { get; set; }
    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; }
    public List<string>? SlideshowImages { get; set; }
    public bool FullHeight { get; set; }

    public HeaderBannerComponent()
    {

    }

    public HeaderBannerComponent(string title, string? subTitle, string? body, string? imageSrc, IEnumerable<OvalContainerComponent>? ovalContainers = null)
    {
        Title = title;
        SubTitle = subTitle;
        Body = body;
        ImageSrc = imageSrc;
        OvalContainers = ovalContainers;
    }

    public HeaderBannerComponent(string title, string subTitle, string? body, List<string> slideshowImages, IEnumerable<OvalContainerComponent>? ovalContainers = null)
    {
        Title = title;
        SubTitle = subTitle;
        Body = body;
        ImageSrc = null;
        SlideshowImages = slideshowImages;
        OvalContainers = ovalContainers;
    }

    public HeaderBannerComponent(string? imageSrc)
    {
        Title = null;
        SubTitle = null;
        Body = null;
        ImageSrc = imageSrc;
    }
}