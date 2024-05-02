$('.roomToggleButton').on('click', function () {
    if ($(this).hasClass('toggled')) {
        $(this).removeClass('toggled');
        $(this).html('<i class="fa-solid fa-circle-arrow-down"></i> Show Details');
    }
    else {
        $(this).addClass('toggled');
        $(this).html('<i class="fa-solid fa-circle-arrow-up"></i> Hide Details');
    }
});

$('.js-room-quantity').on('change', function() {
    let roomTypeId = $(this).data('room-type-id');
    let roomTypeName = $(this).data('room-type-name');
    let cloudbedsPropertyId = $(this).data('cloudbeds-property-id');
    let roomIsShared = $(this).data('room-is-shared') == 'True';

    let bedQuantityControl = $('.js-room-bed-quantity[data-room-type-id=' + roomTypeId + ']');
    let adultQuantityControl = $('.js-room-adults-quantity[data-room-type-id=' + roomTypeId + ']');
    let childQuantityControl = $('.js-room-children-quantity[data-room-type-id=' + roomTypeId + ']');

    let roomQuantity= bedQuantityControl.length && bedQuantityControl.val().length > 0 ? bedQuantityControl.val() : 0;
    let adultQuantity= adultQuantityControl.length && adultQuantityControl.val().length > 0  ? adultQuantityControl.val() : 0;
    let childQuantity = childQuantityControl.length && childQuantityControl.val().length > 0  ? childQuantityControl.val() : 0;

    let rooms = serializedData.propertyRooms;
    
    let room = $.grep(rooms, function (item) {
        return item["roomTypeID"] === roomTypeId;
    })[0];
    
    let request = new BasketItemRoomModel(roomTypeId, roomTypeName, roomIsShared, roomQuantity, room.roomRate, adultQuantity, childQuantity, cloudbedsPropertyId)
    
    doPost({
        url : "AddRoomToBasket",
        formData: request,
        successCallback: (result) => {
            let basket = result.basket;
            let item = result.item;
            let itemsCount = basket.itemsCount;

            $('.basketItemsNavQuantity').html(itemsCount).removeClass('d-none');
            $('#basketTotal').html('$ ' + basket.total.toFixed(2));
            $('#selectionTotal').html('$ ' + item.total.toFixed(2));
            $('#selectionRoomCount').html(item.rooms.length + ' rooms');
        }
    });
});