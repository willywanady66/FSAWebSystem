$(document).ready(function () {
    //var tableApproval = $("#dataTableApproval").DataTable({
    //    "processing": true,
    //    "serverSide": true,
    //    "ajax": {
    //        url: "Approvals/GetApprovalPagination",
    //        type: "POST"
    //    },
    //    "columns": [
    //        { "data": "submitDate" },  //0
    //        { "data": "bannerName" }, //1
    //        { "data": "pcMap" },       //2
    //        { "data": "descriptionMap" }, //3
    //        { "data": "proposeAdditional" },      //4
    //        //{ "data": "rephase" }, //5
    //        //{ "data": "remark" }, //6
    //    ]
    //});


    $('#dataTableApproval').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Approvals/GetApprovalPagination",
            type: "POST"
        },
        "columns": [
            { "data": "submitDate" },  //0
            { "data": "level1" }, //1
            { "data": "level2" }, //2
            { "data": "bannerName" }, //3
            { "data": "pcMap" },       //4
            { "data": "descriptionMap" }, //5
            { "data": "proposeAdditional" },      //6
            { "data": "rephase" }, //7
            { "data": "id" }, //8
            { "data": "id" }, //9
            //{ "data": "remark" }, //6
        ],
        columnDefs: [
            {
                targets: 8,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<button class="btn btn-primary">Approve</button>`
                }
            },
            {
                targets: 9,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<button class="btn btn-danger">Reject</button>`
                }
            }
        ],
    });
}); 


