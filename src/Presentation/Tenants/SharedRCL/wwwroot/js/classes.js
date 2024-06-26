class BasketItemModel {
    constructor(id, propertyName = null, propertyId = null, cloudbedsPropertyId = null, propertyImageUrl = null, checkInDate = null, checkOutDate = null, tourName = null, tourId = null) {
        this.Id = id;
        this.PropertyName = propertyName;
        this.PropertyId = propertyId;
        this.CloudbedsPropertyId = cloudbedsPropertyId;
        this.PropertyImageUrl = propertyImageUrl;
        this.CheckInDate = checkInDate;
        this.CheckOutDate = checkOutDate;
        this.TourName = tourName;
        this.TourId = tourId;
    }
}

class BasketItemRoomModel {
    constructor(roomTypeId, roomTypeName, roomIsShared, roomQuantity, roomRate, adultQuantity, childrenQuantity, cloudbedsPropertyId) {
        this.PropertyId = propertyId;
        this.PropertyName = propertyName;
        this.PropertyImageUrl = propertyImageUrl;
        this.CheckInDate = checkInDate.toUTCString();
        this.CheckOutDate = checkOutDate.toUTCString();
        this.RoomTypeName = roomTypeName;
        this.RoomTypeId = roomTypeId;
        this.IsShared = roomIsShared;
        this.RoomQuantity = parseInt(roomQuantity);
        this.RoomRate = roomRate;
        this.AdultQuantity = parseInt(adultQuantity);
        this.ChildrenQuantity = parseInt(childrenQuantity);
        this.CloudbedsPropertyId = parseInt(cloudbedsPropertyId);
    }
}

class BasketItemDateModel
{
    constructor(dateId, tourId, tourName, tourImageUrl, guestQuantity, price, startDate, pickupLocation) {
        this.DateId = dateId;
        this.TourId = tourId;
        this.TourName = tourName;
        this.GuestQuantity = guestQuantity;
        this.TourImageUrl = tourImageUrl;
        this.Price = price;
        this.StartDate = startDate;
        this.PickupLocation = pickupLocation;
    }
}

class BasketItemGuestModel {
    constructor(itemId, firstName, surname, email, dateOfBirth, phoneNumber, nationality, gender, id = null) {
        this.Id = id;
        this.ItemId = itemId;
        this.FirstName = firstName;
        this.Surname = surname;
        this.Email = email;
        this.DateOfBirth = dateOfBirth;
        this.PhoneNumber = phoneNumber;
        this.Nationality = nationality;
        this.Gender = gender;
    }
}

class BasketItemGuestRequest {
    constructor(itemId, id) {
        this.ItemId = itemId;
        this.Id = id;
    }
}

class GetBasketItemGuestsRequest {
    constructor(id) {
        this.Id = id;
    }
}

class AddGuestToBasketItemRequest {
    constructor(itemId, itemIdFrom, guestId) {
        this.ItemId = itemId;
        this.ItemIdFrom = itemIdFrom;
        this.GuestId = guestId;
    }
}

class DataTableRow {
    constructor(id, reference, bookingDate, description, itemQuantity){
        this.Id = id;
        this.Reference = reference;
        this.BookingDate = bookingDate;
        this.Description = description;
        this.ItemQuantity = itemQuantity;
    }
}
