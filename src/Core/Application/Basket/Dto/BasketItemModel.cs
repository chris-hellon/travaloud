using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Basket.Dto;

public class BasketItemModel
{
	public DefaultIdType Id { get; set; }
	public string? PropertyName { get; set; }
	public DefaultIdType? PropertyId { get; set; }
	public int? CloudbedsPropertyId { get; set; }
	public string? PropertyImageUrl { get; set; }
	public bool IsConfirmationPage { get; set; }
	public bool HideRemoveButton { get; set; }
	public string? CheckInDate { get; set; } 
	public string? CheckOutDate { get; set; }
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
	public string EditBookingHref { get; set; }
	
	public IList<BasketItemRoomModel>? Rooms { get; init; }
	
	public DefaultIdType? TourId { get; set; }
	
	public TourDetailsDto? Tour { get; init; }
	
	public IList<BasketItemDateModel>? TourDates { get; set; }

	public IList<BasketItemGuestModel>? Guests { get; set; }
	
	public decimal Total => CalculateTotal();
	
	public bool GuestsExistInBasket { get; set; }
	
	public BasketItemModel()
	{
	}

	public BasketItemModel(TourDetailsDto tour, IList<BasketItemDateModel> tourDates)
	{
		Tour = tour;
		TourId = tour.Id;
		TourDates = tourDates;
	}

	public BasketItemModel(string propertyName, DefaultIdType propertyId, string propertyImageUrl, string checkInDate, string checkOutDate, string propertyBookingUrl, DefaultIdType? userId, IList<BasketItemRoomModel> rooms)
	{
		PropertyName = propertyName;
		PropertyId = propertyId;
		PropertyImageUrl = propertyImageUrl;
		CheckInDate = checkInDate;
		CheckOutDate = checkOutDate;
		Rooms = rooms;
		
		var url = $"/{propertyBookingUrl}/{propertyName.UrlFriendly()}/{CheckInDateParsed.Value.ToString("yyyy-MM-dd").UrlFriendly()}/{CheckOutDateParsed.Value.ToString("yyyy-MM-dd").UrlFriendly()}{(userId != null ? $"/{userId.ToString()}" : "")}";
		EditBookingHref = url;
	}


	public void AddRoom(BasketItemRoomModel room)
	{
		room.Id = DefaultIdType.NewGuid();
		Rooms?.Add(room);
	}
	
	public decimal CalculateTotal()
	{
		var total = 0M;

		return Rooms?.Sum(room => room.RoomRate * room.RoomQuantity) ?? total;
	}
}