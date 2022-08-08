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
            { "data": "remark" }, //8
            { "data": "id" }, //9
            { "data": "id" }, //10
            { "data": "proposalId" }, //11
            //{ "data": "remark" }, //6
        ],
       columnDefs: [
           {
               "targets": [11],
               "className": "hide_column"
           },
            {
                targets: 9,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<button id="approveBtn" class="btn btn-primary">Approve</button>`
                }
            },
            {
                targets: 10,
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
        var type = data.proposal.type;
        $.ajax({
            type: "POST",
            url: "Approvals/ApproveProposal",
            data: { "proposalId": proposalId, "approvalId": approvalId, "type" : type },
            success: function (data) {
                var ul = document.getElementById('error-messages');
                ul.innerHTML = '';
                approvalReallocateTable.draw();
            }
        })
    });




    var approvalReallocateTable = $('#dataTableApprovalReallocate').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Approvals/GetApprovalReallocatePagination",
            type: "POST"
        },
        "columns": [
            { "data": "submitDate" },  //0
            { "data": "level1" }, //1
            { "data": "level2" }, //2
            { "data": "bannerName" }, //3
            { "data": "proposal.plantName" }, //4
            { "data": "proposal.plantCode" }, //5
            { "data": "pcMap" },       //6
            { "data": "descriptionMap" }, //7
            { "data": "proposal.reallocate" },      //8
            { "data": "proposal.bannerTargetId" }, //9
            { "data": "remark" }, //10
            { "data": "id" }, //11
            { "data": "id" }, //12
            { "data": "proposalId" }, //13
        ],
        columnDefs: [
            {
                targets: 9,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return full.proposal.bannerNameTarget + " (" + full.proposal.plantNameTarget + ")" + " (" + full.proposal.plantCodeTarget + ")";
                }
            },
            {
                targets: 11,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<button id="approveReallocateBtn" class="btn btn-primary">Approve</button>`
                }
            },
            {
                targets: 12,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<button class="btn btn-danger">Reject</button>`
                }
            },
            {
                targets: 13,
                className: 'hide_column'
            }
        ]
    });

    $("#dataTableApprovalReallocate tbody").on('click', '#approveReallocateBtn', function () {
        var data = approvalReallocateTable.row($(this).parents('tr')).data();
        var proposalId = data.proposalId;
        var approvalId = data.id;
        var type = data.proposal.type;
        $.ajax({
            type: "POST",
            url: "Approvals/ApproveProposal",
            data: { "proposalId": proposalId, "approvalId": approvalId, "type": type },
            success: function (data) {
                var ul = document.getElementById('error-messages');
                ul.innerHTML = '';
            }
        })
    });
}); 


