$(window).on('load', function() {
    let selectedStartDate = $('.check-in-out-date-start').val();
    let selectedEndDate = $('.check-in-out-date-end').val();

    if (selectedStartDate !== undefined && selectedStartDate.length && selectedEndDate !== undefined && selectedEndDate.length)
    {
        $('.check-in-out-date').daterangepicker({
            "minDate": new Date(),
            "opens": "center",
            "drops": "up",
            "format":"dd/MM/yyyy",
            "startDate": moment(selectedStartDate, "DD/MM/YYYY"),
            "endDate": moment(selectedEndDate, "DD/MM/YYYY"),
            "autoApply": true,
            "autoUpdateInput":true,
            "parentEl": ".daterangepickeroverlay"
        }, function(start, end, label) {
            let checkInDate = start.format("DD/MM/YYYY");
            let checkOutDate = end.format("DD/MM/YYYY");
            $('.check-in-out-date').val(checkInDate + ' - ' +checkOutDate).addClass('active');
            $('.check-in-out-date-start').val(checkInDate);
            $('.check-in-out-date-end').val(checkOutDate);
        });
    }
    else {
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
            let checkInDate = start.format("DD/MM/YYYY");
            let checkOutDate =end.format("DD/MM/YYYY");
            $('.check-in-out-date').val(checkInDate + ' - ' +checkOutDate).addClass('active');
            $('.check-in-out-date-start').val(checkInDate);
            $('.check-in-out-date-end').val(checkOutDate);
        });
    }
});