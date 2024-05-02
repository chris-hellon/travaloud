using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.Events.Queries;
using Travaloud.Application.Catalog.Images.Dto;

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
        return Task.FromResult(new List<ImageDto>()
        {
            new() {
                //ImagePath = "https://travaloudcdn.azureedge.net/fuse/assets/images/home-page-banner-1.webp?w=2000",
                //ThumbnailImagePath = "https://travaloudcdn.azureedge.net/fuse/assets/images/home-page-banner-1.webp?w=2000",
                ImagePath =
                    "https://travaloudcdn.azureedge.net/fuse/assets/images/POOLPARTY_23-03-22-12.jpg?w=2000",
                ThumbnailImagePath =
                    "https://travaloudcdn.azureedge.net/fuse/assets/images/POOLPARTY_23-03-22-08.jpg?w=2000",
                Title = "FUSE",
                SubTitle1 = "COME TOGETHER AT",
                SubTitle2 = "HOSTELS AND TRAVEL"
            }
        });
    }
}