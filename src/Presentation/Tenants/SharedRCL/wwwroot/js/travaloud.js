$("window").on('load', function() {
    $('.select-input.active, .form-control.active').trigger('focus');
});

$("document").ready(function() {
    bindSelectsAndModals();
});

const bindSelectsAndModals = () => {
    $('.datepicker-disable-past, .datepicker-disable-future').on('close.mdb.datepicker', function () {
        $('.datepicker-backdrop').remove();
        window.setTimeout(function () {
            $('.datepicker-modal-container').remove();

            $('body').css({
                "overflow": "unset",
                "padding-right": "unset",
                "overflow-y": "overlay",
                "overflow-x": "hidden"
            });
        }, 200);
    });

    $('.modal').on('hidden.mdb.modal', function () {
        $('body').css({
            "overflow": "unset",
            "padding-right": "unset",
            "overflow-y": "overlay",
            "overflow-x": "hidden"
        });
    });
}

const validateSelectElements = (form, valid) => {
    let $formSelect = form.find('.select');

    if ($formSelect.length > 0)
        $formSelect.each(function (index, select) {
            let selectValid = validateSelectElement(select);

            if (!selectValid && valid)
                valid = false;
        });

    return valid;
}

const validateSelectElement = (select) => {
    let selectValue = $(select).val();
    let validationMessage = $(select).data('val-required');
    let name = $(select).attr('name');

    let validationSpan = $('span[data-valmsg-for="' + name + '"]');

    if (selectValue == null || selectValue.length === 0) {
        validationSpan.removeClass('field-validation-valid').addClass('field-validation-error').html('<span id="' + name + '-error" class="">' + validationMessage + '</span>');

        return false;
    }
    else validationSpan.removeClass('field-validation-error').addClass('field-validation-valid').html('');


    return true;
}
