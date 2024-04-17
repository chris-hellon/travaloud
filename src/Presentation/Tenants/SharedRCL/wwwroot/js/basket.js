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