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
            { "data": "email" },       //1
            { "data": "role" }, //2
           
            { "data": "bannerName" },      //3
            { "data": "status" }, //4
            { "data": "userId"}, //5
            { "data": "id" }, //6
            { "data": "userId" }, //7
        ],
        columnDefs: [
            {
                "targets": [6,7],
                "className": "hide_column"
            },
            {
                targets: 5,
                orderable : false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="/UserUnilevers/Edit/${full.userId}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                }

                    
            }
        ]
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
});