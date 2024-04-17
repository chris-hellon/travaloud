using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Basket.Dto;

public class BasketItemModel
{
    public DefaultIdType Id { get; set; }
    public string? EditBookingHref { get; set; }
    public IList<BasketItemGuestModel>? Guests { get; set; }
    
    #region Properties
    public string? PropertyName { get; init; }
    public DefaultIdType? PropertyId { get; init; }
    public int? CloudbedsPropertyId { get; init; }
    public string? PropertyImageUrl { get; init; }
    public bool IsConfirmationPage { get; set; }
    public bool HideRemoveButton { get; set; }
    public string? CheckInDate { get; init; }
    public string? CheckOutDate { get; init; }
    public DateTime? CheckInDateParsed
    {
        get
        {
            if (CheckInDate != null) return DateTime.Parse(CheckInDate);
            return null;
        }
    }
    public DateTime? CheckOutDateParsed
    {
        get
        {
            if (CheckOutDate != null) return DateTime.Parse(CheckOutDate);
            return null;
        }
    }
    public IList<BasketItemRoomModel>? Rooms { get; init; }
    #endregion

    #region Tours
    public DefaultIdType? TourId { get; set; }
    public string? TourName { get; set; }
    public string? TourImageUrl { get; set; }
    public IList<BasketItemDateModel>? TourDates { get; set; }
    #endregion

    public decimal Total => CalculateTotal();

    public bool GuestsExistInBasket { get; set; }

    public int GuestCount
    {
        get { return (Rooms?.Sum(x => x.RoomQuantity + x.AdultQuantity) ?? 0) + (TourDates?.Sum(x => x.GuestQuantity) ?? 0); }
    }

    public int RequiredGuestCount => GuestCount - 1;
    public int MaxGuestCount => RequiredGuestCount;
    public bool ShowTable => Guests != null && Guests.Any();
    public bool MaxGuestCountFulfilled => ShowTable && Guests?.Count >= MaxGuestCount;
    public bool ShowAddGuestButton => !MaxGuestCountFulfilled && GuestsExistInBasket;

    public BasketItemModel()
    {
    }
    
    public BasketItemModel(
        string tourName,
        DefaultIdType tourId,
        string tourImageUrl,
        IList<BasketItemDateModel> tourDates)
    {
        TourName = tourName;
        TourId = tourId;
        TourImageUrl = tourImageUrl;
        TourDates = tourDates;
    }
    
    public BasketItemModel(
        string propertyName, 
        DefaultIdType propertyId, 
        string propertyImageUrl, 
        string checkInDate,
        string checkOutDate, 
        string propertyBookingUrl, 
        int cloudbedsPropertyId, 
        DefaultIdType? userId,
        IList<BasketItemRoomModel> rooms)
    {
        PropertyName = propertyName;
        PropertyId = propertyId;
        PropertyImageUrl = propertyImageUrl;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        Rooms = rooms;
        CloudbedsPropertyId = cloudbedsPropertyId;

        var url = $"/{propertyBookingUrl}/{propertyName.UrlFriendly()}/{CheckInDateParsed?.ToString("yyyy-MM-dd").UrlFriendly()}/{CheckOutDateParsed?.ToString("yyyy-MM-dd").UrlFriendly()}{(userId != null ? $"/{userId.ToString()}" : "")}";
        EditBookingHref = url;
    }


    public void AddRoom(BasketItemRoomModel room)
    {
        room.Id = DefaultIdType.NewGuid();
        Rooms?.Add(room);
    }
    
    public void AddDate(BasketItemDateModel date)
    {
        date.Id = DefaultIdType.NewGuid();
        TourDates?.Add(date);
    }

    public void SetTourEditBookingHref(string tourName)
    {
        if (TourDates == null) return;
        
        var url = $"/tours/{tourName.UrlFriendly()}/{TourDates.First().DateId}/{TourDates.First().GuestQuantity}";
        EditBookingHref = url;
    }

    public decimal CalculateTotal()
    { 
        var total = 0M;

        if (Rooms != null && Rooms.Any())
            total = (decimal) Rooms?.Sum(room => room.RoomRate * room.RoomQuantity)!;
        
        if (TourDates != null && TourDates.Any())
            total += (decimal) TourDates?.Sum(date => date.Price * date.GuestQuantity)!;
        
        return total;
    }
}