namespace Travaloud.Tenants.SharedRCL.Models.WebComponents.FuseHostelsAndTravel;

public class ContainerHalfImageTextComponent : CardComponent
{
    public bool SwapDirection { get; private set; }
    public OvalContainerComponent? OvalContainer { get; private set; }

    public ContainerHalfImageTextComponent()
    {

    }

    public ContainerHalfImageTextComponent(string title, string? body, string? imageSrc, bool swapDirection = false, OvalContainerComponent? ovalContainer = null, ButtonComponent? buttonComponent = null, int? mdClass = null, int? lgClass = null, int? xsClass = null, int? marginBottom = null, int? marginTop = null, int? marginLeft = null, int? marginRight = null, int? paddingBottom = null, int? paddingTop = null, int? paddingLeft = null, int? paddingRight = null) : base(title, body, imageSrc, mdClass, lgClass, xsClass, marginBottom, marginTop, marginLeft, marginRight, paddingBottom, paddingTop, paddingLeft, paddingRight, buttonComponent)
    {
        SwapDirection = swapDirection;
        OvalContainer = ovalContainer;
    }

    public ContainerHalfImageTextComponent(string title, string? body, string? imageSrc, string? thumbnailImageSrc, bool swapDirection = false, OvalContainerComponent? ovalContainer = null, ButtonComponent? buttonComponent = null, int? mdClass = null, int? lgClass = null, int? xsClass = null, int? marginBottom = null, int? marginTop = null, int? marginLeft = null, int? marginRight = null, int? paddingBottom = null, int? paddingTop = null, int? paddingLeft = null, int? paddingRight = null) : base(title, body, imageSrc, mdClass, lgClass, xsClass, marginBottom, marginTop, marginLeft, marginRight, paddingBottom, paddingTop, paddingLeft, paddingRight, buttonComponent, thumbnailImageSrc)
    {
        SwapDirection = swapDirection;
        OvalContainer = ovalContainer;
    }
}