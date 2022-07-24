$(document).ready(function () {

    var userTable = $('#dataTableUsers').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Admin/GetUserPagination",
            type: "POST"
        },
        "columns": [
            { "data": "name" },  //0
            { "data": "wlName"}, //1
            { "data": "email" },       //2
            { "data": "role" }, //3
            { "data": "bannerName" },      //4
            { "data": "status" }, //5
            { "data": "userId"}, //6
            { "data": "id" }, //7
            { "data": "userId" }, //8
        ],
        columnDefs: [
            {
                "targets": [7,8],
                "className": "hide_column"
            },
            {
                targets: 6,
                orderable : false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="/UserUnilevers/Edit/${full.userId}">
                                <i class="fas fa-pen"></i>
                            <a/>`
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
            { "data": "bannerName" },       //1
            { "data": "plantName" }, //2
            { "data": "plantCode" },      //3
            { "data": "id" } //4
        ],
        columnDefs: [
            {
                targets: 4,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="/Banners/Edit/${full.id}">
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
                searchable : false,
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
            { "data": "category" } //2
        ]
    });


    $('#dataTableWorkLevels').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Admin/GetWorkLevelPagination",
            type: "POST"
        },
        "columns": [
            {"data" : "id"}, //0
            { "data": "wl" }  //1
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
});