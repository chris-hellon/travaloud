const updateBasket = (id) => {
    let guestQuantity = $('.tourDateGuestQuantity[data-id="' + id + '"]').val();

    $(basket.items).each(function (i, v) {
        $(v.tourDates).each(function (i, d) {
            if (d.id == id)
                d.guestQuantity = parseInt(guestQuantity);
        });
    });

    postBasket();
}

const addNewGuestToBasketItem = (button) =>
{
    let itemId =  $('#addNewGuestModal').data('basket-item-id');
    let form = $('#checkoutAddGuestForm');
    
    let valid = form.valid();
    
    if (validateSelectElements(form, valid))
    {
        $(button).prop('disabled', true);
        
        let firstName = $('#addNewGuestModal #FirstName').val();
        let surname = $('#addNewGuestModal #Surname').val();
        let email = $('#addNewGuestModal #Email').val();
        let dateOfBirth = $('#addNewGuestModal #DateOfBirth').val();
        let phoneNumber = $('#addNewGuestModal #PhoneNumber').val();
        let nationality = $('#addNewGuestModal #Nationality').val();
        let gender = $('#addNewGuestModal #Gender').val();
        
        let request = new BasketItemGuestModel(
            itemId,
            firstName,
            surname,
            email,
            moment(dateOfBirth, "DD/MM/YYYY"),
            phoneNumber,
            nationality,
            gender
        )
        
        postAjax("AddGuestToBasketItem", request, function (result) {
            loadBasket(result.basket);
            $('#addNewGuestModal .btn-close').trigger('click');
            $('.additional-guests-container').replaceWith(result.html);
            $('.add-new-guest-modal-body').html('');
        });
    }
}

const removeGuestFromBasketItem = (itemId, id) => {
    let request = new BasketItemGuestRequest(itemId, id);

    postAjax("RemoveGuestFromBasketItem", request, function (result) {
        loadBasket(result.basket);
        $('.additional-guests-container').replaceWith(result.html);
    });
}

const editBasketItemGuest = (itemId, id) => {
    let request = new BasketItemGuestRequest(itemId, id);

    postAjax("EditBasketItemGuest", request, function (result) {
        $('.add-new-guest-modal-body').html(result.html);

        bindAddNewGuestModalInputs();
        bindSelectsAndModals();
    });
}

const updateBasketItemGuest = (button, id) => {
    let itemId =  $('#addNewGuestModal').data('basket-item-id');
    let form = $('#checkoutAddGuestForm');

    let valid = form.valid();

    if (validateSelectElements(form, valid))
    {
        $(button).prop('disabled', true);
        
        let firstName = $('#addNewGuestModal #FirstName').val();
        let surname = $('#addNewGuestModal #Surname').val();
        let email = $('#addNewGuestModal #Email').val();
        let dateOfBirth = $('#addNewGuestModal #DateOfBirth').val();
        let phoneNumber = $('#addNewGuestModal #PhoneNumber').val();
        let nationality = $('#addNewGuestModal #Nationality').val();
        let gender = $('#addNewGuestModal #Gender').val();

        let request = new BasketItemGuestModel(
            itemId,
            firstName,
            surname,
            email,
            moment(dateOfBirth, "DD/MM/YYYY"),
            phoneNumber,
            nationality,
            gender,
            id
        )

        postAjax("UpdateBasketItemGuest", request, function (result) {
            loadBasket(result.basket);
            $('#addNewGuestModal .btn-close').trigger('click');
            $('.additional-guests-container').replaceWith(result.html);
            $('.add-new-guest-modal-body').html('');
        });
    }
}

const loadNewGuestModal = () =>
{
    postAjax("GetAddNewGuestModal", null, function (result) {
        $('.add-new-guest-modal-body').html(result.html);
        
        bindAddNewGuestModalInputs();
        bindSelectsAndModals();
    });
}

const loadSelectGuestModal = (itemId) => {
    $('#addGuestModal').data('basket-item-id', itemId);
    
    let request = new GetBasketItemGuestsRequest(itemId);
    postAjax("GetSelectGuestModal", request, function (result) {
        $('.select-guest-modal-body').html(result.html);
    });
}

const addExistingGuestToBasketItem = (itemIdFrom, guestId) => {
    let itemId = $('#addGuestModal').data('basket-item-id');
    let request = new AddGuestToBasketItemRequest(itemId, itemIdFrom, guestId);

    postAjax("AddExistingGuestToBasketItem", request, function (result) {
        loadBasket(result.basket);
        $('.additional-guests-container').replaceWith(result.html);
        $('#addGuestModal .btn-close').trigger('click');
    });
}

const removeItemFromBasket = (id) =>
{
    let request = new BasketItemModel(id);

    postAjax("RemoveItemFromBasket", request, function (result) {
        $('.basketItem[data-id="' + id + '"], .basketItemSpacer[data-id="' + id + '"], .basketItemDetails[data-id="' + id + '"]').remove();
        loadBasket(result.basket);
    });
}

const loadBasket = (updatedBasket) => 
{
    basket = updatedBasket;
    $('#basketTotal').html('$ ' + parseFloat(updatedBasket.total).toFixed(2));

    let itemsCount = updatedBasket.itemsCount;
    let itemsLabel = itemsCount + ' item' + (itemsCount > 1 || itemsCount == 0 ? 's' : '');

    $('.basketItemsQuantity').html(itemsLabel);
    
    let basketItemsNavQuantity = $('.basketItemsNavQuantity');
    basketItemsNavQuantity.html(itemsCount);

    if (itemsCount === 0) {
        $('<p>Your basket is empty.</p>').insertAfter('.basketBodySpacer');
        basketItemsNavQuantity.addClass('d-none');
        $('#proceedToCheckoutButton').addClass('d-none');
    }
}

const postBasket = () => {
    postAjax("UpdateBasket", basket, function (result) {
        loadBasket(result.basket);
    });
}

$('.itemToggleButton').on('click', function () {
    if ($(this).hasClass('toggled')) {
        $(this).removeClass('toggled');
        $(this).html('<i class="fa-solid fa-circle-arrow-down"></i> Show Details');
    }
    else {
        $(this).addClass('toggled');
        $(this).html('<i class="fa-solid fa-circle-arrow-up"></i> Hide Details');
    }
});

const addGuestModal = $('#addNewGuestModal');
addGuestModal.on('show.mdb.modal', (e) => {
    let button = $(e.relatedTarget);
    let basketItemId = button.data('basket-item-id');
    addGuestModal.data('basket-item-id', basketItemId);
})

$('#checkoutTogglePassword').on('click', function() {
    if ($(this).is(':checked')) {
        $('.checkout-account-fields').removeClass('d-none');
    }
    else {
        $('.checkout-account-fields').addClass('d-none');
    }
});

const bindAddNewGuestModalInputs = () => {
    document.querySelectorAll('#addNewGuestModal .form-outline').forEach((formOutline) => {
        new mdb.Input(formOutline).init();
    });

    document.querySelectorAll('#addNewGuestModal .select').forEach((formOutline) => {
        if (formOutline.classList.contains('nationality-select')){
            new mdb.Select(formOutline, {
                filter : true,
                container: '#addNewGuestModal'
            });
        }
        else {
            new mdb.Select(formOutline);
        }
    });

    document.querySelectorAll('#addNewGuestModal .datepicker-disable-future').forEach((formOutline) => {
        const options = {
            disableFuture: true
        }
       
        new mdb.Datepicker(formOutline, options);
    });
}