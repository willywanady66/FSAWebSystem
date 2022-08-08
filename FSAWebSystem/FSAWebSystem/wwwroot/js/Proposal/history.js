$(document).ready(function () {
    var month = $('#dropDownMonth option:selected').val();
    var year = $('#dropDownYear option:selected').val();


    var tableHistory = $("#dataTableProposalHistory").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Proposals/GetProposalHistoryPagination",
            type: "POST",
            data: function (d) {
                return $.extend(d, { "month": month, "year": year, "type":99 });
            }
        },
        columns: [
            { "data": "submittedAt" }, //0
            { "data": "week" }, //1
            { "data": "bannerName" }, //2
            { "data": "plantName" },  //3
            { "data": "pcMap" },       //4
            { "data": "descriptionMap" }, //5
            { "data": "rephase" },      //6
            { "data": "approvedRephase" },   //7
            { "data": "proposeAdditional" },   //8
            { "data": "approvedProposeAdditional" },   //9
            { "data": "reallocate" },   //10
            { "data": "remark" },   //11
            { "data": "status" },   //12
            { "data": "approvedBy" },   //13
            { "data": "rejectionReason" },   //14
        ],
        "rowCallback": function (row, data, index) {
            if (data.status == "Approved") {
                $('td:eq(12)', row).css({ color: "green" });
            }
            else if (data.status == "Rejected") {
                $('td:eq(12)', row).css({ color: "red" });
            }
        }
    });


    var tableMonthlyBucketHistory = $('#dataTableMonthlyBucketHistory').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Proposals/GetMonthlyBucketHistoryPagination",
            type: "POST",
            data: function (d) {
                return $.extend(d, {
                    "month": month, "year": year
                });
            }
        },
        columns: [
            { "data": "uploadedDate" }, //0
            { "data": "year" }, //1
            { "data": "month" }, //2
            { "data": "bannerName" },//3
            { "data": "plantName" }, //4
            { "data": "pcMap" }, //5
            { "data": "descriptionMap" }, //6
            { "data": "price" }, //7
            { "data": "plantContribution" }, //8
            { "data": "ratingRate" }, //9
            { "data": "tct" }, //10
            { "data": "monthlyTarget" }, //11
        ],
        columnDefs: [
            {
                targets: 7,
                render: $.fn.dataTable.render.number(',', '.', 0, '')
            },
            {
                targets: [8, 10, 11],
                render: function (data, type, row) {
                    return data + ' %';
                }
            }
        ],

    });


    var tableWeeklyBucketHistory = $('#dataTableWeeklyBucketHistory').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Proposals/GetWeeklyBucketHistoryPagination",
            type: "POST",
            data: function (d) {
                return $.extend(d, {
                    "month": month, "year": year
                });
            }
        },
        columns: [
            { "data": "uploadedDate" },
            { "data": "year" },
            { "data": "month" },
            { "data": "week" },
            { "data": "bannerName" },
            { "data": "plantName" },
            { "data": "pcMap" },
            { "data": "descriptionMap" },
            { "data": "dispatchConsume" },
        ]
    });

});