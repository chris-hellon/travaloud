using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Basket.Queries;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Basket;

public interface IBasketService : ITransientService
{
    Task<BasketModel> GetBasket();
    void EmptyBasket();
    Task<BasketModel> SetPromoCode(string promoCode);
    Task<BasketModel> RemoveItem(DefaultIdType id);
    Task<BasketItemModel> AddItem(BasketModel basket, BasketItemModel item);
    Task<Tuple<BasketModel, BasketItemModel>> AddItem(BasketItemRoomModel room, string propertyBookingUrl, DefaultIdType? userId);
    Task<BasketItemModel?>? AddItem(TourDetailsDto tour, TourDateDto? tourDate, int guestQuantity);
    Task<Tuple<BasketModel, BasketItemModel>?> AddGuest(DefaultIdType itemId, BasketItemGuestModel guest);
    Task<Tuple<BasketModel, BasketItemModel>?> RemoveGuestFromBasketItem(DefaultIdType itemId, DefaultIdType id);
    Task<Tuple<BasketModel, BasketItemModel>?> UpdateGuest(DefaultIdType itemId, BasketItemGuestModel guest);
    Task<IList<BasketItemGuestModel>> GetGuests(GetBasketItemGuestsRequest request);
    Task<BasketModel> AddExistingGuestToBasketItem(AddGuestToBasketItemRequest request);
    Task<BasketModel> SetPrimaryContactInformation(string? firstName, string? surname, string? email, DateTime? dateOfBirth, string? phoneNumber, string? nationality, string? gender, string? password, string? confirmPassword);
}