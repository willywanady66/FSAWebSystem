$(document).ready(function () {

    //function approve(id) {
    //    console.log();
    //}

   var approvalTable = $('#dataTableApproval').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: approvalPaginationUrl,
            type: "POST"
        },
        "columns": [
            { "data": "proposalSubmitDate" },  //0
            { "data": "bannerName" }, //1
            { "data": "pcMap" },       //2
            { "data": "descriptionMap" }, //3
            { "data": "proposeAdditional" },      //4
            { "data": "rephase" }, //5
            { "data": "remark" }, //6
            { "data": "id" }, //7
           
        ],
       columnDefs: [
           {
               "targets": [6],
               "className": "hide_column"
           },
            {
                targets: 7,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    
                    return `<a href="${approvalDetailUrl}/${full.id}">
                                <i class="fas fa-eye"></i>
                            <a/>`;
                }
            }
       ],
       error: {

       }
    });


    var approvalDetailTable = $('#dataTableApprovalDetails').DataTable({
        searching: false,
        lengthChange: false,
        ordering: false,
        info: false,
        columnDefs: [
            {
                "targets": [10],
                "render": function (data) {
                    if (data < 0) {
                        var val = '(' + data.toString().substring(1) + ')';
                        data = '<p style="color:red">' + val + '</p>';
                    }
                    else {
                        data = '<p>' + data + '</p>' 
                    }
                    return data;
                }
            },
            {
                "targets": [5, 6, 7, 8, 9],
                "render": function (data) {
                    if (data < 0) {
                        data = '(' + data.toString().substring(1) + ')';
                    }
                    return data;
                }
            },
        ]
    });

    $("#approveProposalBtn").click(function () {
        
        var approvalId = $('.approvalId').attr('value');
        var approvalNote = $('#approvalNote').val();
        $.ajax({
            type: "POST",
            url: approveUrl,
            data: { "approvalId": approvalId, "approvalNote": approvalNote },
            success: function (data) {
                setTimeout(
                    function () {
                        window.location.href = indexUrl;
                    }, 1500)
            }
        })
    });


    $("#rejectProposalBtn").click(function () {

        var approvalId = $('.approvalId').val();
        var approvalNote = $('#approvalNote').val();
        $.ajax({
            type: "POST",
            url: rejectUrl,
            data: { "approvalId": approvalId, "approvalNote": approvalNote },
            success: function (data) {
                setTimeout(
                function() {
                        window.location.href = indexUrl;
                }, 1500)
                
            }
        })
    });

    //var approvalReallocateTable = $('#dataTableApprovalReallocate').DataTable({
    //    "processing": true,
    //    "serverSide": true,
    //    "ajax": {
    //        url: "Approvals/GetApprovalReallocatePagination",
    //        type: "POST"
    //    },
    //    "columns": [
    //        { "data": "submitDate" },  //0
    //        { "data": "level1" }, //1
    //        { "data": "level2" }, //2
    //        { "data": "bannerName" }, //3
    //        { "data": "pcMap" },       //4
    //        { "data": "descriptionMap" }, //5
    //        { "data": "proposal.reallocate" },      //6
    //        { "data": "proposal.bannerTargetId" }, //7
    //        { "data": "remark" }, //8
    //        { "data": "id" }, //9
    //        { "data": "id" }, //10
    //        { "data": "proposalId" }, //11
    //        //{ "data": "remark" }, //6
    //    ],
    //    columnDefs: [
    //        {
    //            targets: 7,
    //            orderable: false,
    //            className: 'text-center',
    //            "render": function (data, type, full, meta) {
    //                return full.proposal.bannerNameTarget + " (" + full.proposal.plantNameTarget + ")" + " (" + full.proposal.plantCodeTarget + ")";
    //            }
    //        },
    //        {
    //            targets: 9,
    //            orderable: false,
    //            className: 'text-center',
    //            "render": function (data, type, full, meta) {
    //                return `<button id="approveReallocateBtn" class="btn btn-primary">Approve</button>`
    //            }
    //        },
    //        {
    //            targets: 10,
    //            orderable: false,
    //            className: 'text-center',
    //            "render": function (data, type, full, meta) {
    //                return `<button class="btn btn-danger">Reject</button>`
    //            }
    //        },
    //        {
    //            targets: 11,
    //            className: 'hide_column'
    //        }
    //    ]
    //});

    //$("#dataTableApprovalReallocate tbody").on('click', '#approveReallocateBtn', function () {
    //    var data = approvalReallocateTable.row($(this).parents('tr')).data();
    //    var proposalId = data.proposalId;
    //    var approvalId = data.id;
    //    var type = data.proposal.type;
    //    $.ajax({
    //        type: "POST",
    //        url: "Approvals/ApproveProposal",
    //        data: { "proposalId": proposalId, "approvalId": approvalId, "type": type },
    //        success: function (data) {
    //            var ul = document.getElementById('error-messages');
    //            ul.innerHTML = '';
    //        }
    //    })
    //});
}); 


