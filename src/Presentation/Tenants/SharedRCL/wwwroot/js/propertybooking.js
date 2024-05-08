$('.roomToggleButton').on('click', function () {
    if ($(this).hasClass('toggled')) {
        $(this).removeClass('toggled');
        $(this).html('<i class="fa-solid fa-circle-arrow-down"></i> Show Details');
    } else {
        $(this).addClass('toggled');
        $(this).html('<i class="fa-solid fa-circle-arrow-up"></i> Hide Details');
    }
});

$('.js-room-quantity').on('change', function () {
    let roomTypeId = $(this).data('room-type-id');
    let roomTypeName = $(this).data('room-type-name');
    let cloudbedsPropertyId = $(this).data('cloudbeds-property-id');
    let roomIsShared = $(this).data('room-is-shared') == 'True';

    let bedQuantityControl = $('.js-room-bed-quantity[data-room-type-id=' + roomTypeId + ']');
    let adultQuantityControl = $('.js-room-adults-quantity[data-room-type-id=' + roomTypeId + ']');
    let childQuantityControl = $('.js-room-children-quantity[data-room-type-id=' + roomTypeId + ']');

    let roomQuantity = bedQuantityControl.length && bedQuantityControl.val().length > 0 ? bedQuantityControl.val() : 0;
    let adultQuantity = adultQuantityControl.length && adultQuantityControl.val().length > 0 ? adultQuantityControl.val() : 0;
    let childQuantity = childQuantityControl.length && childQuantityControl.val().length > 0 ? childQuantityControl.val() : 0;

    let rooms = serializedData.propertyRooms;

    let room = $.grep(rooms, function (item) {
        return item["roomTypeID"] === roomTypeId;
    })[0];

    let request = new BasketItemRoomModel(roomTypeId, roomTypeName, roomIsShared, roomQuantity, room.roomRate, adultQuantity, childQuantity, cloudbedsPropertyId)

    doPost({
        url: "AddRoomToBasket",
        formData: request,
        successCallback: (result) => {
            let basket = result.basket;
            let item = result.item;
            let itemsCount = basket.itemsCount;

            $('.basketItemsNavQuantity').html(itemsCount).removeClass('d-none');
            $('#basketTotal').html('$ ' + basket.total.toFixed(2));
            $('#selectionTotal').html('$ ' + item.total.toFixed(2));
            $('#selectionRoomCount').html(item.rooms.length + ' rooms');

            if (itemsCount > 0)
                $('#btnProceedToCheckout').prop('disabled', false);
            else {
                $('#btnProceedToCheckout').prop('disabled', true);
            }
        }
    });
});


$('.js-tour-date-select').on('change', function () {
    let tourId = $(this).data('tour-id');
    let tourPriceSelect = $('.js-tour-time-select[data-tour-id=' + tourId + ']');
    let tourDatesParsed = $(this).data('tour-dates');

    let parts = $(this).val().split(/\/|\s|:/);
    let tourDate = new Date(parts[2], parts[1] - 1, parts[0], parts[3], parts[4], parts[5]);

    let tourDateFormatted = tourDate;
    tourDateFormatted.setHours(0, 0, 0);

    tourPriceSelect.html('<option value="" hidden selected=""></option>').prop('disabled', true);
    
    if (tourDatesParsed.length > 0) {
        $(tourDatesParsed).each(function (i, v) {
            let startDate = new Date(v.StartDate);

            if (startDate.getDate() === tourDateFormatted.getDate())
            {
                console.log(v);
                let tourDateTimeParsed = moment(startDate).format('HH:mm');
                tourPriceSelect.append('<option data-start-date="' + v.StartDate + '" data-tour-price="' + v.TourPrice.Price + '" data-max-guests="' + v.AvailableSpaces + '" value="' + v.Id + '">' + tourDateTimeParsed + '</option>')
            }
        });
        
        tourPriceSelect.prop('disabled', false);
    }
});

$('.js-tour-time-select').on('change', function () {
    let tourId = $(this).data('tour-id');
    let tourDateId = $(this).val();
    let maxSpaces = $(this).find('option:selected').data('max-guests');
    let tourPrice = $(this).find('option:selected').data('tour-price');
    let startDate = $(this).find('option:selected').data('start-date');
    let tourDatesParsed = $('.js-tour-date-select[data-tour-id=' + tourId + ']').data('tour-dates');
    
    alert(startDate);
    
    let guestQuantityControl = $('.js-tour-guest-quantity-select[data-tour-id=' + tourId + ']');
    let tourName = guestQuantityControl.data('tour-name');
    let tourImageUrl = guestQuantityControl.data('tour-image-url');
    
    guestQuantityControl.attr('max', maxSpaces).prop('disabled', false);
    guestQuantityControl.on('blur', function() {
        addTourToBasket(tourId, tourName, tourImageUrl, tourDateId, guestQuantityControl.val(), tourPrice, startDate);
    });
});

const addTourToBasket = (tourId, tourName, tourImageUrl, tourDateId, guestQuantity, tourPrice, startDate) => {
    let request = new BasketItemDateModel(tourDateId, tourId, tourName, tourImageUrl, guestQuantity, tourPrice, new Date(startDate));

    console.log(request);
    
    doPost({
        url : "AddTourDateToBasket",
        formData: request,
        successCallback: (result) => {
            let basket = result.basket;
            let item = result.item;
            let itemsCount = basket.itemsCount;

            $('.basketItemsNavQuantity').html(itemsCount).removeClass('d-none');
            $('#basketTotal').html('$ ' + basket.total.toFixed(2));
            $('#selectionTotal').html('$ ' + item.total.toFixed(2));

            // let proceedToCheckoutButton = $('.proceed-to-checkout-button');
            // let addToBasketButton = $('.add-tour-to-basket-button');
            //
            // if (addToBasketButton.html() !== 'Update Basket')
            // {
            //     let guestQuantityControl = $('#GuestQuantity');
            //     guestQuantityControl.parent().find('.form-helper').html('Select a Date');
            //
            //     $('#TourDate, #TourDateStartTime').val('').removeClass('active');
            //     $('#TourDate').prop('disabled', false);
            //     guestQuantityControl.val('').prop('disabled', true).removeClass('active');
            //     $('#TourDateStartTime').html('<option value="" hidden></option>').val('').prop('disabled', true);
            // }
            //
            // addToBasketButton.prop('disabled', true);
            // proceedToCheckoutButton.prop('disabled', false);
            // proceedToCheckoutButton.on('click', function() {
            //     showLoader();
            //     window.location.href = "/checkout";
            // })
        }
    });
}