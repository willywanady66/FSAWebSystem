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
            {"data" : "isActive"}
            { "data": "bannerName", searchable: false }      //3
            { "data": "id" }, //4
            { "data": "userId" }, //5
        ], "columnDefs": [
            {
                "targets": [4, 5],
                "className": "hide_column"
            }
            ]
    });


    $('#dataTableBanners').DataTable();
    $('#dataTableProductCategory').DataTable();
    $('#dataTableSKUs').DataTable();
});