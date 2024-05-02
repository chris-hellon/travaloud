using Finbuckle.MultiTenant;
using Travaloud.Application.Basket;
using Travaloud.Application.Basket.Commands;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Basket.Queries;
using Travaloud.Infrastructure.Common.Extensions;

namespace Travaloud.Infrastructure.Basket;

public class BasketService : IBasketService
{
    private readonly ISession? _session;
    private const string BasketKey = "BookingBasket";
    private readonly IMultiTenantContextAccessor _multiTenantContextAccessor;

    public BasketService(IHttpContextAccessor httpContextAccessor, IMultiTenantContextAccessor multiTenantContextAccessor)
    {
        _session = httpContextAccessor.HttpContext?.Session;
        _multiTenantContextAccessor = multiTenantContextAccessor;
    }

    public Task<BasketModel> GetBasket()
    {
        return Task.FromResult(_session.GetOrCreateObjectFromSession<BasketModel>(BasketKey));
    }

    public void EmptyBasket()
    {
        _session?.Remove(BasketKey);
    }

    public async Task<BasketModel> SetPromoCode(string promoCode)
    {
        var basket = await GetBasket();
        basket.SetPromoCode(promoCode);
        
        _session.UpdateObjectInSession(BasketKey, basket);

        return basket;
    }
    
    public async Task<BasketModel> SetCreateId(string createId)
    {
        var basket = await GetBasket();
        basket.SetCreateId(createId);
        
        _session.UpdateObjectInSession(BasketKey, basket);

        return basket;
    }

    public async Task<BasketModel> SetPrimaryContactInformation(string? firstName, string? surname, string? email, DateTime? dateOfBirth, string? phoneNumber, string? nationality, string? gender, TimeSpan? estimatedArrivalTime, string? password, string? confirmPassword)
    {
        var basket = await GetBasket();    
        basket.SetPrimaryContactInformation(firstName, surname, email, dateOfBirth, phoneNumber, nationality, gender, estimatedArrivalTime, password, confirmPassword);
        
        _session.UpdateObjectInSession(BasketKey, basket);

        return basket;
    }
    
    public async Task<BasketModel> RemoveItem(DefaultIdType id)
    {
        var basket = await GetBasket();

        var item = basket.Items.FirstOrDefault(x => x.Id == id);

        if (item != null)
            basket.Items.Remove(item);

        basket.CalculateTotal();

        _session.UpdateObjectInSession(BasketKey, basket);
        return basket;
    }

    public async Task<BasketModel> RemoveRoom(DefaultIdType id, DefaultIdType itemId)
    {
        var basket = await GetBasket();

        var item = basket.Items.FirstOrDefault(x => x.Id == itemId);
        var room = item?.Rooms?.FirstOrDefault(x => x.Id == id);
            
        if (room != null)
            item?.Rooms?.Remove(room);

        _session.UpdateObjectInSession(BasketKey, basket);
        
        return basket;
    }

    public Task<BasketItemModel> AddItem(BasketModel basket, BasketItemModel item)
    {
        item.Id = DefaultIdType.NewGuid();
        basket.Items.Add(item);

        return Task.FromResult(item);
    }

    public async Task<Tuple<BasketModel, BasketItemModel>> AddItem(BasketItemRoomModel room, string propertyBookingUrl, DefaultIdType? userId)
    {
        var basket = await GetBasket();
        var item = basket.Items.FirstOrDefault(x => x is {CheckOutDateParsed: not null, CheckInDateParsed: not null} &&
                                                    x.PropertyId == room.PropertyId &&
                                                    x.CheckInDateParsed.Value.Date == room.CheckInDateParsed.Date &&
                                                    x.CheckOutDateParsed.Value.Date == room.CheckOutDateParsed.Date);

        // item = await AddItem(
        //     basket: basket,
        //     item: new BasketItemModel(room.PropertyName,
        //         room.PropertyId,
        //         room.PropertyImageUrl,
        //         room.CheckInDate,
        //         room.CheckOutDate,
        //         propertyBookingUrl,
        //         room.CloudbedsPropertyId,
        //         userId,
        //         new List<BasketItemRoomModel>()));
        //
        // item.AddRoom(room);
        
        if (item == null)
        {
            item = await AddItem(
                basket: basket,
                item: new BasketItemModel(room.PropertyName,
                    room.PropertyId,
                    room.PropertyImageUrl,
                    room.CheckInDate,
                    room.CheckOutDate,
                    propertyBookingUrl,
                    room.CloudbedsPropertyId,
                    userId,
                    new List<BasketItemRoomModel>()));
        
            item.AddRoom(room);
        }
        else
        {
            var existingRoom = item.Rooms?.FirstOrDefault(x =>
                x.RoomTypeId == room.RoomTypeId &&
                x.CheckInDateParsed.Date == room.CheckInDateParsed.Date &&
                x.CheckOutDateParsed.Date == room.CheckOutDateParsed.Date);
        
            if (existingRoom == null)
                item.AddRoom(room);
            else if (room.RoomQuantity == 0)
                item.Rooms?.Remove(existingRoom);
            else
                existingRoom.Update(room.RoomQuantity, room.AdultQuantity, room.ChildrenQuantity);
        }

        if (item is {Rooms.Count: 0})
            basket.Items.Remove(item);

        basket.CalculateTotal();
        _session.UpdateObjectInSession(BasketKey, basket);

        return new Tuple<BasketModel, BasketItemModel>(basket, item);
    }

    public async Task<Tuple<BasketModel, BasketItemModel>> AddItem(BasketItemDateModel date)
    {
        var basket = await GetBasket();
        var item = basket.Items.FirstOrDefault(x => x.TourId == date.TourId && x.TourDates?.FirstOrDefault(td => td.DateId == date.DateId) != null);

        // item = await AddItem(
        //     basket: basket,
        //     item: new BasketItemModel(date.TourName!,
        //         date.TourId,
        //         date.TourImageUrl!,
        //         new List<BasketItemDateModel>()));
        //
        // item.AddDate(date);
        //
        if (item == null)
        {
            item = await AddItem(
                basket: basket,
                item: new BasketItemModel(date.TourName!,
                    date.TourId,
                    date.TourImageUrl!,
                    new List<BasketItemDateModel>()));
        
            item.AddDate(date);
        }
        else
        {
            var existingDate = item.TourDates?.FirstOrDefault(x => x.DateId == date.DateId);
        
            if (existingDate == null)
                item.AddDate(date);
            else if (date.GuestQuantity == 0)
                item.TourDates?.Remove(existingDate);
            else
                existingDate.Update(date.GuestQuantity);
        }

        var tenantId = _multiTenantContextAccessor?.MultiTenantContext?.TenantInfo?.Id;
        item.SetTourEditBookingHref(date.TourName!, tenantId);

        if (item is {TourDates.Count: 0})
            basket.Items.Remove(item);

        basket.CalculateTotal();
        _session.UpdateObjectInSession(BasketKey, basket);

        return new Tuple<BasketModel, BasketItemModel>(basket, item);
    }

    
    // public async Task<Tuple<BasketModel, BasketItemModel>> AddItem(DefaultIdType tourId, DefaultIdType tourDateId, int guestQuantity)
    // {
    //     var basket = await GetBasket();
    //     var item = basket.Items.FirstOrDefault(x => x.TourId == tourId);
    //
    //     if (item == null)
    //     {
    //         await AddItem(
    //             basket: basket,
    //             item: new BasketItemModel(tour, new List<BasketItemDateModel>
    //             {
    //                 new(tourDate, guestQuantity, tour.Name, tour.ImagePath ?? string.Empty)
    //             }));
    //     }
    //     else
    //     {
    //         var itemTourDate = item.TourDates?.FirstOrDefault(x => x.TourDate.Id == tourDate.Id);
    //
    //         if (itemTourDate != null)
    //             itemTourDate.GuestQuantity += guestQuantity;
    //         else
    //             item.TourDates?.Add(new BasketItemDateModel(tourDate, guestQuantity, tour.Name,
    //                 tour.ImagePath ?? string.Empty));
    //     }
    //
    //     basket.CalculateTotal();
    //     _session.UpdateObjectInSession(BasketKey, basket);
    //
    //     return new Tuple<BasketModel, BasketItemModel>(basket, item);
    // }
    //
    //
    public async Task<Tuple<BasketModel, BasketItemModel>?> AddGuest(DefaultIdType itemId, BasketItemGuestModel guest)
    {
        var basket = await GetBasket();
        var basketItem = basket.Items.FirstOrDefault(x => x.Id == itemId);

        if (basketItem != null)
        {
            guest.Id = DefaultIdType.NewGuid();

            basketItem.Guests ??= new List<BasketItemGuestModel>();
            basketItem.Guests?.Add(guest);
        }

        _session.UpdateObjectInSession(BasketKey, basket);

        return basketItem != null ? new Tuple<BasketModel, BasketItemModel>(basket, basketItem) : null;
    }
    
    public async Task<Tuple<BasketModel, BasketItemModel>?> UpdateGuest(DefaultIdType itemId, BasketItemGuestModel guest)
    {
        var basket = await GetBasket();
        var basketItem = basket.Items.FirstOrDefault(x => x.Id == itemId);

        var guests = basket.Items.Where(x => x.Guests != null).SelectMany(x => x.Guests.Where(g => g.Id == guest.Id));

        if (guests.Any())
        {
            foreach (var basketItemGuest in guests)
            {
                basketItemGuest.Update(
                    guest.FirstName,
                    guest.Surname,
                    guest.Email,
                    guest.DateOfBirth,
                    guest.PhoneNumber,
                    guest.Nationality,
                    guest.Gender
                );
            }
        }

        // if (basketItem?.Guests != null)
        // {
        //     var basketItemGuest = basketItem.Guests.FirstOrDefault(x => x.Id == guest.Id);
        //
        //     basketItemGuest?.Update(
        //         guest.FirstName,
        //         guest.Surname,
        //         guest.Email,
        //         guest.DateOfBirth,
        //         guest.PhoneNumber,
        //         guest.Nationality,
        //         guest.Gender
        //     );
        // }

        _session.UpdateObjectInSession(BasketKey, basket);

        return basketItem != null ? new Tuple<BasketModel, BasketItemModel>(basket, basketItem) : null;
    }
    
    public async Task<Tuple<BasketModel, BasketItemModel>?> RemoveGuestFromBasketItem(DefaultIdType itemId, DefaultIdType id)
    {
        var basket = await GetBasket();
        var basketItem = basket.Items.FirstOrDefault(x => x.Id == itemId);

        var guest = basketItem?.Guests?.FirstOrDefault(x => x.Id == id);
        
        if (guest != null)
        {
            basketItem?.Guests?.Remove(guest);
        }

        _session.UpdateObjectInSession(BasketKey, basket);

        return basketItem != null ? new Tuple<BasketModel, BasketItemModel>(basket, basketItem) : null;
    }

    public async Task<IList<BasketItemGuestModel>> GetGuests(GetBasketItemGuestsRequest request)
    {
        var basket = await GetBasket();
        var basketItem = basket.Items.FirstOrDefault(x => x.Id == request.Id);

        if (basketItem == null) return new List<BasketItemGuestModel>();
        {
            var existingGuests = basketItem.Guests ?? new List<BasketItemGuestModel>();
            var basketItems = basket.Items.Where(x => x.Id != request.Id);

            var guestsNotInExisting = basketItems
                .SelectMany(x => x.Guests ?? new List<BasketItemGuestModel>())
                .Where(guest => existingGuests.All(existingGuest => existingGuest.Id != guest.Id))
                .ToList();

            return guestsNotInExisting.DistinctBy(x => x.Id).ToList();
        }

    }

    public async Task<BasketModel> AddExistingGuestToBasketItem(AddGuestToBasketItemRequest request)
    {
        var basket = await GetBasket();
        var basketItemFrom = basket.Items.FirstOrDefault(x => x.Id == request.ItemIdFrom);
        var basketItemTo = basket.Items.FirstOrDefault(x => x.Id == request.ItemId);

        if (basketItemFrom != null && basketItemTo != null && basketItemFrom.Guests != null)
        {
            var guest = basketItemFrom.Guests.FirstOrDefault(x => x.Id == request.GuestId);

            if (guest != null)
            {
                var clonedGuest = new BasketItemGuestModel()
                {
                    FirstName = guest.FirstName,
                    Surname = guest.Surname,
                    DateOfBirth = guest.DateOfBirth,
                    Email = guest.Email,
                    Nationality = guest.Nationality,
                    Gender = guest.Gender,
                    PhoneNumber = guest.PhoneNumber,
                    ItemId = request.ItemId,
                    Id = guest.Id
                };
                
                basketItemTo.Guests ??= new List<BasketItemGuestModel>();
                basketItemTo.Guests?.Add(clonedGuest);
            }
        }
        
        _session.UpdateObjectInSession(BasketKey, basket);

        return basket;
    }
}