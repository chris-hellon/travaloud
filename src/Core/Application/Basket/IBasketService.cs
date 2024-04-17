using Travaloud.Application.Basket.Commands;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Basket.Queries;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Basket;

public interface IBasketService : ITransientService
{
    /// <summary>
    /// Returns a Basket.
    /// </summary>
    /// <returns></returns>
    Task<BasketModel> GetBasket();
    
    /// <summary>
    /// Empties a Basket.
    /// </summary>
    void EmptyBasket();
    
    /// <summary>
    /// Sets a Basket Promo Code.
    /// </summary>
    /// <param name="promoCode"></param>
    /// <returns></returns>
    Task<BasketModel> SetPromoCode(string promoCode);
    
    /// <summary>
    /// Removes a Basket Item.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BasketModel> RemoveItem(DefaultIdType id);
    
    /// <summary>
    /// Removes a Room from a Basket Item.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="itemId"></param>
    /// <returns></returns>
    Task<BasketModel> RemoveRoom(DefaultIdType id, DefaultIdType itemId);
    
    /// <summary>
    /// Adds a Basket Item.
    /// </summary>
    /// <param name="basket"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    Task<BasketItemModel> AddItem(BasketModel basket, BasketItemModel item);
    
    /// <summary>
    /// Adds a Property Basket Item.
    /// </summary>
    /// <param name="room"></param>
    /// <param name="propertyBookingUrl"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Tuple<BasketModel, BasketItemModel>> AddItem(BasketItemRoomModel room, string propertyBookingUrl, DefaultIdType? userId);
    
    /// <summary>
    /// Adds a Tour Basket Item.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    Task<Tuple<BasketModel, BasketItemModel>> AddItem(BasketItemDateModel date);
    
    /// <summary>
    /// Adds a Guest to a Basket Item.
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="guest"></param>
    /// <returns></returns>
    Task<Tuple<BasketModel, BasketItemModel>?> AddGuest(
        DefaultIdType itemId, 
        BasketItemGuestModel guest);
    
    /// <summary>
    /// Removes a Guest from a Basket Item.
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Tuple<BasketModel, BasketItemModel>?> RemoveGuestFromBasketItem(
        DefaultIdType itemId, 
        DefaultIdType id);
    
    /// <summary>
    /// Updates a Guest in a Basket Item.
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="guest"></param>
    /// <returns></returns>
    Task<Tuple<BasketModel, BasketItemModel>?> UpdateGuest(
        DefaultIdType itemId, 
        BasketItemGuestModel guest);
    
    /// <summary>
    /// Retrieves Guests from a Basket Item.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<IList<BasketItemGuestModel>> GetGuests(GetBasketItemGuestsRequest request);
    
    /// <summary>
    /// Adds an existing Guest to another Basket Item.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<BasketModel> AddExistingGuestToBasketItem(AddGuestToBasketItemRequest request);
    
    /// <summary>
    /// Sets the Basket primary Contact Information.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="surname"></param>
    /// <param name="email"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="nationality"></param>
    /// <param name="gender"></param>
    /// <param name="estimatedArrivalTime"></param>
    /// <param name="password"></param>
    /// <param name="confirmPassword"></param>
    /// <returns></returns>
    Task<BasketModel> SetPrimaryContactInformation(
        string? firstName,
        string? surname,
        string? email,
        DateTime? dateOfBirth,
        string? phoneNumber,
        string? nationality,
        string? gender,
        TimeSpan? estimatedArrivalTime,
        string? password,
        string? confirmPassword);
}