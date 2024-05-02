$("document").ready(function() {
    validateAdditionalGuests(); 
});

$('#checkoutForm').on('submit', function(e) {
    let valid = $(this).valid();

    if (!validateSelectElements($(this), valid)) {
        e.preventDefault();
        return false;
    }
});

const addNewGuestToBasketItem = (button) =>
{
    let itemId =  $('#addNewGuestModal').data('basket-item-id');
    let form = $('#checkoutAddGuestForm');
    form.removeData("validator");
    form.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);
    
    let valid = form.valid();

    if (valid)
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
        
        doPost({
            url : "AddGuestToBasketItem",
            formData: request,
            successCallback: (result) => {
                loadBasket(result.basket);
                $('#addNewGuestModal .btn-close').trigger('click');
                $('.additional-guests-container').replaceWith(result.html);
                $('.add-new-guest-modal-body').html('');
                validateAdditionalGuests();
            }
        });
    }
}

const removeGuestFromBasketItem = (itemId, id) => {
    let request = new BasketItemGuestRequest(itemId, id);

    doPost({
        url : "RemoveGuestFromBasketItem",
        formData: request,
        successCallback: (result) => {
            loadBasket(result.basket);
            $('.additional-guests-container').replaceWith(result.html);
            validateAdditionalGuests();
        }
    });
}

const editBasketItemGuest = (itemId, id) => {
    let request = new BasketItemGuestRequest(itemId, id);

    doPost({
        url : "EditBasketItemGuest",
        formData: request,
        successCallback: (result) => {
            $('.add-new-guest-modal-body').html(result.html);

            bindAddNewGuestModalInputs();
            bindSelectsAndModals();
        }
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
        
        doPost({
            url : "UpdateBasketItemGuest",
            formData: request,
            successCallback: (result) => {
                loadBasket(result.basket);
                $('#addNewGuestModal .btn-close').trigger('click');
                $('.additional-guests-container').replaceWith(result.html);
                $('.add-new-guest-modal-body').html('');
            }
        });
    }
}

const loadNewGuestModal = () =>
{
    doPost({
        url : "GetAddNewGuestModal",
        formData: null,
        successCallback: (result) => {
            $('.add-new-guest-modal-body').html(result.html);

            bindAddNewGuestModalInputs();
            bindSelectsAndModals();
        }
    });
}

const loadSelectGuestModal = (itemId) => {
    $('#addGuestModal').data('basket-item-id', itemId);

    let request = new GetBasketItemGuestsRequest(itemId);

    doPost({
        url : "GetSelectGuestModal",
        formData: request,
        successCallback: (result) => {
            $('.select-guest-modal-body').html(result.html);
        }
    });
}

const addExistingGuestToBasketItem = (itemIdFrom, guestId) => {
    let itemId = $('#addGuestModal').data('basket-item-id');
    let request = new AddGuestToBasketItemRequest(itemId, itemIdFrom, guestId);
    
    doPost({
        url : "AddExistingGuestToBasketItem",
        formData: request,
        successCallback: (result) => {
            loadBasket(result.basket);
            $('.additional-guests-container').replaceWith(result.html);
            $('#addGuestModal .btn-close').trigger('click');
            validateAdditionalGuests();
        }
    });
}

const validateAdditionalGuests = () => {
    let valid = true;
    
    $('.additional-guests-container').each(function() {
        let remainingGuestsCount = $(this).data('remaining-guests-count');
        if (remainingGuestsCount > 0)
            valid = false;
    });
    
    if (valid)
    {
        $('.checkout-payment-button').prop('disabled', false);
    }
    else
    {
        $('.checkout-payment-button').prop('disabled', true);
    }
}