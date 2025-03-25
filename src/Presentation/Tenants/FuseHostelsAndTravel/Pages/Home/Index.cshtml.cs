using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.Events.Queries;
using Travaloud.Application.Catalog.Images.Dto;
using Travaloud.Application.Common.Extensions;

namespace FuseHostelsAndTravel.Pages.Home;

public class IndexModel : TravaloudBasePageModel
{
    [BindProperty] public List<ContainerHalfImageTextComponent> HostelsContainers { get; private set; } = [];

    [BindProperty] public GenericBannerComponent? HostelsInVietnamBanner { get; private set; }

    [BindProperty] public ContainerHalfImageRoundedTextComponent? IntroductionBanner { get; private set; }

    [BindProperty] public AlternatingCardHeightComponent? ToursCards { get; private set; }

    [BindProperty] public CarouselCardsComponent? ToursCarousel { get; private set; }

    [BindProperty] public CarouselCardsComponent? EventsCards { get; private set; }

    [BindProperty] public FullImageCarouselComponent? CarouselComponent { get; private set; }

    [BindProperty] public OvalContainerComponent? MarqueeOvals { get; private set; }

    public override Guid? PageId()
    {
        return new Guid("db6b1983-9f8a-4b76-4409-08dc303eaa33");
    }

    public override string MetaKeywords(string? overrideValue = null)
    {
        return base.MetaKeywords( "FUSE hostels, FUSE tours, Hostel experience, Budget accommodation, Backpacker lodging, Vietnam travel experiences, Social spaces, Chill out spots, Hoi An Beachside, Hoi An Old Town, Nha Trang Beachside, Tailored tours, Bamboo boat tours, Bicycle tours, Paddleboarding tours, Jeep tours");
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return base.MetaDescription("Discover the ultimate hostel experience with FUSE Hostels & Tours. With two impressive locations opening in Hoi An in late 2022, our crew offers up everything travelers need to explore, kick back, and have fun in Vietnam. From our boutique-style beachside location with an ocean view and weekly pool parties to our prime location in Hoi An Old Town with weekly events, nightly BBQs, and surrounded by great street food and bars, we offer up the latest accommodation and travel experiences in Vietnam. Our tailored tours, including bamboo boat tours, bicycle tours, paddleboarding tours, and jeep tours, offer unique experiences for all our guests. Come discover the good vibes, safe environment, and friendly staff dedicated to making your stay unforgettable at FUSE Hostels & Tours.");
    }
    
    private readonly IEventsService _eventsService;

    public IndexModel(IEventsService eventsService)
    {
        _eventsService = eventsService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetDataAsync();

        MarqueeOvals = new OvalContainerComponent("parallaxBannerOvals", 20, null, -10, null);

        IntroductionBanner = new ContainerHalfImageRoundedTextComponent(
            new List<string>() {"3 Hostel Locations:", "Hoi An Beachside", "Hoi An Old Town", "Nha Trang"}, null,
            "Experience Vietnam as a backpacker with Fuse Hostels and Travel. Immerse yourself in the Hoi An Old Town, where timeless beauty meets vibrant culture. Wander through ancient streets and indulge in local delicacies while drinking ice cold beer. Prefer the beach? At our Hoi An beachside accommodation you can chill and enjoy cocktails & beer at sunset on the beach or at the pool. Ready for more excitement? Venture to Nha Trang, a coastal gem with stunning beaches and energetic nightlife. Unleash your wanderlust with Fuse Hostels and Travel - your gateway to Vietnam's captivating destinations.",
            "https://travaloudcdn.azureedge.net/fuse/assets/images/OT soft opening _ more (40 of 154).jpg?w=600",
            new ButtonComponent("/Properties/Index", "View Locations"), new List<OvalContainerComponent>()
            {
                new("homePageIntroductionOvals1", -40, null, null, -14),
                new("homePageIntroductionOvals2", null, -7, null, 36),
                new("homePageIntroductionOvals3", null, -30, null, -19)
            }, videoSrc: "https://travaloud.azureedge.net/fuse/assets/images/0b47297a-3910-4e83-8f83-a94e35996c09.mp4");

        HostelsInVietnamBanner = new GenericBannerComponent(
            "HOSTELS IN VIETNAM",
            "Check out what our backpacker hostels have to offer below.",
            new List<OvalContainerComponent>()
            {
                new("homePageHostelsOvals", -70, null, -20, null)
            }
        );

        var eventsRequest = _eventsService.SearchAsync(new SearchEventsRequest()
        {
            PageNumber = 1,
            PageSize = 99999
        });
        
        var hostelContainers = WebComponentsBuilder.FuseHostelsAndTravel.GetHostelsContainersAsync(Properties);
        var toursCards = WebComponentsBuilder.FuseHostelsAndTravel.GetToursCardsAsync(Tours);
        var toursCarousel = WebComponentsBuilder.FuseHostelsAndTravel.GetToursCarouselCardsAsync(Tours);
        var carouselImages = Task.Run(GetHomePageCarouselImages);
        
        await Task.WhenAll(hostelContainers, toursCards, toursCarousel, eventsRequest, carouselImages);

        var eventsCards = await WebComponentsBuilder.FuseHostelsAndTravel.GetEventsCarouselCardsAsync(eventsRequest.Result.Data);
        
        HostelsContainers = hostelContainers.Result;
        ToursCards = toursCards.Result;
        ToursCarousel = toursCarousel.Result;
        EventsCards = eventsCards;
        CarouselComponent = new FullImageCarouselComponent(carouselImages.Result, new BookNowComponent(Properties, null, true));

        return Page();
    }

    public Task<List<ImageDto>> GetHomePageCarouselImages()
    {
        var (title, subTitle, subSubTitle) = PageDetails.GetTitleSubTitleAndSubSubTitle(
            "FUSE",
            "COME TOGETHER AT", 
            "HOSTELS AND TRAVEL");
        
        return Task.FromResult(new List<ImageDto>()
        {
            new() {
                //ImagePath = "https://travaloudcdn.azureedge.net/fuse/assets/images/home-page-banner-1.webp?w=2000",
                //ThumbnailImagePath = "https://travaloudcdn.azureedge.net/fuse/assets/images/home-page-banner-1.webp?w=2000",
                ImagePath =
                    "https://travaloudcdn.azureedge.net/fuse/assets/images/POOLPARTY_23-03-22-12-min.jpg?w=2000",
                ThumbnailImagePath =
                    "https://travaloudcdn.azureedge.net/fuse/assets/images/POOLPARTY_23-03-22-08-min.jpg?w=2000",
                Title = title,
                SubTitle1 = subTitle,
                SubTitle2 = subSubTitle
            }
        });
    }
}