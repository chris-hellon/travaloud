﻿const $dropdown = $(".dropdown");
const $dropdownToggle = $(".dropdown-toggle");
const $dropdownMenu = $(".dropdown-menu");
const $showClass = "show";
let $windowWidth = null;

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
});

$(window).on("resize", function () {
    bindFuseJs();
    adjustNavbar();
    setNavigationDropdownHover();
    //setMobileCarouselImages();
});

$(window).on("load", function () {
    bindFuseJs();
    adjustNavbar();
    setNavigationDropdownHover();
    configureDatepickers();
    configureSelects();
    setParallaxMarquee();
    getCookie();
    //setMobileCarouselImages();

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
                postAjax("ValidateUser", { "Username": $('#LoginModal_Email').val(), "Password": $('#LoginModal_Password').val() }, function (result) {
                    if (result.success !== true) {
                        e.preventDefault();
                        e.stopImmediatePropagation();

                        $('#feedbackAlertMessage').html(result.message);
                        mdb.Alert.getInstance(document.getElementById('feedbackAlert')).show();

                        loginModalButton.prop('disabled', false);

                        return false;
                    }
                    else {
                        loginModalForm.attr('validated', true);
                        loginModalForm.submit();
                    }

                    loginModalButton.prop('disabled', false);
                }, true);

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
                postAjax("CheckIfUserExists", { "Username": $('#RegisterModal_Email').val(), "Password": $('#RegisterModal_Password').val() }, function (result) {
                    if (result.success !== true) {
                        e.preventDefault();
                        e.stopImmediatePropagation();

                        $('#feedbackAlertMessage').html(result.message);
                        mdb.Alert.getInstance(document.getElementById('feedbackAlert')).show();

                        registerModalButton.prop('disabled', false);

                        return false;
                    }
                    else {
                        registerModalForm.attr('validated', true);
                        registerModalForm.submit();
                    }

                    registerModalButton.prop('disabled', false);
                }, true);

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

const bindFuseJs = ()=> {
    adjustCardMargain();
    adjustCircularImages();
    setMultiItemCarousel();

    let inheritStickyParent = $('.inherit-sticky-parent');
    if (inheritStickyParent.length > 0) {
        inheritStickyParent.each(function () {
            let stickyHeight = $('.sticky-top[data-mdb-sticky-boundary]').outerHeight();
            $(this).css('margin-top', '-' + stickyHeight + 'px');
            $(this).find('.offset-background').css('background-image', '-webkit-linear-gradient(-50deg, #FFFFFF 70%, #D1AC00 50%)');
        });
    }
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

Date.prototype.addDays = function (days) {
    let date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
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

const adjustCardMargain = () => {
    let alternatingCardHeight = $('.alternating-card-height');
    let lightboxCards = $('.lightbox-card');
    
    if (alternatingCardHeight.length > 0) {
        alternatingCardHeight.each(function () {
            let $marginBottom = 0;
            let $cards = $(this).find('.card');
            if ($cards.length > 0) {
                let cardsLength = $cards.length;
                $cards.each(function (i, v) {
                    if (i % 2 !== 0) {
                        if (isMobile()) {
                            $(v).parent('div').css('top', 'unset');
                            $(v).parent('div').css('margin-bottom', 'unset');
                        }
                        else {
                            if ($marginBottom === 0)
                                $marginBottom = parseFloat($(v).parent('div').css('marginBottom').replace('px', ''));

                            let $marginTop = 315 + $marginBottom;
                            $(v).parent('div').attr('style', 'margin-bottom: -' + $marginTop + 'px !important; top: -' + $marginTop + 'px !important;');
                        }
                    }
                });
            }
        });
    }

    if (lightboxCards.length > 0) {
        lightboxCards.each(function (i, v) {
            let cardsLength = lightboxCards.length;
            if (i % 2 !== 0) {
                if (isMobile()) {
                    $(v).css('top', 'unset');
                    $(v).css('margin-bottom', 'unset');
                }
                else {
                    let $marginBottom = $(v).css('marginBottom').replace('px', '');

                    let $marginTop = (315 + parseInt($marginBottom));

                    if ((i + 1) !== cardsLength)
                        $(v).attr('style', 'margin-bottom: -' + $marginTop + 'px !important; top: ' + $marginBottom + 'px !important;');
                    else
                        $(v).attr('style', 'padding-bottom' + $marginBottom + 'px !important;; top: ' + $marginBottom + 'px !important;');
                }
            }
        });
    }
}

const adjustCircularImages = () => {
    $('.full-circle').each(function () {
        let $image = $(this).find('.object-fit-cover');
        let $imageHeight = $image.height();
        let $parentWidth = $(this).parent().width();

        let $previousDivHeight = $(this).parent().prev('div').outerHeight() + 150;

        if ($previousDivHeight > 430)
            $previousDivHeight = 430;

        $previousDivHeight = $parentWidth;

        $(this).width($previousDivHeight);
        $(this).height($previousDivHeight);
        $image.show();
    });

    $('.lightbox-card').each(function () {
        let $image = $(this).find('img');
        let $imageWidth = $image.width();
        $image.width($imageWidth);
        $image.height($imageWidth);
        $image.show();
    });
}

const setMultiItemCarousel = () => {
    let owlCarousels = $('.owl-carousel');
    
    if (owlCarousels.length > 0) {
        owlCarousels.each(function () {
            let carousel = $(this);
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
                        items: 3,
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

const doPost = (options) => {
    const {
        url,
        formData,
        callback,
        skipLoader = false
    } = options;

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
            //window.location.href = "/error";
        }
    });
}

let postAjax = function (url, formData, callback, skipLoader = false) {
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
            //window.location.href = "/error";
        }
    });
}

const setParallaxMarquee = () => {
    if ($('.marquee').length > 0) {
        let banners = [
            { element: $('.marquee:nth-of-type(1)'), direction: 'left', factor: 7 },
            { element: $('.marquee:nth-of-type(2)'), direction: 'right', factor: 8 },
            { element: $('.marquee:nth-of-type(3)'), direction: 'left', factor: 9 }
        ];

        banners.forEach(function (banner) {
            let bannerOffset = banner.element.offset().left;
            let bannerWidth = banner.element.width();
            let lastScrollPosition = -1;

            if (banner.direction === 'left') {
                banner.element.css({
                    'transform': 'translateX(' + (-bannerWidth) + 'px)'
                });
            } else {
                banner.element.css({
                    'transform': 'translateX(' + ($(window).width() - bannerOffset) + 'px)'
                });
            }

            $(window).scroll(function () {
                let scroll = $(window).scrollTop();
                let delta = lastScrollPosition === -1 ? 0 : scroll - lastScrollPosition;
                lastScrollPosition = scroll;
                requestAnimationFrame(function () {
                    if (banner.direction === 'left') {
                        let bannerPosition = -scroll / banner.factor - bannerOffset;
                        if (bannerPosition < -bannerWidth) {
                            bannerPosition += bannerWidth;
                        }
                        banner.element.css({
                            'transform': 'translateX(' + bannerPosition + 'px)'
                        });
                    } else {
                        let bannerPosition = scroll / banner.factor + ($(window).width() - bannerOffset);
                        if (bannerPosition > $(window).width()) {
                            bannerPosition -= bannerWidth;
                        }
                        banner.element.css({
                            'transform': 'translateX(' + bannerPosition + 'px)'
                        });
                    }
                });
            });
        });
    }
}

const cookieName = 'fuse-hostels-and-travel';
const getCookie = () => {
    let cookie = Cookie.get(cookieName);

    if (cookie === undefined) {
        window.setTimeout(function () {
            let cookiesModalEl = document.getElementById('cookiesModal');
            let modal = new mdb.Modal(cookiesModalEl);
            modal.show();
        }, 2000);
    }
}
const confirmCookie = () => {
    Cookie.set(cookieName, new Date(), {expires: 365});
    $('#cookiesModal').modal('hide');
}