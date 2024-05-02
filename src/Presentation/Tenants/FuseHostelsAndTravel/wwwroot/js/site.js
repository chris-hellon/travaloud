const bindFuseJs = ()=> {
    adjustCardMargain();
    adjustCircularImages();

    let inheritStickyParent = $('.inherit-sticky-parent');
    if (inheritStickyParent.length > 0) {
        inheritStickyParent.each(function () {
            let stickyHeight = $('.sticky-top[data-mdb-sticky-boundary]').outerHeight();
            $(this).css('margin-top', '-' + stickyHeight + 'px');
            $(this).find('.offset-background').css('background-image', '-webkit-linear-gradient(-50deg, #FFFFFF 70%, #D1AC00 50%)');
        });
    }
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

initializeTravaloud({
    cookieName : 'fuse-hostels-and-travel',
    onResizeCallbacks : [bindFuseJs],
    onLoadCallbacks : [bindFuseJs, setParallaxMarquee]
});