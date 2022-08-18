$(document).ready(function () {

    var userTable = $('#dataTableUsers').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getUsersUrl,
            type: "POST"
        },
        "columns": [
            { "data": "name" },  //0
            { "data": "wlName" }, //1
            { "data": "email" },       //2
            { "data": "role" }, //3
            { "data": "bannerName" },      //4
            { "data": "status" }, //5
            { "data": "userId" }, //6
            { "data": "id" }, //7
            { "data": "userId" }, //8
        ],
        columnDefs: [
            {
                "targets": [7, 8],
                "className": "hide_column"
            },
            {
                targets: 6,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {

                    return `<a href="${editUserUrl}/${full.userId}">
                                <i class="fas fa-pen"></i>
                            <a/>`

                    return null;

                }
            }
        ],
        "rowCallback": function (row, data, index) {
            if (data.status == "Active") {
                $('td:eq(5)', row).css({ color: "green" });
            }
            else {
                $('td:eq(5)', row).css({ color: "red" });
            }
        }
    });


    $('#dataTableBanners').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Admin/GetBannerPagination",
            type: "POST"
        },
        "columns": [
            { "data": "trade" },  //0
            { "data": "cdm" },  //1
            { "data": "kam" },  //2
            { "data": "bannerName" },       //3
            { "data": "plantName" }, //4
            { "data": "plantCode" },      //5
            { "data": "id" } //6
        ],
        columnDefs: [
            {
                targets: 6,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="./Banners/Edit/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                }
            }
        ]
    });

    $('#dataTableProductCategory').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Admin/GetCategoryPagination",
            type: "POST"
        },
        "columns": [
            { "data": "id" },  //0
            { "data": "categoryProduct" }       //1
        ],
        columnDefs: [
            {
                targets: 0,
                searchable: false,
                orderable: false,
                "render": function (data, type, full, meta) {
                    return meta.row + 1 + meta.settings._iDisplayStart;
                }
            }
        ]
    });

    $('#dataTableSKUs').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Admin/GetSKUPagination",
            type: "POST"
        },
        "columns": [
            { "data": "pcMap" },  //0
            { "data": "descriptionMap" },       //1
            { "data": "category" },  //2
            { "data": "status" }, //3
            { "data": "id" } //4
        ],
        columnDefs: [{
            targets: 4,
            orderable: false,
            className: 'text-center',
            "render": function (data, type, full, meta) {
                return `<a href="{editSKUUrl}/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
            }
        }],
        "rowCallback": function (row, data, index) {
            if (data.status == "Active") {
                $('td:eq(3)', row).css({ color: "green" });
            }
            else {
                $('td:eq(3)', row).css({ color: "red" });
            }
        }
    });


    $('#dataTableWorkLevels').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getWLUrl,
            type: "POST"
        },
        "columns": [
            { "data": "id" }, //0
            { "data": "wl" },  //1
            { "data": "status" },  //2
            { "data": "id" }  //3

        ],
        columnDefs: [
            {
                targets: 0,
                searchable: false,
                orderable: false,
                "render": function (data, type, full, meta) {
                    return meta.row + 1 + meta.settings._iDisplayStart;
                }
            },
            {
                targets: 3,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="./WorkLevels/Edit/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                }
            }
        ],
        "rowCallback": function (row, data, index) {
            if (data.status == "Active") {
                $('td:eq(2)', row).css({ color: "green" });
            }
            else {
                $('td:eq(2)', row).css({ color: "red" });
            }
        }
    });

    var month = $('#dropDownMonth option:selected').val();
    var year = $('#dropDownYear option:selected').val();


    var dtCalendar = $('#dataTableCalendar').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Admin/GetFSACalendar",
            type: "GET",
            data: function (d) {
                return $.extend(d, { "month": month, "year": year });
            }
        },
        "columns": [
            { "data": "week" },
            { "data": "startDate" },
            { "data": "endDate" },
            { "data": "year" },
            { "data": "month" },
            { "data": "id" }
        ],
        columnDefs: [
            {
                targets: 5,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="${editFSACalUrl}/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                }
            },
            {
                targets: 'all',
                defaultContent: ""
            }
        ],
        rowsGroup: [5],
        "rowCallback": function (row, data, index) {
            $('td:eq(5)', row).addClass('align-middle');
        },
        "paging": false,
        "info": false,
        searching: false
    });


    $('#dropDownMonth').change(function () {
        month = $('#dropDownMonth option:selected').val();
        dtCalendar.draw();
    });


    $('#dropDownYear').change(function () {
        year = $('#dropDownYear option:selected').val();
        dtCalendar.draw();
    });





    var doc = $('#documentType option:selected').val();
    if (doc !== "Monthly Bucked") {
        $('#uploadMonthGroup').hide();

    }

    $('#documentType').change(function () {
        doc = $('#documentType option:selected').text();
        if (doc !== "Monthly Bucket") {
            $('#uploadMonthGroup').hide();
        }
        else {
            $('#uploadMonthGroup').show();
        }
    });


    var monthUli = $('#dropDownMonthULI option:selected').val();
    var yearUli = $('#dropDownYearULI option:selected').val();
    $('#dropDownMonthULI').change(function () {
        monthUli = $('#dropDownMonthULI option:selected').val();
        dtULICalendar.draw();
    });


    $('#dropDownYearULI').change(function () {
        yearUli = $('#dropDownYearULI option:selected').val();
        dtULICalendar.draw();
    });
    var dtULICalendar = $('#dataTableUliCalendar').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getULICal,
            type: "GET",
            data: function (d) {
                return $.extend(d, { "month": monthUli, "year": yearUli });
            }
        },
        "columns": [
            { "data": "week" },
            { "data": "startDate" },
            { "data": "endDate" },
            { "data": "year" },
            { "data": "month" },
            { "data": "id" }
        ],
        columnDefs: [
            {
                targets: 5,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="${editULICalUrl}/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                }
            },
            {
                targets: 'all',
                defaultContent: ""
            }
        ],
        rowsGroup: [5],
        "rowCallback": function (row, data, index) {
            $('td:eq(5)', row).addClass('align-middle');
        },
        "paging": false,
        "info": false,
        searching: false
    });
});