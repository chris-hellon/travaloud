namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class CardComponent : BaseComponent
{
    public string? Title { get; private set; }
    public string? Body { get; private set; }
    public string? ImageSrc { get; private set; }
    public string? VideoSrc { get; private set; }
    public string? ThumbnailImageSrc { get; private set; }
    public string? AnimationStart { get; set; } = "onScroll";
    public decimal? AnimationDelay { get; set; } = 200;
    public List<string>? LightboxImages { get; private set; }
    public ButtonComponent? ButtonComponent { get; set; }
    public string? NavigatePage { get; set; }
    public Dictionary<string, string>? NavigatePageRoutes { get; set; }
    public List<string>? AdditionalFields { get; set; }
    public string? FontawesomeIconClass { get; set; }
    public string? TenantId { get; set; }

    public CardComponent()
    {

    }

    public CardComponent(string title, string? body, string? imageSrc, int? mdClass = null, int? lgClass = null, int? xsClass = null, int? marginBottom = null, int? marginTop = null, int? marginLeft = null, int? marginRight = null, int? paddingBottom = null, int? paddingTop = null, int? paddingLeft = null, int? paddingRight = null, ButtonComponent? buttonComponent = null, string? thumbnailTmageSrc = null, string? videoSrc = null) : base(mdClass, lgClass, xsClass, marginBottom, marginTop, marginLeft, marginRight, paddingBottom, paddingTop, paddingLeft, paddingRight)
    {
        Title = title;
        Body = body;
        ImageSrc = imageSrc;
        ButtonComponent = buttonComponent;
        ThumbnailImageSrc = thumbnailTmageSrc;
        VideoSrc = videoSrc;
    }

    public CardComponent(string title, string body, string? imageSrc, List<string>? lightBoxImages = null, int? mdClass = null, int? lgClass = null, int? xsClass = null, decimal? animationDelay = null, int? marginBottom = null, int? marginBottomLg = null, string? animationStart = "onScroll") : base(mdClass, lgClass, xsClass, marginBottom, null, null, null, null, null, null, null, marginBottomLg)
    {
        Title = title;
        ImageSrc = imageSrc;
        Body = body;
        LightboxImages = lightBoxImages;
        AnimationDelay = animationDelay;
        AnimationStart = animationStart;
    }

    public CardComponent(string title, string? body, string? imageSrc, int? mdClass = null, int? lgClass = null, int? xsClass = null, decimal? animationDelay = null, ButtonComponent? buttonComponent = null, int? marginBottom = null, int? marginBottomLg = null) : base(mdClass, lgClass, xsClass, marginBottom, null, null, null, null, null, null, null, marginBottomLg)
    {
        Title = title;
        Body = body;
        ImageSrc = imageSrc;
        ButtonComponent = buttonComponent;
        AnimationDelay = animationDelay;
    }

    public CardComponent(string title, string? body, string? imageSrc, string? thumbnailTmageSrc, int? mdClass = null, int? lgClass = null, int? xsClass = null, decimal? animationDelay = null, ButtonComponent? buttonComponent = null, int? marginBottom = null, int? marginBottomLg = null) : base(mdClass, lgClass, xsClass, marginBottom, null, null, null, null, null, null, null, marginBottomLg)
    {
        Title = title;
        Body = body;
        ImageSrc = imageSrc;
        ButtonComponent = buttonComponent;
        AnimationDelay = animationDelay;
        ThumbnailImageSrc = thumbnailTmageSrc ?? imageSrc;
    }

    public CardComponent(string title, string body, string imageSrc, int? mdClass = null, int? lgClass = null, int? xsClass = null, decimal? animationDelay = null, int? marginBottom = null, int? marginTop = null, ButtonComponent buttonComponent = null, string? animationStart = "onScroll") : base(mdClass, lgClass, xsClass, marginBottom, marginTop)
    {
        Title = title;
        Body = body;
        ImageSrc = imageSrc;
        AnimationDelay = animationDelay;
        ButtonComponent = buttonComponent;
        AnimationStart = animationStart;
    }

    public CardComponent(string title, string body, string imageSrc, string thumbnailTmageSrc, int? mdClass = null, int? lgClass = null, int? xsClass = null, decimal? animationDelay = null, int? marginBottom = null, int? marginTop = null, ButtonComponent buttonComponent = null, string? animationStart = "onScroll") : base(mdClass, lgClass, xsClass, marginBottom, marginTop)
    {
        Title = title;
        Body = body;
        ImageSrc = imageSrc;
        AnimationDelay = animationDelay;
        ButtonComponent = buttonComponent;
        AnimationStart = animationStart;
        ThumbnailImageSrc = thumbnailTmageSrc ?? imageSrc;
    }

    public CardComponent(string title, string body, string imageSrc, int? mdClass = null, int? lgClass = null, int? xsClass = null, decimal? animationDelay = null, int? marginBottom = null, int ? marginBottomLg = null, int? marginTop = null, ButtonComponent buttonComponent = null, string? animationStart = "onScroll") : base(mdClass, lgClass, xsClass, marginBottom, marginTop, null, null, null, null, null, null, marginBottomLg)
    {
        Title = title;
        Body = body;
        ImageSrc = imageSrc;
        AnimationDelay = animationDelay;
        ButtonComponent = buttonComponent;
        AnimationStart = animationStart;
    }

    public CardComponent(string title, string? body, string? imageSrc, string? thumbnailTmageSrc, int? mdClass = null, int? lgClass = null, int? xsClass = null, decimal? animationDelay = null, int? marginBottom = null, int? marginBottomLg = null, int? marginTop = null, ButtonComponent buttonComponent = null, string? animationStart = "onScroll") : base(mdClass, lgClass, xsClass, marginBottom, marginTop, null, null, null, null, null, null, marginBottomLg)
    {
        Title = title;
        Body = body;
        ImageSrc = imageSrc;
        AnimationDelay = animationDelay;
        ButtonComponent = buttonComponent;
        AnimationStart = animationStart;
        ThumbnailImageSrc = thumbnailTmageSrc ?? imageSrc;
    }

    public CardComponent(string imageSrc, ButtonComponent buttonComponent = null)
    {
        ImageSrc = imageSrc;
        ButtonComponent = buttonComponent;
    }

    public CardComponent(string title, string? body, ButtonComponent? buttonComponent = null)
    {
        Title = title;
        Body = body;
        ButtonComponent = buttonComponent;
    }

    public CardComponent(string title, string? body, string? imageSrc, string? thumbnailImageSrc)
    {
        Title = title;
        Body = body;
        ImageSrc = imageSrc;
        ThumbnailImageSrc = thumbnailImageSrc;
    }
}