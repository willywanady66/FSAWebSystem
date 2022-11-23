$(document).ready(function () {
    var monthDailyReport = $('#dropDownMonthDaily option:selected').val();
    var yearDailyReport = $('#dropDownYearDaily option:selected').val();

    var monthWeeklyReport = $('#dropDownMonthWeekly option:selected').val();
    var yearWeeklyReport = $('#dropDownYearWeekly option:selected').val();

    //var monthMonthlyReport = 
    $('#dataTableDailyReport').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getDailyReports,
            type: "POST",
            data: function (d) {
                return $.extend(d, { "month": monthDailyReport, "year": yearDailyReport });
            },
        },
        "columns": [
            { "data": "name" },
            { "data": "publishedDate" },
            { "data": "status" },
            { "data": "id" },
        ],
        "columnDefs": [
            {
                "targets": "_all", // your case first column
                "className": "text-center"
            },
            {
                "targets": 1,
                "render": function (data) {
                    var date = moment(data).format("DD-MMM-yyyy HH:mm");
                    return date;
                },
            },
            {
                "targets": 3,
                "render": function (data) {
                    return `<a href="">
                                <i class="fas fa-download"></i>
                            <a/>`;
                },
            }
        ],
    });


    $('#dataTableWeeklyReport').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getWeeklyReports,
            type: "POST",
            data: function (d) {
                return $.extend(d, { "month": monthWeeklyReport, "year": yearWeeklyReport });
            },
        },
        "columns": [
            { "data": "name" },
            { "data": "publishedDate" },
            { "data": "status" },
            { "data": "id" },
        ],
        "columnDefs": [
            {
                "targets": "_all", // your case first column
                "className": "text-center"
            },
            {
                "targets": 1,
                "render": function (data) {
                    var date = moment(data).format("DD-MMM-yyyy HH:mm");
                    return date;
                },
            },
            {
                "targets": 3,
                "render": function (data) {
                    return `<a href="">
                                <i class="fas fa-download"></i>
                            <a/>`;
                },
            }
        ],
    });
});