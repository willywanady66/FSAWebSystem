$(document).ready(function () {

    //function approve(id) {
    //    console.log();
    //}

   var approvalTable = $('#dataTableApproval').DataTable({
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
            { "data": "proposalId" }, //10
            //{ "data": "remark" }, //6
        ],
       columnDefs: [
           {
               "targets": [10],
               "className": "hide_column"
           },
            {
                targets: 8,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<button id="approveBtn" class="btn btn-primary">Approve</button>`
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


    $("#dataTableApproval tbody").on('click', '#approveBtn', function () {
        var data = approvalTable.row($(this).parents('tr')).data();
        var proposalId = data.proposalId;
        var approvalId = data.id;
        $.ajax({
            type: "POST",
            url: "Approvals/ApproveProposal",
            data: { "proposalId": proposalId, "approvalId": approvalId },
            success: function (data) {
                var ul = document.getElementById('error-messages');
                ul.innerHTML = '';
            }
        })
    })
}); 


