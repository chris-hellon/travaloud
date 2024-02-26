namespace Travaloud.Tenants.SharedRCL.Models.WebComponents.FuseHostelsAndTravel;

public class AlternatingCardHeightComponent
{
    public GenericBannerComponent? Header { get; private set; }
    public List<CardComponent> Cards { get; private set; }
    public string? AnimationStart { get; private set; }

    public AlternatingCardHeightComponent()
    {

    }

    public AlternatingCardHeightComponent(GenericBannerComponent? header, List<CardComponent>? cards = null, string? animationStart = "onScroll")
    {
        Cards = cards ?? new List<CardComponent>();
        Header = header;
        AnimationStart = animationStart;
    }
}