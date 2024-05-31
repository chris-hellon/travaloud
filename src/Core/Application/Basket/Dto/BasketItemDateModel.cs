namespace Travaloud.Application.Basket.Dto;

public class BasketItemDateModel
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType DateId { get; set; }
    public DefaultIdType TourId { get; set; }
    public string? TourName { get; set; }
    public string? TourImageUrl { get; set; }
    public int GuestQuantity { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public string FormattedPrice => $"{Price:n2}";
    public string GuestQuantityLabel => $"{GuestQuantity} Guest{(GuestQuantity > 1 ? "s" : "")}";
    public string? PickupLocation { get; set; }
    public IList<BasketItemGuestModel>? Guests { get; set; }
    public void Update(int guestQuantity)
    {
        GuestQuantity = guestQuantity;
    }
}
