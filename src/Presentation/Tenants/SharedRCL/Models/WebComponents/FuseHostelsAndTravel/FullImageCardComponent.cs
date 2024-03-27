namespace Travaloud.Tenants.SharedRCL.Models.WebComponents.FuseHostelsAndTravel;

public class FullImageCardComponent
{
    public GenericBannerComponent Header { get; private set; }
    public List<CardComponent> Cards { get; private set; }
    public string? AnimationStart { get; private set; }
    public IEnumerable<OvalContainerComponent>? OvalContainers { get; private set; }

    public FullImageCardComponent()
    {

    }

    public FullImageCardComponent(GenericBannerComponent header, List<CardComponent>? cards = null, string? animationStart = "onScroll", IEnumerable<OvalContainerComponent>? ovalContainers = null)
    {
        Cards = cards ?? new List<CardComponent>();
        Header = header;
        AnimationStart = animationStart;
        OvalContainers = ovalContainers;
    }
}