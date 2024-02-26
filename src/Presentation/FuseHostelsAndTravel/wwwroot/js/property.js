$(document).ready(function() {
    // Add a click event handler to elements with class 'nav-link' and 'href' containing '#'
    $('.nav-link[href*="#"]').click(function (e) {

        e.preventDefault(); // Prevent the default behavior of the link

        // Get the target element's ID from the 'href' attribute
        var targetId = $(this).attr('href').substring(1);

        // Calculate the target scroll position
        var $targetElement = $('#' + targetId);
        var offset = $('.navbar').outerHeight(); // Adjust this value to your desired offset
        var targetScrollPosition = $targetElement.offset().top - offset;

        // Animate the scroll to the target position
        $('html, body').animate({
            scrollTop: targetScrollPosition
        }, 300); // You can adjust the duration (in milliseconds) as needed
    });
});