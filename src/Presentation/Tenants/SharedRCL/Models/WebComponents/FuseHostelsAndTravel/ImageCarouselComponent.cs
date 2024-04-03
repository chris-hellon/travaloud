namespace Travaloud.Tenants.SharedRCL.Models.WebComponents.FuseHostelsAndTravel;

public class ImageCarouselComponent : BaseComponent
{
    public string ImageSrc { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? SubTitle { get; private set; }
    public string? SmallTitle { get; private set; } 
    public List<string>? Urls { get; private set; }

    public ImageCarouselComponent()
    {

    }

    public ImageCarouselComponent(string imageSrc, string title, string? subTitle = null, string? smallTitle = null, List<string>? urls = null)
    {
        ImageSrc = imageSrc;
        Title = title;
        SubTitle = subTitle;
        SmallTitle = smallTitle;
        Urls = urls;
    }
}