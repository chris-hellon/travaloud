namespace Travaloud.Tenants.SharedRCL.Models.WebComponents.FuseHostelsAndTravel;

public class CarouselCardsComponent : AlternatingCardHeightComponent
{
    public string PartialView { get; private set; }
    public int? HeaderPaddingTop { get; set; }
    public int? HeaderPaddingBottom { get; set; }
    
    public CarouselCardsComponent()
    {

    }

    public CarouselCardsComponent(GenericBannerComponent? header, List<CardComponent>? cards = null, string? animationStart = "onScroll", string partialView = "_CardPartial") : base(header, cards, animationStart)
    {
        PartialView = partialView;
    }
}