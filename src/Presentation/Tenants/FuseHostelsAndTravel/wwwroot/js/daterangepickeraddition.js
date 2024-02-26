$(function() {
    $('.check-in-out-date').daterangepicker({
        "minDate": new Date(),
        "opens": "center",
        "drops": "up",
        "format":"dd/MM/yyyy",
        // "cancelClass": "btn-outline-primary",
        // "applyClass": "btn-primary",
        "autoApply": true,
        "autoUpdateInput":true,
        "parentEl": ".daterangepickeroverlay"
    }, function(start, end, label) {
        console.log("A new date selection was made: " + start.format('YYYY-MM-DD') + ' to ' + end.format('YYYY-MM-DD'));

        let checkInDate = start.format("DD/MM/YYYY");
        let checkOutDate =end.format("DD/MM/YYYY");
        $('.check-in-out-date').val(checkInDate + ' - ' +checkOutDate).addClass('active');
        $('.check-in-out-date-start').val(checkInDate);
        $('.check-in-out-date-end').val(checkOutDate);
    });
});