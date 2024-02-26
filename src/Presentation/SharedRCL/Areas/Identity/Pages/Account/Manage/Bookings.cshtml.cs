using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Properties.Dto;

namespace Travaloud.Tenants.SharedRCL.Areas.Identity.Pages.Account.Manage;

public class BookingsModel : TravaloudBasePageModel
{
    private readonly IBookingsService _bookingsService;
    private readonly IPropertiesService _propertiesService;

    public BookingsModel(IBookingsService bookingsService, IPropertiesService propertiesService)
    {
        _bookingsService = bookingsService;
        _propertiesService = propertiesService;
    }

    [BindProperty] public IEnumerable<BookingDto>? Bookings { get; set; }

    [BindProperty] public BookingDto? Booking { get; set; }

    [BindProperty] public PropertyDetailsDto? BookingProperty { get; set; }

    [BindProperty] public FeaturesTableComponent? FacilitiesTable { get; private set; }

    [BindProperty] public CarouselCardsComponent? ToursCards { get; private set; }

    public async Task OnGetAsync(string? bookingId = null)
    {
        await base.OnGetDataAsync();

        Bookings = await _bookingsService.GetGuestBookingsAsync(new GetGuestBookingsRequest(UserId.ToString()!));

        var bookingDtos = Bookings as BookingDto[] ?? Bookings.ToArray();
        if (bookingDtos.Length != 0)
        {
            if (bookingId != null)
            {
                Booking = bookingDtos.FirstOrDefault(x => x.Id == Guid.Parse(bookingId));

                var bookingItemWithProperty = Booking?.Items?.FirstOrDefault(x => x.PropertyId.HasValue);
            
                if (bookingItemWithProperty != null)
                {
                    if (bookingItemWithProperty.PropertyId != null)
                        BookingProperty = await _propertiesService.GetAsync(bookingItemWithProperty.PropertyId.Value);

                    if (BookingProperty != null)
                    {
                        FacilitiesTable = await WebComponentsBuilder.FuseHostelsAndTravel.GetHostelFacilitiesAsync(BookingProperty);
                        ToursCards = await WebComponentsBuilder.FuseHostelsAndTravel.GetToursCarouselCardsAsync(Tours, "onScroll", $"TOURS IN {BookingProperty.PageTitle?.ToUpper()}", null);
                    }
                    //await SetPropertyInformation(BookingProperty);
                }
            }  
        }
        
        //TODO: implement this
        // Bookings = await ApplicationRepository.GetGuestBookingsAsync(UserId);
        //
        // if (Bookings.Any())
        // {
        //     if (bookingId != null)
        //     {
        //         Booking = Bookings.FirstOrDefault(x => x.Id == Guid.Parse(bookingId));
        //
        //         if (Booking != null)
        //         {
        //             var bookingItemWithProperty = Booking.Items.FirstOrDefault(x => x.PropertyId.HasValue);
        //
        //             if (bookingItemWithProperty != null)
        //             {
        //                 BookingProperty = await ApplicationRepository.GetPropertyWithDetailsAsync(bookingItemWithProperty.PropertyId.Value);
        //
        //                 var tourPrices = Tours.SelectMany(x => x.TourPrices);
        //                 //await SetPropertyInformation(BookingProperty);
        //
        //                 FacilitiesTable = WebComponentsBuilder.FuseHostelsAndTravel.GetHostelFacilities(BookingProperty);
        //                 ToursCards = WebComponentsBuilder.FuseHostelsAndTravel.GetToursCarouselCards(Tours, "onScroll", $"TOURS IN {BookingProperty.PageTitle.ToUpper()}", null);
        //             }
        //         }
        //     }  
        // }
    }
}