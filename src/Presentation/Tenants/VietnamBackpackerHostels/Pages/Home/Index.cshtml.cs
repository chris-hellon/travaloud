using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Catalog.Images.Dto;

namespace VietnamBackpackerHostels.Pages.Home;

public class IndexModel : TravaloudBasePageModel
{
    [BindProperty]
    public FullImageCarouselComponent CarouselComponent { get; private set; }

    [BindProperty]
    public FullImageCardsComponent ToursCards { get; private set; }

    [BindProperty]
    public GenericCardsComponent PromotedToursCards { get; private set; }

    [BindProperty]
    public FullImageCardsComponent DestinationsCards { get; private set; }

    [BindProperty]
    public CarouselCardsComponent ToursCarousel { get; private set; }

    [BindProperty]
    public CarouselCardsComponent ServicesCarousel { get; private set; }

    public override List<Guid> PromotedToursIds()
    {
        return new List<Guid>() {
            new Guid("4BCCB2F8-B479-41F9-3A60-08DB5D9B92D2"),
            new Guid("9110E990-3DB4-4EFD-39B2-08DB61B176FE"),
            new Guid("CDA60752-3C10-4E5E-9F89-57DCA5DA9020")
            //new Guid("CDA60752-3C10-4E5E-9F89-57DCA5DA9020"),
            //new Guid("732A1761-4870-4797-92F0-956E53B744C0"),
            //new Guid("106E5F27-19A7-46DE-8A7C-D8B1A7BEF3F6"),
            //new Guid("7DFDF106-6F6E-4BBF-8DE7-D4B0F8A9B80D"),
            //new Guid("DAB38E23-4CD3-4299-86B4-02B3F7440BE7"),
            //new Guid("C0DA1FB6-4620-466F-89B0-7AF6E3ED0FD6"),
            //new Guid("E07C4685-AB93-4450-8FE2-AEDCAA221F2C"),
            //new Guid("9FF6B3B7-839E-40D2-916A-2F60C5728ABD")
        };
    }

    public async Task<IActionResult> OnGet()
    {
        await OnGetDataAsync();
        
        CarouselComponent = new FullImageCarouselComponent(await GetHomePageCarouselImages(), new BookNowComponent(Properties, null, true), true);
        ToursCards = WebComponentsBuilder.GetToursWithCategoriesFullImageCards(TenantId, ToursWithCategories.Where(x => !x.GroupParentCategoryId.HasValue), null, null, false);
        ToursCarousel = WebComponentsBuilder.VietnamBackpackerHostels.GetToursWithCategoriesCarouselCards(PromotedTours);
        DestinationsCards = WebComponentsBuilder.VietnamBackpackerHostels.GetDestinationsCards(Destinations);
        ServicesCarousel = WebComponentsBuilder.VietnamBackpackerHostels.GetServicesCarouselCards(Services);
        
        
        return Page();
    }
    
            public async Task<List<ImageDto>> GetHomePageCarouselImages()
        {
            var images = new List<ImageDto>();

            await Task.Run(() =>
            {
                images = new List<ImageDto>()
                {
                    //new ImageDto()
                    //{
                    //    VideoPath = "https://travaloud.blob.core.windows.net/vbh/assets/images/2f419f45-f2d3-4038-b74d-e7e552eeb7cf.mp4",
                    //    Html = @"<div class=""col-lg-3 pe-2 text-center text-white d-lg-block d-none"">
                    //                <h5 class=""text-white mb-4 py-2"" style=""border-right:1px solid white; border-left:1px solid white;"">2DN1<br>HA LONG BAY CRUISE</h5>
                    //                <div>
                    //                    <h5 class=""text-white d-inline-block me-2"">WAS</h5>
                    //                    <span class=""text-white d-inline-block header-font display-6 strikethrough-text"">$400</h3>
                    //                </div>
                    //                <div>
                    //                    <h5 class=""text-white d-inline-block me-2"">NOW</h5>
                    //                    <span class=""text-white d-inline-block header-font display-6"">$299</h3>
                    //                </div>
                    //            </div>
                    //            <div class=""col-lg-6 text-center text-white"">
                    //                <img class=""img-fluid mb-5 mb-lg-3 d-none d-lg-inline-block"" style=""height:150px;"" src=""https://travaloudcdn.azureedge.net/vbh/assets/images/vbh-logo-full.png?w=500""/>
                    //                <h4 class=""mb-4 text-white"">Special Offer</h4>
                    //                <h1 class=""display-1 lh-1 m-0 p-0 fs-1 mt-n4 text-white"">Flash</h1>
                    //                <h1 class=""display-1 lh-1 m-0 p-0 fs-1 mt-n3 text-gold"">Sale</h1>
                    //                <h4 class=""mb-4 text-white"">Ha Long Bay Cruise</h4>
                    //                <div class=""d-lg-none d-block"">
                    //                    <h5 class=""text-white d-inline-block me-2"">WAS</h5>
                    //                    <span class=""text-white d-inline-block header-font display-6 strikethrough-text"">$400</h3>
                    //                </div>
                    //                <div class=""d-lg-none d-block mb-3 mt-n3"">
                    //                    <h5 class=""text-white d-inline-block me-2"">NOW</h5>
                    //                    <span class=""text-white d-inline-block header-font display-6"">$299</h3>
                    //                </div>
                    //                <a href=""/explore/tour/ha-long-bay-deluxe-cruise-2d1n"" class=""btn btn-outline-white"" data-mdb-ripple-color=""dark"">Book Now</a>
                    //            </div>
                    //            <div class=""col-lg-3 ps-2 text-center text-white d-lg-block d-none"">
                    //                <div class=""mb-4 d-flex justify-content-center align-items-center"" style=""border-right:1px solid white; border-left:1px solid white;"">
                    //                    <span class=""text-white header-font display-6 mb-0 d-inline-block"">+ $10</h3>
                    //                </div>
                    //                <h5 class=""text-white"">OFF ANY <br>TOUR IN HANOI</h5>
                    //            </div>"
                    //},
                    new ImageDto()
                    {
                        ImagePath = "https://travaloudcdn.azureedge.net/vbh/assets/images/home-5.webp?w=2000",
                        Html = "<div class=\"text-center text-white\"><h1 class=\"display-1 lh-1 m-0 p-0 fs-1 text-white\">Vietnam</h1><h2 class=\"mb-0 mt-n3 fs-4 text-white\">Backpacker Hostels</h2><small class=\"text-end mt-n2 me-1 mb-4 header-font d-block\">est. 2004</small> <h4 class=\"mb-4 text-white\">#MORETHANJUSTABED</h4></div>"
                    },
                    new ImageDto()
                    {
                        ImagePath = "https://travaloudcdn.azureedge.net/vbh/assets/images/home-8.webp?w=2000",
                        Html = "<div class=\"text-center text-white\"><h1 class=\"display-1 lh-1 m-0 p-0 fs-1 text-white\">Vietnam</h1><h2 class=\"mb-0 mt-n3 fs-4 text-white\">Backpacker Hostels</h2><small class=\"text-end mt-n2 me-1 mb-4 header-font d-block\">est. 2004</small> <h4 class=\"mb-4 text-white\">#MORETHANJUSTABED</h4></div>"
                    },
                    new ImageDto()
                    {
                        ImagePath = "https://travaloudcdn.azureedge.net/vbh/assets/images/hoi-an-banner-3.webp?w=2000",
                        Html = "<div class=\"text-center text-white\"><h1 class=\"display-1 lh-1 m-0 p-0 fs-1 text-white\">Vietnam</h1><h2 class=\"mb-0 mt-n3 fs-4 text-white\">Backpacker Hostels</h2><small class=\"text-end mt-n2 me-1 mb-4 header-font d-block\">est. 2004</small> <h4 class=\"mb-4 text-white\">#MORETHANJUSTABED</h4></div>"
                    },
                    new ImageDto()
                    {
                        ImagePath = "https://travaloudcdn.azureedge.net/vbh/assets/images/hue-banner-2.webp?w=2000",
                        Html = "<div class=\"text-center text-white\"><h1 class=\"display-1 lh-1 m-0 p-0 fs-1 text-white\">Vietnam</h1><h2 class=\"mb-0 mt-n3 fs-4 text-white\">Backpacker Hostels</h2><small class=\"text-end mt-n2 me-1 mb-4 header-font d-block\">est. 2004</small> <h4 class=\"mb-4 text-white\">#MORETHANJUSTABED</h4></div>"
                    }
                };
            });

            return images;
        }
}