namespace Travaloud.Application.Basket.Commands;

public class AddGuestToBasketItemRequest
{
    public DefaultIdType ItemId { get; set; }
    public DefaultIdType ItemIdFrom { get; set; }
    public DefaultIdType GuestId { get; set; }
}