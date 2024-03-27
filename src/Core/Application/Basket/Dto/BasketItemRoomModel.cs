namespace Travaloud.Application.Basket.Dto;

public class BasketItemRoomModel
{
    public DefaultIdType Id { get; set; }
	
    public int RoomTypeId { get; set; }
	
    public string RoomTypeName { get; set; } = default!;
	
    public bool IsShared { get; set; }
	
    public int RoomQuantity { get; set; }
	
    public int AdultQuantity { get; set; }
	
    public int ChildrenQuantity { get; set; }
	
    public decimal RoomRate { get; set; }

    public string PropertyName { get; set; } = default!;
	
    public DefaultIdType PropertyId { get; set; }
	
    public string PropertyImageUrl { get; set; } = default!;
	
    public bool IsConfirmationPage { get; set; } = false;
	
    public string CheckInDate { get; set; } = default!;
	
    public string CheckOutDate { get; set; }= default!;

    public DateTime CheckInDateParsed => DateTime.Parse(CheckInDate);
    public DateTime CheckOutDateParsed => DateTime.Parse(CheckOutDate);

    public string FormattedRate => $"{RoomRate * RoomQuantity:n2}";
    public string RoomQuantityLabel {
	    get
	    {
		    var roomQuantityLabel =  $"{RoomQuantity} {(IsShared ? "Bed" : "Room")}{(RoomQuantity > 1 ? "s" : "")}";

		    if (AdultQuantity > 0)
			    roomQuantityLabel += $" x {AdultQuantity} adults";

		    if (ChildrenQuantity > 0)
			    roomQuantityLabel += $" {(AdultQuantity > 0 ? "&" : "x")} {ChildrenQuantity} children";
		    
		    return roomQuantityLabel;
	    }
	}

    public void Update(int roomQuantity, int adultQuantity, int childrenQuantity)
    {
        RoomQuantity = roomQuantity;
        AdultQuantity = adultQuantity;
        ChildrenQuantity = childrenQuantity;
    }
}