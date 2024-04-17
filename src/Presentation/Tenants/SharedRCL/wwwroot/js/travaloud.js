$("window").on('load', function() {
    $('.select-input.active, .form-control.active').trigger('focus');
});

$("document").ready(function() {
    bindSelectsAndModals();
});

const bindSelectsAndModals = () => {
    let confirmDateOnSelectPickers = $('.confirm-date-on-select');

    if (confirmDateOnSelectPickers.length)
    {
        confirmDateOnSelectPickers.each(function(i, v) {
            if (!$(v).hasClass('select-tour-date-picker'))
            {
                new mdb.Datepicker(v, { 
                    confirmDateOnSelect: true, 
                    disableFuture: $(v).hasClass('datepicker-disable-future'),
                    disablePast: $(v).hasClass('datepicker-disable-past')
                });
            }
        });
    }
    
    $('.datepicker-disable-past, .datepicker-disable-future, .datepicker-with-filter, .confirm-date-on-select, .datepicker-close-on-select').on('close.mdb.datepicker', function () {
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

    let dateInputFields = $('.js-date-input-field');

    dateInputFields.on('input', function() {
        let inputValue = $(this).val();
        if (inputValue.length === 2 || inputValue.length === 5) {
            // If the user has entered two characters for day or month, append '/'
            $(this).val(inputValue + '/');
        }
        // If the input is longer than 10 characters, truncate it to 10 characters
        if (inputValue.length > 10) {
            $(this).val(inputValue.slice(0, 10));
        }
    });

    // Function to enforce DD/MM/YYYY format
    dateInputFields.on('blur', function() {
        let inputValue = $(this).val();
        if (!/^\d{2}\/\d{2}\/\d{4}$/.test(inputValue)) {
            // If the input is not in the format DD/MM/YYYY, clear the input
            $(this).val('');
        }
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

$('input[type="number"]').on('input', function() {
    let maxAttr = $(this).attr('max');

    if (typeof maxAttr !== typeof undefined && maxAttr !== false) {
        let maxVal = parseFloat(maxAttr);
        let enteredVal = parseFloat($(this).val());
        if (!isNaN(enteredVal) && enteredVal > maxVal) {
            $(this).val(maxVal);
        }
    }
});

