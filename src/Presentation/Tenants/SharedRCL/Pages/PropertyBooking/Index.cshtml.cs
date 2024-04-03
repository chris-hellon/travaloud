using System.Web;
using Newtonsoft.Json;
using Travaloud.Application.Basket;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.Cloudbeds.Responses;

using PropertyRoomDto = Travaloud.Application.Cloudbeds.Dto.PropertyRoomDto;

namespace Travaloud.Tenants.SharedRCL.Pages.PropertyBooking;

public class IndexModel : TravaloudBasePageModel
{
    private readonly ICloudbedsService _cloudbedsService;

    public IndexModel(ICloudbedsService cloudbedsService)
    {
        _cloudbedsService = cloudbedsService;
    }

    public override string MetaKeywords()
    {
        return $"{PropertyName}, hostel booking, budget travel, backpacker accommodation";
    }

    public override string MetaDescription()
    {
        return $"Looking for budget-friendly backpacker accommodation in Vietnam? Look no further than {TenantName} - {PropertyName}. Book your stay now for an unforgettable experience.";
    }

    private ApplicationUser? ApplicationUser { get; set; }
    
    public PropertyDto? Property { get; set; }
    
    public string PropertyName { get; set; } = string.Empty;

    public string PropertyImageUrl { get; set; } = string.Empty;
    
    public Guid PropertyId { get; set; }
    
    public int CloudbedsPropertyId { get; set; }
    
    public string IframeUrl { get; set; }
    
    public DateTime CheckInDate { get; set; }
    
    public DateTime CheckOutDate { get; set; }

    public string CloudbedsDataSerialized { get; set; }
    
    [BindProperty]
    public HeaderBannerComponent HeaderBanner { get; private set; }

    [BindProperty]
    public GetPropertyAvailabilityResponse? CloudbedsPropertyResponse { get; set; }
    
    [BindProperty]
    public BookNowComponent? BookNowBanner { get; private set; }
    
    [BindProperty]
    public BasketModel Basket { get; private set; }
    
    [BindProperty]
    public BasketItemModel? BasketItem { get; private set; }
    
    public async Task<IActionResult> OnGetAsync(string propertyName, string? checkInDate = null, string? checkOutDate = null, string? userId = null)
    {
        await OnGetDataAsync();
        
        var url = Request.GetEncodedUrl();

        LoginModal.ReturnUrl = url;
        RegisterModal.ReturnUrl = url;
        Basket = await BasketService.GetBasket();

        if (userId != null)
            ApplicationUser = await UserManager.FindByIdAsync(userId);

        if (Properties == null) return LocalRedirect("/error");

        Property = Properties.FirstOrDefault(h => h.Name.UrlFriendly() == propertyName.UrlFriendly());

        if (Property == null) return LocalRedirect("/error");

        var pageTitle = Property.PageTitle ?? Property.Name;
        var pageSubTitle = Property.PageSubTitle ?? "";

        HeaderBanner = new HeaderBannerComponent(pageTitle, pageSubTitle, null, Property.ImagePath, new List<OvalContainerComponent>() { new("hostelPageHeaderBannerOvals1", 15, null, -30, null) });
        PropertyName = Property.Name;
        PropertyId = Property.Id;
        PropertyImageUrl = Property.ImagePath ?? "";
        
        if (!string.IsNullOrEmpty(Property.CloudbedsPropertyId))
            CloudbedsPropertyId = int.Parse(Property.CloudbedsPropertyId);
        
        if (!string.IsNullOrEmpty(Property.CloudbedsApiKey) && !string.IsNullOrEmpty(Property.CloudbedsPropertyId))
        {
            await GetCloudbedsProperties(checkInDate, checkOutDate);
        }
        else
        {
            IframeUrl = $"https://hotels.cloudbeds.com/reservation/{Property.CloudbedsKey}";

            if (ApplicationUser != null)
            {
                IframeUrl += "?";

                if (!string.IsNullOrEmpty(ApplicationUser.FirstName))
                    IframeUrl += $"firstName={ApplicationUser.FirstName}";

                if (!string.IsNullOrEmpty(ApplicationUser.LastName))
                    IframeUrl += $"&lastName={ApplicationUser.LastName}";

                if (!string.IsNullOrEmpty(ApplicationUser.Email))
                    IframeUrl += $"&email={ApplicationUser.Email}";

                if (!string.IsNullOrEmpty(ApplicationUser.Nationality))
                    IframeUrl += $"&country={ApplicationUser.Nationality}";

                if (!string.IsNullOrEmpty(ApplicationUser.PhoneNumber))
                    IframeUrl += $"&phone={ApplicationUser.PhoneNumber}";
            }

            if (string.IsNullOrEmpty(checkInDate) && string.IsNullOrEmpty(checkOutDate)) return Page();
        
            IframeUrl += "#";

            if (!string.IsNullOrEmpty(checkInDate))
                IframeUrl += $"&checkin={checkInDate}";

            if (!string.IsNullOrEmpty(checkOutDate))
                IframeUrl += $"&checkout={checkOutDate}";   
        }

        return Page();

    }

    private async Task GetCloudbedsProperties(string checkInDate, string checkOutDate)
    {
        CloudbedsPropertyResponse = await _cloudbedsService.GetPropertyAvailability(new GetPropertyAvailabilityRequest()
        {
            PropertyId = Property.CloudbedsPropertyId,
            PropertyApiKey = Property.CloudbedsApiKey,
            StartDate = checkInDate,
            EndDate = checkOutDate
        });

        CheckInDate = DateTime.Parse(checkInDate);
        CheckOutDate = DateTime.Parse(checkOutDate);
            
        BookNowBanner = new BookNowComponent(Properties, PropertyId)
        {
            CheckInDate = CheckInDate,
            CheckOutDate = CheckOutDate,
            DateRange = $"{CheckInDate.ToShortDateString()} - {CheckOutDate.ToShortDateString()}"
        };

        var basket = await BasketService.GetBasket();

        if (CloudbedsPropertyResponse.Success && CloudbedsPropertyResponse.Data != null && CloudbedsPropertyResponse.Data.Any())
        {
            CloudbedsDataSerialized = HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(CloudbedsPropertyResponse.Data.First()));
            
            var propertyData = CloudbedsPropertyResponse.Data.First();
            BasketItem = basket.Items.FirstOrDefault(x =>
                x is {CheckOutDateParsed: not null, CheckInDateParsed: not null} &&
                x.PropertyId == PropertyId &&
                x.CheckInDateParsed.Value.Date == CheckInDate.Date &&
                x.CheckOutDateParsed.Value.Date == CheckOutDate.Date);

            if (BasketItem != null)
            {
                var propertyRoomTypeIds = BasketItem.Rooms.Select(x => x.RoomTypeId);
                var existingRooms = propertyData.PropertyRooms.Where(x => propertyRoomTypeIds.Contains(x.RoomTypeId));
                var propertyRoomDtos = existingRooms as PropertyRoomDto[] ?? existingRooms.ToArray();
                
                if (propertyRoomDtos.Length != 0)
                {
                    foreach (var existingRoom in propertyRoomDtos)
                    {
                        var basketRoomItem = BasketItem.Rooms.FirstOrDefault(x => x.RoomTypeId == existingRoom.RoomTypeId);
                        if (basketRoomItem == null) continue;
                        
                        existingRoom.RoomQuantity = basketRoomItem.RoomQuantity;
                        existingRoom.AdultQuantity = basketRoomItem.AdultQuantity;
                        existingRoom.ChildrenQuantity = basketRoomItem.ChildrenQuantity;
                    }
                }
            }
        }
    }
}