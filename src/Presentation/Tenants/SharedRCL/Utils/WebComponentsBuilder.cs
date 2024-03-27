using System.Globalization;
using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Catalog.Events.DTO;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Services.Dto;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Tenants.SharedRCL.Utils;

public static class WebComponentsBuilder
{
    public static class FuseHostelsAndTravel
    {
        public static async Task<List<ContainerHalfImageTextComponent>> GetHostelsContainersAsync(
            IEnumerable<PropertyDto>? properties)
        {
            return await Task.Run(() => GetHostelsContainers(properties));
        }

        private static List<ContainerHalfImageTextComponent> GetHostelsContainers(IEnumerable<PropertyDto>? properties)
        {
            var hostelsContainers = new List<ContainerHalfImageTextComponent>();
            var swapDirection = true;

            if (properties == null) return hostelsContainers;

            var propertyDtos = properties as PropertyDto[] ?? properties.ToArray();
            foreach (var item in propertyDtos)
            {
                hostelsContainers.Add(new ContainerHalfImageTextComponent(
                    item.Name + " Hostel",
                    item.ShortDescription,
                    item.ImagePath,
                    item.ThumbnailImagePath,
                    swapDirection,
                    swapDirection
                        ? new OvalContainerComponent(item.Name.ConvertToCamelCase("Ovals"), null, -30, null, -25)
                        : null,
                    new ButtonComponent($"/Property/Index?propertyName={item.FriendlyUrl}",
                        $"View {item.Name} Hostel"), null, null, null, null, null, null, null,
                    item == propertyDtos.Last() ? 0 : 5)
                {
                    PaddingBottomLg = 2
                });

                swapDirection = !swapDirection;
            }

            return hostelsContainers;
        }

        public static async Task<AlternatingCardHeightComponent> GetToursCardsAsync(IEnumerable<TourDto>? tours,
            string? animationStart = "onScroll", string title = "TAILORED TOURS",
            string body =
                "Whether you're hitting up the Hoi An Old Town Hostel, chilling at the Hoi An Beachside Hostel, or soaking in the vibes at our Nha Trang Hostel, we've got your adventure covered.<br /><br />Our crew knows the ins and outs of this beautiful country like nobody else. They're like your personal tour guides, making sure you have the time of your life. From exploring ancient streets in Hoi An to lounging by the beach in Nha Trang, we've got tailored experiences that'll blow your mind.")
        {
            return await Task.Run(() => GetToursCards(tours, animationStart, title, body));
        }

        private static AlternatingCardHeightComponent GetToursCards(IEnumerable<TourDto>? tours,
            string? animationStart = "onScroll", string title = "TAILORED TOURS",
            string body =
                "Explore the breathtaking culture of Vietnam with FUSE Travel. Our experienced travel crew will help you get the most out of your time in this beautiful country, with unique tailored experiences for all our guests. Check out some of our offerings below, or ask any of our super-friendly crew for their recommendations.")
        {
            var toursCards =
                new AlternatingCardHeightComponent(new GenericBannerComponent(title, body), null, animationStart);

            var animationDelay = 200M;
            if (tours == null) return toursCards;

            var tourDtos = tours as TourDto[] ?? tours.ToArray();
            foreach (var item in tourDtos)
            {
                toursCards.Cards.Add(new CardComponent(
                    item.Name,
                    item.ShortDescription,
                    item.ImagePath,
                    item.ThumbnailImagePath,
                    12, 6, 12,
                    animationDelay,
                    item != tourDtos.Last() ? 5 : null,
                    item != tourDtos.Last() ? 8 : null,
                    null,
                    new ButtonComponent("btn-outline-primary", $"/Tour/Index?tourName={item.FriendlyUrl}",
                        "Find Out More"), animationStart));

                animationDelay += 200M;
            }

            return toursCards;
        }

        public static async Task<CarouselCardsComponent> GetToursCarouselCardsAsync(IEnumerable<TourDto>? tours,
            string? animationStart = "onScroll", string title = "TAILORED TOURS",
            string? body =
                "Explore the breathtaking culture of Vietnam with FUSE travel. Our experienced travel crew will help you get the most out of your time and FUSE Travel offers up unique tailored experiences for all our guests.")
        {
            return await Task.Run(() => GetToursCarouselCards(tours, animationStart, title));
        }

        public static CarouselCardsComponent GetToursCarouselCards(IEnumerable<TourDto>? tours,
            string? animationStart = "onScroll", string title = "TAILORED TOURS",
            string body =
                "Explore the breathtaking culture of Vietnam with FUSE travel. Our experienced travel crew will help you get the most out of your time and FUSE Travel offers up unique tailored experiences for all our guests.")
        {
            var toursCards = new CarouselCardsComponent(new GenericBannerComponent(title, body), null, animationStart);

            if (tours == null) return toursCards;

            foreach (var item in tours)
            {
                toursCards.Cards.Add(new CardComponent(
                    item.Name,
                    item.ShortDescription,
                    item.ImagePath,
                    item.ThumbnailImagePath,
                    null, null, null,
                    null,
                    null,
                    null,
                    null,
                    new ButtonComponent("btn-outline-primary align-bottom",
                        $"/Tour/Index?tourName={item.FriendlyUrl}", "Find Out More")));
            }

            return toursCards;
        }

        public static async Task<CarouselCardsComponent> GetEventsCarouselCardsAsync(IEnumerable<EventDto> events)
        {
            return await Task.Run(() => GetEventsCarouselCards(events));
        }

        private static CarouselCardsComponent GetEventsCarouselCards(IEnumerable<EventDto> events)
        {
            var eventsCards = new CarouselCardsComponent(null, null, null, "_CardWithBackgroundPartial");

            foreach (var item in events)
            {
                eventsCards.Cards.Add(new CardWithBackgroundComponent(item.Name,
                    item.ShortDescription,
                    item.BackgroundColor,
                    item.ImagePath,
                    null, null, null, null, null, null, new List<OvalContainerComponent>()
                    {
                        new($"eventsCards{item.Id}OvalsContainer1", -60, null, -110, null),
                        new($"eventsCards{item.Id}OvalsContainer2", null, -60, null, -100)
                    }));
            }

            return eventsCards;
        }

        public static async Task<FullImageCardComponent> GetHostelAccommodationCardsAsync(
            IEnumerable<PropertyRoomDto> propertyRooms, string? animationStart = "onScroll",
            string title = "ACCOMMODATION",
            string body =
                "All private & shared rooms at FUSE Beachside come with pool or ocean views as standard so there is no better place to stay if beach vibes are your thing.")
        {
            return await Task.Run(() => GetHostelAccommodationCards(propertyRooms, animationStart, title, body));
        }

        private static FullImageCardComponent GetHostelAccommodationCards(IEnumerable<PropertyRoomDto> propertyRooms,
            string? animationStart = "onScroll", string title = "ACCOMMODATION",
            string body =
                "All private & shared rooms at FUSE Beachside come with pool or ocean views as standard so there is no better place to stay if beach vibes are your thing.")
        {
            var accommodationCards = new FullImageCardComponent(new GenericBannerComponent(title, body,
                new List<OvalContainerComponent>()
                {
                    new("hostelAccommodationOvals1", -185, null, -40, null)
                }), null, animationStart);
            var animationDelay = 200M;

            var propertyRoomDtos = propertyRooms as PropertyRoomDto[] ?? propertyRooms.ToArray();
            for (var i = 0; i < propertyRoomDtos.ToList().Count; i++)
            {
                var item = propertyRoomDtos.ToList()[i];
                accommodationCards.Cards.Add(new CardComponent(
                    item.Name,
                    item.Description,
                    item.ImagePath,
                    new List<string> {item.ImagePath ?? string.Empty},
                    12, 6, 12,
                    animationDelay, 8, 10,
                    animationStart));

                animationDelay += 200M;
            }

            return accommodationCards;
        }

        public static async Task<NavPillsComponent> GetHostelDirectionsNavPillsAsync(PropertyDetailsDto property)
        {
            return await Task.Run(() => GetHostelDirectionsNavPills(property));
        }

        private static NavPillsComponent GetHostelDirectionsNavPills(PropertyDetailsDto property)
        {
            return new NavPillsComponent(
                $"GETTING TO {property.PageSubTitle?.ToUpper()} {property.PageTitle?.ToUpper()}",
                (property.Directions ?? new List<PropertyDirectionDto>()).Select(x =>
                    new NavPill(x.Title, x.Content.Select(c => new NavPillContent(c.Body, c.Style ?? string.Empty)).ToList()))
                .ToList());
        }

        public static async Task<FeaturesTableComponent?> GetHostelFacilitiesAsync(PropertyDetailsDto property)
        {
            return await Task.Run(() => GetHostelFacilities(property));
        }

        public static FeaturesTableComponent? GetHostelFacilities(PropertyDetailsDto property)
        {
            return property.Facilities != null ? new FeaturesTableComponent("Facilities & Amenities", property.Facilities) : null;
        }
    }

    public static class UncutTravel
    {
        public static FullImageCardsComponent GetDestinationsCards(IEnumerable<DestinationDto> destinations)
        {
            return new FullImageCardsComponent("Destinations", destinations.Select(x =>
                new CardComponent(x.Name, x.Description, x.ImagePath, x.ThumbnailImagePath)
                {
                    LgClass = 4,
                    MdClass = 12,
                    MarginBottom = 4,
                    NavigatePage = "/Destination/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"destinationName", x.FriendlyUrl}},
                    TenantId = "uncut"
                }).ToList());
        }

        public static PageCategoryComponent GetDestinationsCategoryPage(IEnumerable<DestinationDto> destinations)
        {
            return new PageCategoryComponent("Destinations",
                "No matter where it is you arrive, Uncut has got you covered...", new GenericCardsComponent(null,
                    destinations.Select(x =>
                        new CardComponent(x.Name, x.ShortDescription, x.ImagePath, x.ThumbnailImagePath)
                        {
                            LgClass = 4,
                            MdClass = 12,
                            MarginBottom = 4,
                            NavigatePage = "/Destination/Index",
                            NavigatePageRoutes = new Dictionary<string, string>() {{"destinationName", x.FriendlyUrl}},
                            TenantId = "uncut"
                        }).ToList()), false);
        }

        public static PageCategoryComponent GetToursWithCategoriesPageCategoryComponent(
            IEnumerable<TourWithCategoryDto> toursWithCategories, TourWithCategoryDto? tourCategory = null,
            bool includeAll = false)
        {
            return new PageCategoryComponent(tourCategory != null ? tourCategory.Name : "Tours & Activities",
                "No matter where it is you arrive, Uncut has got you covered...",
                new GenericCardsComponent(null,
                    GetToursWithCategoriesCards("uncut", toursWithCategories, tourCategory, includeAll)), false);
        }


        public static GenericCardsComponent GetPromotedToursWithCategoriesGenericCards(List<TourWithCategoryDto> tours)
        {
            return new GenericCardsComponent(null, GetPromotedToursCards(tours));
        }

        public static FullImageCardsComponent GetPromotedToursFullImageCards(List<TourWithCategoryDto> tours)
        {
            return new FullImageCardsComponent("Tours & Activities", GetPromotedToursCards(tours));
        }

        public static GenericCardsComponent GetPromotedToursGenericImageCards(List<TourWithCategoryDto> tours)
        {
            return new GenericCardsComponent("Tours & Activities", GetPromotedToursCards(tours));
        }

        public static GenericCardsComponent GetDestinationsWithCategoriesGenericCards(
            IEnumerable<DestinationDto> destinations, string? title = "More Like This")
        {
            return new GenericCardsComponent(title, destinations.Select(x =>
                new CardComponent(x.Name, x.ShortDescription, x.ImagePath, x.ThumbnailImagePath)
                {
                    LgClass = 4,
                    MdClass = 12,
                    MarginBottom = 4,
                    NavigatePage = "/Destination/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"destinationName", x.FriendlyUrl}},
                    TenantId = "uncut"
                }).ToList(), true);
        }

        public static GenericCardsComponent GetToursGenericCards(IEnumerable<TourDetailsDto> tours,
            string? title = "More Like This", bool includeTimeFrame = false)
        {
            return new GenericCardsComponent(title, tours.Select(x =>
                new CardComponent(x.Name, x.ShortDescription, x.ImagePath, x.ThumbnailImagePath)
                {
                    LgClass = 4,
                    MdClass = 12,
                    MarginBottom = 4,
                    NavigatePage = "/Tour/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"tourName", x.FriendlyUrl}},
                    AdditionalFields = GetTourDetails(x, includeTimeFrame),
                    TenantId = "uncut"
                }).ToList(), true);
        }

        public static GenericCardsComponent GetServicesGenericCards(IEnumerable<ServiceDto> destinations,
            string? title = "More Like This")
        {
            return new GenericCardsComponent(title, destinations.Select(x =>
                new CardComponent(x.Title, x.ShortDescription)
                {
                    LgClass = 4,
                    MdClass = 12,
                    MarginBottom = 4,
                    NavigatePage = "/Service/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"serviceName", x.FriendlyUrl}},
                    FontawesomeIconClass = x.IconClass,
                    TenantId = "uncut"
                }).ToList(), true);
        }

        private static List<CardComponent> GetPromotedToursCards(IEnumerable<TourWithCategoryDto> tours)
        {
            return tours.Select(x => new CardComponent(x.Name, x.ShortDescription, x.ImagePath, x.ThumbnailImagePath)
            {
                LgClass = 4,
                MdClass = 12,
                MarginBottom = 4,
                NavigatePage = "/Tour/Index",
                NavigatePageRoutes = new Dictionary<string, string>() {{"tourName", x.FriendlyUrl}},
                AdditionalFields = GetTourWithCategoryDetails(x, true, true),
                TenantId = "uncut"
            }).ToList();
        }
    }

    public static class VietnamBackpackerHostels
    {
        public static async Task<CarouselCardsComponent> GetToursCarouselCardsAsync(IEnumerable<TourDetailsDto> tours,
            string? animationStart = "onScroll", string title = "Upcoming Trips",
            string body =
                "Discover the hidden gems of Vietnam from North to South with our upcoming tours - explore the vibrant streets of Hanoi, soak up the rich history and stunning landscapes of Central Vietnam, and indulge in the ultimate backpacking adventure with Vietnam Backpacker Hostels!")
        {
            return await Task.Run(() => GetToursCarouselCards(tours, animationStart, title));
        }

        private static CarouselCardsComponent GetToursCarouselCards(IEnumerable<TourDetailsDto> tours,
            string? animationStart = "onScroll", string title = "Upcoming Trips",
            string body =
                "Discover the hidden gems of Vietnam from North to South with our upcoming tours - explore the vibrant streets of Hanoi, soak up the rich history and stunning landscapes of Central Vietnam, and indulge in the ultimate backpacking adventure with Vietnam Backpacker Hostels!")
        {
            var toursCards = new CarouselCardsComponent(new GenericBannerComponent(title, body), null, animationStart,
                "_GenericCardPartial");

            foreach (var item in tours)
            {
                toursCards.Cards.Add(new CardComponent(
                    item.Name,
                    item.ShortDescription,
                    item.ImagePath,
                    item.ThumbnailImagePath, 12, 12, 12, null, null, 2, 2)
                {
                    AdditionalFields = GetTourDetails(item),
                    NavigatePage = "/Tour/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"tourName", item.FriendlyUrl}},
                    TenantId = "vbh"
                });
            }

            return toursCards;
        }

        public static CarouselCardsComponent GetToursWithCategoriesCarouselCards(IEnumerable<TourWithCategoryDto> tours,
            string? animationStart = "onScroll", string title = "Upcoming Trips",
            string body =
                "Discover the hidden gems of Vietnam from North to South with our upcoming tours - explore the vibrant streets of Hanoi, soak up the rich history and stunning landscapes of Central Vietnam, and indulge in the ultimate backpacking adventure with Vietnam Backpacker Hostels!")
        {
            var toursCards = new CarouselCardsComponent(new GenericBannerComponent(title, body), null, animationStart,
                "_GenericCardPartial");

            foreach (var item in tours)
            {
                toursCards.Cards.Add(new CardComponent(
                    item.Name,
                    item.ShortDescription,
                    item.ImagePath,
                    item.ThumbnailImagePath, 12, 12, 12, null, null, 2, 2)
                {
                    AdditionalFields = GetTourWithCategoryDetails(item),
                    NavigatePage = "/Tour/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"tourName", item.FriendlyUrl}},
                    TenantId = "vbh"
                });
            }

            return toursCards;
        }

        public static CarouselCardsComponent GetServicesCarouselCards(IEnumerable<ServiceDto> services,
            string? animationStart = "onScroll", string title = "SERVICES",
            string body =
                "At Vietnam Backpacker Hostels, we offer a range of services to make your backpacking trip in Vietnam as easy and enjoyable as possible. Ranging from airport transfers to volunteer programs, let our friendly staff guide you through the best of what Vietnam has to offer. We are here to help you experience the country's rich culture and natural beauty, while ensuring that every aspect of your trip is taken care of.")
        {
            var toursCards = new CarouselCardsComponent(new GenericBannerComponent(title, body), null, animationStart,
                "_GenericCardPartial");

            foreach (var item in services)
            {
                toursCards.Cards.Add(new CardComponent(
                    item.Title,
                    item.ShortDescription,
                    null,
                    12, 12, 12, null, null, 2, 2)
                {
                    NavigatePage = "/Service/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"serviceName", item.FriendlyUrl}},
                    FontawesomeIconClass = item.IconClass,
                    TenantId = "vbh"
                });
            }

            return toursCards;
        }

        public static FullImageCardsComponent GetDestinationsCards(IEnumerable<DestinationDto> destinations,
            string title = "Hostels")
        {
            return new FullImageCardsComponent(title, destinations.Select(x =>
                new CardComponent(x.Name, x.Description, x.ImagePath, x.ThumbnailImagePath)
                {
                    LgClass = 4,
                    MdClass = 12,
                    MarginBottom = 4,
                    NavigatePage = "/Destination/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"destinationName", x.FriendlyUrl}},
                    TenantId = "vbh"
                }).ToList());
        }

        private static List<CardComponent> GetToursWithCategoriesCards(
            IEnumerable<TourWithCategoryDto> toursWithCategories, TourWithCategoryDto? tourCategory = null,
            bool includeAll = false)
        {
            toursWithCategories = includeAll
                ? toursWithCategories
                :
                tourCategory != null
                    ?
                    toursWithCategories
                        .Where(x => x.CategoryId == tourCategory.Id || x.ParentTourCategoryId == tourCategory.Id)
                        .OrderBy(x => x.SortOrder)
                    : toursWithCategories.Where(x => x.CategoryId == null && x.ParentTourCategoryId == null)
                        .OrderBy(x => x.SortOrder);

            var tourWithCategoryDtos = toursWithCategories as TourWithCategoryDto[] ?? toursWithCategories.ToArray();
            if (tourWithCategoryDtos.Length != 0)
            {
                tourWithCategoryDtos.DistinctBy(x => x.Id);
            }

            return tourWithCategoryDtos.Select(x =>
                new CardComponent(x.Name, x.ShortDescription, x.ImagePath, x.ThumbnailImagePath)
                {
                    LgClass = 4,
                    MdClass = 12,
                    MarginBottom = 4,
                    NavigatePage = $"/{(x.IsCategory ? "TourCategory" : "Tour")}/Index",
                    NavigatePageRoutes = new Dictionary<string, string>()
                        {{$"{(x.IsCategory ? "tourCategoryName" : "tourName")}", x.FriendlyUrl}},
                    AdditionalFields = GetTourWithCategoryDetails(x),
                    TenantId = "vbh"
                }).ToList();
        }

        public static PageCategoryComponent GetDestinationsCategoryPage(IEnumerable<DestinationDto> destinations)
        {
            return new PageCategoryComponent("Hostels",
                "No matter where you are in Vietnam, VBH has got you covered...", new GenericCardsComponent(null,
                    destinations.Select(x =>
                        new CardComponent(x.Name, x.ShortDescription, x.ImagePath, x.ThumbnailImagePath)
                        {
                            LgClass = 4,
                            MdClass = 12,
                            MarginBottom = 4,
                            NavigatePage = "/Destination/Index",
                            NavigatePageRoutes = new Dictionary<string, string>() {{"destinationName", x.FriendlyUrl}},
                            TenantId = "vbh"
                        }).ToList()));
        }

        public static PageCategoryComponent GetToursWithCategoriesAndDestinationsPageCategoryComponent(
            IEnumerable<TourWithCategoryDto> toursWithCategories)
        {
            var toursCards = GetToursWithCategoriesCards(toursWithCategories, null, true);
            //var destinationsCards = destinations.Select(x => new CardComponent(x.Name, x.Description, x.ImagePath, x.ThumbnailImagePath)
            //{
            //    LgClass = 4,
            //    MdClass = 12,
            //    MarginBottom = 4,
            //    NavigatePage = "/Destination/Index",
            //    NavigatePageRoutes = new Dictionary<string, string>() { { "destinationName", x.FriendlyUrl } }
            //}).ToList();
            //var cards = toursCards;
            //cards.AddRange(destinationsCards);

            return new PageCategoryComponent("Trips & Travel", "Where to next?",
                new GenericCardsComponent(null, toursCards));
        }

        public static GenericCardsComponent GetDestinationsGenericCards(IEnumerable<DestinationDto> destinations)
        {
            return new GenericCardsComponent(null, destinations.Select(x =>
                new CardComponent(x.Name, x.Description, x.ImagePath, x.ThumbnailImagePath)
                {
                    LgClass = 4,
                    MdClass = 12,
                    MarginBottom = 4,
                    NavigatePage = "/Destination/Index",
                    NavigatePageRoutes = new Dictionary<string, string>() {{"destinationName", x.FriendlyUrl}},
                    TenantId = "vbh"
                }).ToList());
        }
    }

    private static List<string> GetTourDetails(TourDetailsDto tour, bool includeTimeFrame = false)
    {
        var details = new List<string>();

        //if (!string.IsNullOrEmpty(tour.Address))
        //    details.Add(new Tuple<string, string>("<i class='fas fa-location-pin text-white small'></i>", tour.Address));

        if (tour.TourPrices != null)
        {
            var price = tour.TourPrices.Aggregate("<strong class='d-block'>", (current, tourPrice) => current + $"${tourPrice.Price.ToString(CultureInfo.InvariantCulture)} / ");

            price = price.Trim();
            price = price.TrimEnd('/');
            price = price.Trim();

            price += "</strong>";
            details.Add(price);
        }

        var dayDurationExists = !string.IsNullOrEmpty(tour.DayDuration);
        var nightDurationExists = !string.IsNullOrEmpty(tour.NightDuration);
        var hourDurationExists = !string.IsNullOrEmpty(tour.HourDuration);

        if (dayDurationExists || nightDurationExists || hourDurationExists)
        {
            var duration = "";
            if (dayDurationExists)
                duration +=
                    $"{tour.DayDuration}{(includeTimeFrame ? $" Day{(tour.DayDuration != "1" ? "s" : "")}" : "")}";

            if (nightDurationExists)
                duration +=
                    $"{(dayDurationExists ? " " : "")}{tour.NightDuration}{(includeTimeFrame ? $" Night{(tour.NightDuration != "1" ? "s" : "")}" : "")}";

            if (hourDurationExists)
                duration +=
                    $"{(dayDurationExists ? " " : "")}{tour.HourDuration}{(includeTimeFrame ? $" Hour{(tour.HourDuration != "1" ? "s" : "")}" : "")}";

            details.Add(
                $"<span class='small d-block'><i class=\"fas fa-clock\" aria-hidden=\"true\"></i> {duration}</span>");
        }

        return details;
    }

    private static List<string> GetTourWithCategoryDetails(TourWithCategoryDto tour, bool includeLocations = true,
        bool includeTimeFrame = false)
    {
        var details = new List<string>();

        //if (!string.IsNullOrEmpty(tour.Address))
        //    details.Add(new Tuple<string, string>("<i class='fas fa-location-pin text-white small'></i>", tour.Address));

        if (tour.TourPrices != null)
        {
            var price = "<strong class='d-block'>";

            if (tour is {IsCategory: true, TourPrices: not null} && tour.TourPrices.Any())
            {
                price += $"From ${tour.TourPrices.OrderBy(x => x.Price).First().Price.ToString(CultureInfo.InvariantCulture)}";
            }
            else
            {
                if (tour.TourPrices != null)
                    price = tour.TourPrices.Aggregate(price,
                        (current, tourPrice) => current + $"${tourPrice.Price.ToString(CultureInfo.InvariantCulture)} / ");

                price = price.Trim();
                price = price.TrimEnd('/');
                price = price.Trim();
            }

            price += "</strong>";
            details.Add(price);
        }

        var dayDurationExists = !string.IsNullOrEmpty(tour.DayDuration);
        var nightDurationExists = !string.IsNullOrEmpty(tour.NightDuration);
        var hourDurationExists = !string.IsNullOrEmpty(tour.HourDuration);

        if (dayDurationExists || nightDurationExists || hourDurationExists)
        {
            var duration = "";
            if (dayDurationExists)
                duration +=
                    $"{tour.DayDuration}{(includeTimeFrame ? $" Day{(tour.DayDuration != "1" ? "s" : "")}" : "")}";

            if (nightDurationExists)
                duration +=
                    $"{(dayDurationExists ? " " : "")}{tour.NightDuration}{(includeTimeFrame ? $" Night{(tour.NightDuration != "1" ? "s" : "")}" : "")}";

            if (hourDurationExists)
                duration +=
                    $"{(dayDurationExists ? " " : "")}{tour.HourDuration}{(includeTimeFrame ? $" Hour{(tour.HourDuration != "1" ? "s" : "")}" : "")}";

            details.Add(
                $"<span class='small d-block'><i class=\"fas fa-clock\" aria-hidden=\"true\"></i> {duration}</span>");
        }

        if (!includeLocations) return details;
        if (tour.ChildTours == null || !tour.ChildTours.Any()) return details;
        var childTours = tour.ChildTours.Aggregate("", (current, childTour) => current + $"{childTour.Name}, ");

        childTours = childTours.Trim();
        childTours = childTours.TrimEnd(',');
        childTours = childTours.Trim();
        details.Add(
            $"<span class='small d-block'><i class=\"fas fa-location-pin\" aria-hidden=\"true\"></i> {childTours}</span>");


        return details;
    }

    public static GenericCardsComponent GetToursWithCategoriesGenericCards(string tenantId,
        IEnumerable<TourWithCategoryDto> toursWithCategories, TourWithCategoryDto? tourCategory = null,
        bool includeAll = false, string? title = "More Like This", bool includeTimeFrame = false)
    {
        return new GenericCardsComponent(title,
            GetToursWithCategoriesCards(tenantId, toursWithCategories, tourCategory, includeAll, true,
                includeTimeFrame), true);
    }

    public static PageCategoryComponent GetToursWithCategoriesPageCategoryComponent(string tenantId,
        IEnumerable<TourWithCategoryDto> toursWithCategories, TourWithCategoryDto? tourCategory = null,
        bool includeAll = false, bool includeTimeFrame = false)
    {
        return new PageCategoryComponent(tourCategory != null ? tourCategory.Name : "Explore",
            "It's rude to keep your passport waiting...",
            new GenericCardsComponent(null,
                GetToursWithCategoriesCards(tenantId, toursWithCategories, tourCategory, includeAll, true,
                    includeTimeFrame)));
    }

    private static List<CardComponent> GetToursWithCategoriesCards(string tenantId,
        IEnumerable<TourWithCategoryDto> toursWithCategories, TourWithCategoryDto? tourCategory = null,
        bool includeAll = false, bool includeLocations = true, bool includeTimeFrame = false)
    {
        toursWithCategories = includeAll
            ? toursWithCategories
            :
            tourCategory != null
                ?
                toursWithCategories.Where(x =>
                    x.CategoryId == tourCategory.Id || x.ParentTourCategoryId == tourCategory.Id ||
                    x.GroupParentCategoryId == tourCategory.Id)
                : toursWithCategories.Where(x => x.CategoryId == null && x.ParentTourCategoryId == null);

        var tourWithCategoryDtos = toursWithCategories as TourWithCategoryDto[] ?? toursWithCategories.ToArray();
        if (tourWithCategoryDtos.Length != 0)
        {
            tourWithCategoryDtos.DistinctBy(x => x.Id);
        }

        return tourWithCategoryDtos.Select(x =>
            new CardComponent(x.Name, x.ShortDescription, x.ImagePath, x.ThumbnailImagePath)
            {
                LgClass = 4,
                MdClass = 12,
                MarginBottom = 4,
                NavigatePage = $"/{(x.IsCategory ? "TourCategory" : "Tour")}/Index",
                NavigatePageRoutes = new Dictionary<string, string>()
                    {{$"{(x.IsCategory ? "tourCategoryName" : "tourName")}", x.FriendlyUrl}},
                AdditionalFields = GetTourWithCategoryDetails(x, includeLocations, includeTimeFrame),
                TenantId = tenantId
            }).ToList();
    }

    public static FullImageCardsComponent GetToursWithCategoriesFullImageCards(string tenantId,
        IEnumerable<TourWithCategoryDto> toursWithCategories, TourWithCategoryDto? tourCategory = null,
        string title = "Tours & Activities", bool includeLocations = true)
    {
        return new FullImageCardsComponent(title,
            GetToursWithCategoriesCards(tenantId, toursWithCategories, tourCategory, false, includeLocations));
    }
}