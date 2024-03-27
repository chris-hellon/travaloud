namespace Travaloud.Application.Basket.Queries;

public class AddGuestToBasketItemRequest
{
    public DefaultIdType ItemId { get; set; }
    public DefaultIdType ItemIdFrom { get; set; }
    public DefaultIdType GuestId { get; set; }
}