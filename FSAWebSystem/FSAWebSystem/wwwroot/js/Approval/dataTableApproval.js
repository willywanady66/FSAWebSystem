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
            { "data": "bannerName" }, //1
            { "data": "pcMap" },       //2
            { "data": "descriptionMap" }, //3
            { "data": "proposeAdditional" },      //4
            { "data": "rephase" }, //5
            //{ "data": "remark" }, //6
        ]
    });
}); 


