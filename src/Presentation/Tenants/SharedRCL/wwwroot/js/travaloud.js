const $dropdown = $(".dropdown");
const $dropdownToggle = $(".dropdown-toggle");
const $dropdownMenu = $(".dropdown-menu");
const $showClass = "show";
let $windowWidth = null;
let $cookieName = null;

const initializeTravaloud = (options) => {
    const {
        onLoadCallbacks,
        onResizeCallbacks,
        onScrollCallbacks,
        onReadyCallbacks,
        cookieName 
    } = options;

    $cookieName = cookieName;
    
    $("document").ready(function() {
        bindSelectsAndModals();
        
        if (onReadyCallbacks !== undefined)
            executeCallbacks(onReadyCallbacks);
    });

    $(window).on('beforeunload', function () {
        showLoader();
        window.scrollTo(0, 0);
    });

    $(window).on("scroll", function () {
        adjustNavbar();

        $(".has-parallax-scroll").each(function () {
            let position = $(this).offset().top;
            let scrollPosition = $(window).scrollTop() + $(window).height();
            if (scrollPosition > position) {
                $(this).addClass("fade-in");
            }
        });

        if (onScrollCallbacks !== undefined)
            executeCallbacks(onScrollCallbacks);
    });

    $(window).on("resize", function () {
        adjustNavbar();
        setNavigationDropdownHover();
        
        if (onResizeCallbacks !== undefined)
            executeCallbacks(onResizeCallbacks);
    });

    $(window).on("load", function () {
        // $('.select-input.active, .form-control.active').trigger('focus');

        setMultiItemCarousel();
        adjustNavbar();
        setNavigationDropdownHover();
        configureDatepickers();
        configureSelects();
        getCookie();

        if (onLoadCallbacks !== undefined)
            executeCallbacks(onLoadCallbacks);

        $('.navbar-toggler').on('click', function () {
            $('.animated-icon3').toggleClass('open');

            let navbar = $('.navbar');
            if (navbar.hasClass('toggled')) {
                window.setTimeout(function () {
                    navbar.removeClass('toggled');

                    if (navbar.hasClass('shadow')) {
                        $('.navbar-toggler i').addClass('text-dark').removeClass('text-white');
                        $('.animated-icon3 span').css('background', '#090511');
                    }
                }, 200);

            }
            else {
                navbar.addClass('toggled');
                $('.navbar-toggler i').addClass('text-white').removeClass('text-dark');
                $('.animated-icon3 span').css('background', '#FFFFFF');
                $('.navbar .nav-link').addClass('text-white');
            }
        });

        window.setTimeout(function () {
            history.replaceState("", document.title, window.location.pathname);
        }, 0);

        window.setTimeout(function () {
            window.scrollTo(0, 0);
        }, 100);

        window.setTimeout(function () {
            hideLoader();

            $(".has-parallax-scroll").each(function () {
                if ($(this).is(':visible')) {
                    let position = $(this).offset().top;
                    let scrollPosition = $(window).scrollTop() + $(window).height();
                    if (scrollPosition > position) {
                        $(this).addClass("fade-in");
                    }
                }
            });
        }, 800);

        $('.accordion-button').on('click', function () {
            $('.accordion-collapse').removeClass('show');
            $('.accordion-button').addClass('collapsed');

            let icon = $(this).find('i');
            if (icon.hasClass('fa-caret-down'))
                icon.removeClass('fa-caret-down').addClass('fa-caret-up');
            else
                icon.removeClass('fa-caret-up').addClass('fa-caret-down');

            $('.accordion-button i').not(icon).removeClass('fa-caret-up').addClass('fa-caret-down');
        });

        $('#bookNowForm, #bookNowModalForm').on('submit', function () {
            validateSelectElements($(this));
        });

        $('#download-data').on('submit', function () {
            setTimeout(function () {
                hideLoader();
            }, 1000);
        });

        $('#loginModalForm').submit(function (e) {
            if (!$(this).attr('validated')) {
                if ($(this).valid()) {
                    let loginModalForm =  $('#loginModalForm');
                    let loginModalButton = $('#loginModalButton');

                    loginModalButton.prop('disabled', true);
                    
                    doPost({
                        url : "ValidateUser",
                        formData: { "Username": $('#LoginModal_Email').val(), "Password": $('#LoginModal_Password').val() },
                        successCallback: (result) => {
                            loginModalForm.attr('validated', true);
                            loginModalForm.submit();
                            loginModalButton.prop('disabled', false);
                        },
                        failCallback: (result) => {
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            loginModalButton.prop('disabled', false);
                            
                            return false;
                        }
                    });

                    return false;
                }
            }

            return true;
        });

        $('#registerModalForm').submit(function (e) {
            if (!$(this).attr('validated')) {
                if ($(this).valid()) {
                    let registerModalForm =  $('#registerModalForm');
                    let registerModalButton = $('#registerModalButton');

                    registerModalButton.prop('disabled', true);

                    doPost({
                        url : "CheckIfUserExists",
                        formData: { "Username": $('#RegisterModal_Email').val(), "Password": $('#RegisterModal_Password').val() },
                        successCallback: (result) => {
                            registerModalForm.attr('validated', true);
                            registerModalForm.submit();
                            registerModalButton.prop('disabled', false);
                        },
                        failCallback: (result) => {
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            registerModalButton.prop('disabled', false);
                            
                            return false;
                        }
                    });

                    return false;
                }
            }

            return true;
        });

        $('#feedbackAlert button').on('click', function () {
            mdb.Alert.getInstance(document.getElementById('feedbackAlert')).hide();
        });

        let backToTopButton = document.getElementById("btn-back-to-top");

        window.onscroll = function () {
            scrollFunction();
        };

        function scrollFunction() {
            if (
                document.body.scrollTop > 20 ||
                document.documentElement.scrollTop > 20
            ) {
                backToTopButton.style.display = "block";
            } else {
                backToTopButton.style.display = "none";
            }
        }

        backToTopButton.addEventListener("click", backToTop);

        function backToTop() {
            document.body.scrollTop = 0;
            document.documentElement.scrollTop = 0;
        }
    });
}

const setMultiItemCarousel = () => {
    let owlCarousels = $('.owl-carousel');

    if (owlCarousels.length > 0) {
        owlCarousels.each(function () {
            let carousel = $(this);
            
            let itemsCount = carousel.find('.card').length;

            carousel.owlCarousel({
                loop: true,
                margin: 20,
                responsiveClass: true,
                responsive: {
                    0: {
                        items: 1,
                        nav: true
                    },
                    768: {
                        items: 2,
                        nav: true,
                    },
                    1415: {
                        items: itemsCount >= 3 ? 3 : itemsCount,
                        nav: true,
                        loop: false
                    }
                },
                onInitialized: function (event) {
                    let element = jQuery(event.target);
                    let idx = event.item.index;
                    element.find('.owl-item').addClass('col-xs-12 col-md-12 col-lg-4');
                    element.find('.owl-stage').addClass('row gx-2');
                    element.find('.owl-prev').attr('role', 'button').attr('title', 'Previous').attr('aria-label', 'Previous');
                    element.find('.owl-next').attr('role', 'button').attr('title', 'Next').attr('aria-label', 'Next');
                    element.find('.owl-prev,.owl-next').attr('tabindex', '0');
                    element.find('.owl-dot').attr('title', 'Owl carousel navigation button').attr('aria-label', 'Owl carousel navigation button');
                }
            });
        });
    }
}

const executeCallbacks = (callbacks) => {
    callbacks.forEach(function(callback) {
        callback();
    });
}

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

                let control = $(v).find('.form-control');

                $(control).on('valueChanged.mdb.datepicker', (e) => {
                    console.log(e);
                })
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

const setMobileNav = () => {
    if (isMobile())
        $('.navbar, .sticky-top').addClass('shadow');
    else
        $('.navbar, .sticky-top').removeClass('shadow');
};

const setMobileCarouselImages = () => {

    if (isMobile()) {
        $('#fullPageCarousel').find('.carousel-item').each(function () {
            let backgroundImage = $(this).css('background-image');
            backgroundImage = backgroundImage.replace('w-2000', 'w-1000');

            $(this).css('background-image', backgroundImage);
        });
    }
    else {
        $('#fullPageCarousel').find('.carousel-item').each(function () {
            let backgroundImage = $(this).css('background-image');
            backgroundImage = backgroundImage.replace('w-1000', 'w-2000');

            $(this).css('background-image', backgroundImage);
        });
    }
}

const getWindowWidth = () => {
    return $(window).width();
}

const isMobile = () => {
    return getWindowWidth() <= 992;
}

const setNavigationDropdownHover = () => {
    $windowWidth = getWindowWidth();

    if ($windowWidth > 992) {
        $dropdown.hover(
            function () {
                const $this = $(this);
                $this.addClass($showClass);
                $this.find($dropdownToggle).attr("aria-expanded", "true");
                $this.find($dropdownMenu).addClass($showClass);
            },
            function () {
                const $this = $(this);
                $this.removeClass($showClass);
                $this.find($dropdownToggle).attr("aria-expanded", "false");
                $this.find($dropdownMenu).removeClass($showClass);
            }
        );
    } else {
        $dropdown.off("mouseenter mouseleave");
    }
}

const showLoader = () => {
    $('.pre-loader').show();
    $('html').addClass('no-scrollbar');
}

const hideLoader = () => {
    $('html').removeClass('no-scrollbar');
    $('.pre-loader').hide()
}

const adjustNavbar = () => {
    let navbar = $('.navbar');
    let navbarCollapse = $('.navbar-collapse');
    let navLinks = $('.navbar .nav-link');
    let stickyTop = $('.sticky-top');

    // if (isMobile()) {
    //     navbarCollapse.addClass('vh-100')
    // }
    // else
    //     navbarCollapse.removeClass('vh-100')
    //

    if (!navbar.hasClass('always-shadow')) {
        if ($(this).scrollTop() > 86) {
            $('.fixed-top').addClass('shadow');

            if (!navbar.hasClass('toggled')) {
                $('.navbar-toggler i').addClass('text-dark').removeClass('text-white');
                $('.animated-icon3').find('span').css('background', '#090511');
            }
            else
                navLinks.addClass('text-white');
        } else {
            if (!navbarCollapse.hasClass('show')) {
                $('.navbar-toggler i').removeClass('text-dark');
                $('.animated-icon3 span').css('background', '#FFFFFF');
                $('.fixed-top').removeClass('shadow');
            }

            if (!isMobile()) {
                navbarCollapse.removeClass('show');
                navbar.removeClass('toggled').removeClass('bg-white').removeClass('shadow');
                navLinks.removeClass('text-white');
            }

        }
    }
    else {
        if (!isMobile()) {
            if (navbar.hasClass('toggled')) {
                navbarCollapse.removeClass('show');
                navbar.removeClass('toggled').addClass('bg-white');
                navLinks.removeClass('text-white');
            }
            else {
                navLinks.removeClass('text-white');
            }
        }
        else {
            if (!navbar.hasClass('toggled')) {
                navbar.removeClass('toggled')
                $('.navbar-toggler i').addClass('text-dark').removeClass('text-white');
                $('.animated-icon3 span').css('background', '#090511');
            }
            //    
            //else
            //    $('.navbar .nav-link').addClass('text-white');
        }
    }

    window.setTimeout(function () {
        if (navbar.hasClass('always-shadow')) {
            let navbarHeight = $('.navbar').outerHeight().toFixed(2) + "px !important;";
            $('main').attr('style', 'padding-top:' + navbarHeight);
        }
    }, 200);


    if ($('.is-pinned').is(':visible'))
        navbar.addClass('no-shadow');

    if (stickyTop.length > 0) {
        let eTop = stickyTop.offset().top;
        let eTopWindow = eTop - $(window).scrollTop();
        if (eTopWindow === 81 || isMobile() && eTopWindow === 76) {
            navbar.addClass('no-shadow');
            stickyTop.addClass('is-pinned').addClass('bg-white');
        }
        else {
            navbar.removeClass('no-shadow');
            stickyTop.removeClass('is-pinned').removeClass('bg-white');
        }
    }
}

const configureSelects = () => {
    $('.select').each(function () {
        this.addEventListener('valueChange.mdb.select', (e) => {
            let id = e.target.parentElement.id;
            let inputField = $('#' + id).find('.form-control.select-input');

            if (!$(inputField).hasClass('active'))
                $(inputField).addClass('active');
        });

        let value = $(this).val();
        if (value !== undefined && value.length > 0) {
            let inputField = $(this).parent().find('.form-control.select-input');

            if (!$(inputField).hasClass('active'))
                $(inputField).addClass('active');
        }
    });
}

const configureDatepickers = () => {
    $('.datepicker-disable-future').each(function () {
        let datepicker = new mdb.Datepicker(this, {
            disableFuture: true,
            toggleButton: false
        });
    });

    $('.datepicker-disable-past').each(function () {
        let datepicker = new mdb.Datepicker(this, {
            disablePast: true,
            toggleButton: false
        });

        this.addEventListener('dateChange.mdb.datepicker', (e) => {
            let id = e.srcElement.children[0].id;
            let date = e.date;

            let correspondingId = ""

            switch (id) {
                case "CarouselComponent_CheckInDate":
                    correspondingId = "CarouselComponent_CheckOutDate"
                    break;
                case "BookNowBanner_CheckInDate":
                    correspondingId = "BookNowBanner_CheckOutDate"
                    break;
                case "CheckInDate":
                    correspondingId = "CheckOutDate"
                    break;
                case "CarouselComponent_CheckOutDate":
                    correspondingId = "CarouselComponent_CheckInDate"
                    break;
                case "BookNowBanner_CheckOutDate":
                    correspondingId = "BookNowBanner_CheckInDate"
                    break;
                case "CheckOutDate":
                    correspondingId = "CheckInDate"
                    break;
            }

            if (id === "CarouselComponent_CheckInDate" || id === "BookNowBanner_CheckInDate" || id === "CheckInDate") {
                let minDate = date.addDays(1);
                let checkOutDatepicker = document.getElementById(correspondingId).parentElement;
                let instance = mdb.Datepicker.getInstance(checkOutDatepicker);

                if (instance) {
                    instance.dispose();
                    instance = new mdb.Datepicker(checkOutDatepicker, {
                        disablePast: true,
                        toggleButton: false,
                        min: minDate,
                        startDate: minDate
                    });
                }
            }
            else if (id === "CarouselComponent_CheckOutDate" || id === "BookNowBanner_CheckOutDate" || id === "CheckOutDate") {
                let maxDate = date;
                let checkInDatePicker = document.getElementById(correspondingId).parentElement;
                let instance = mdb.Datepicker.getInstance(checkInDatePicker);

                if (instance) {
                    instance.dispose();
                    instance = new mdb.Datepicker(checkInDatePicker, {
                        disablePast: true,
                        toggleButton: false,
                        max: maxDate
                    });
                }
            }
        })
    });
}

const getCookie = () => {
    let cookie = Cookie.get($cookieName);

    if (cookie === undefined) {
        window.setTimeout(function () {
            let cookiesModalEl = document.getElementById('cookiesModal');
            let modal = new mdb.Modal(cookiesModalEl);
            modal.show();
        }, 2000);
    }
}

const confirmCookie = () => {
    Cookie.set($cookieName, new Date(), {expires: 365});
    $('#cookiesModal').modal('hide');
}

const getBaseUrl = () => {
    let pathArray = location.href.split('/');
    let protocol = pathArray[0];
    let host = pathArray[2];
    let url = protocol + '//' + host + '/';

    return url;
}

const doPost = (options) => {
    const {
        url,
        formData,
        successCallback,
        failCallback,
        skipLoader 
    } = options;

    $.ajax({
        type: "POST",
        url: location.href + "?handler=" + url,
        data: JSON.stringify(formData),
        dataType: 'json',
        contentType: 'application/json',
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            const value = result.value != null ? result.value : result;

            if (successCallback !== undefined && result.success)
                successCallback(value);

            if (failCallback !== undefined && !result.success)
                failCallback(value);

            if (result.modalMessage != null) {
                showAlertModal(result.modalMessage, result.success);
            }

            if (!skipLoader) {
                hideLoader();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(xhr, ajaxOptions, thrownError);

            if (result.modalMessage != null) {
                showAlertModal("There was an error submitting your request. Please try again, or contact us.", false);
            }
        }
    });
}

const showAlertModal = (message, success) => {
    $('#ajaxFeedbackToastBody').html(message);
    
    if (!success)
    {
        $('#ajaxFeedbackToastImage').removeClass('text-success, fa-circle-check').addClass('text-danger, fa-circle-exclamation');
        $('#ajaxFeedbackHeader').html('Error');
    }
    else {
        $('#ajaxFeedbackToastImage').removeClass('text-danger, fa-circle-exclamation').addClass('text-success, fa-circle-check');
        $('#ajaxFeedbackHeader').html('Success');
    }

    let toastInstance = mdb.Toast.getInstance(document.getElementById('ajaxFeedbackToast'));
    toastInstance.show();
}

const postAjax = function (url, formData, callback, skipLoader = false) {
    $.ajax({
        type: "POST",
        url: "/?handler=" + url,
        data: JSON.stringify(formData),
        dataType: 'json',
        contentType: 'application/json',
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            callback(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log(xhr, ajaxOptions, thrownError);
        }
    });
}

Date.prototype.addDays = function (days) {
    let date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
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