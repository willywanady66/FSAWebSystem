$(document).ready(function () {



   var approvalTable = $('#dataTableApproval').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: approvalPaginationUrl,
            type: "POST"
        },
        "columns": [
            { "data": "id" },  //0
            { "data": "proposalSubmitDate" },  //1
            { "data": "bannerName" }, //2   
            { "data": "pcMap" },       //3
            { "data": "descriptionMap" }, //4
            { "data": "proposeAdditional" },      //5
            { "data": "rephase" }, //6
            { "data": "remark" }, //7
            {"data" : "approvedBy"},//8
            { "data": "id" }, //9 for edit
            { "data": "id" }, //10 
           
       ],
       "order": [[1, 'asc']],
       columnDefs: [
           {
               "targets": 0,
               "orderable": false,
               className: 'text-center',
               "render": function (data, type, full, meta) {
                   return `<input type="checkbox" class="" id="checkbox-${full.id}" />`
               }
           },
           {
               "targets": [10],
               "className": "hide_column"
           },
            {
                targets: 9,
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
        Swal.fire({
            title: 'Loading...',
            html: 'Approving...',
            timerProgressBar: true,
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
            },
        });
        var approvalId = $('.approvalId').attr('value');
        var approvalNote = $('#approvalNote').val();
        $.ajax({
            type: "POST",
            url: approveUrl,
            data: { "approvalId": approvalId, "approvalNote": approvalNote },
            success: function (data) {
                Swal.close();
                setTimeout(
                    function () {
                        window.location.href = indexUrl;
                    }, 800)
            }
        })
    });


    $("#rejectProposalBtn").click(function () {
        Swal.fire({
            title: 'Loading...',
            html: 'Rejecting...',
            timerProgressBar: true,
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
            },
        });
        var approvalId = $('.approvalId').val();
        var approvalNote = $('#approvalNoteReject').val();
        $.ajax({
            type: "POST",
            url: rejectUrl,
            data: { "approvalId": approvalId, "approvalNote": approvalNote },
            success: function (data) {
                Swal.close();
                var ul = document.getElementById('error-messages');
                ul.innerHTML = '';

                if (data.value) {
                    if (data.value.errorMessages) {
                        var errorMessages = data.value.errorMessages;
                        var ul = document.getElementById('error-messages');
                        ul.innerHTML = '';
                        for (var i = 0; i < errorMessages.length; i++) {
                            var li = document.createElement('li');
                            li.appendChild(document.createTextNode(errorMessages[i]));
                            ul.appendChild(li);
                        }

                        document.body.scrollTop = 0;
                        document.documentElement.scrollTop = 0;
                    }
                }
                else {
                    setTimeout(
                        function () {
                            window.location.href = indexUrl;
                        }, 1000)
                }
            }
        })
    });

    $("#approveProposalsBtn").click(function () {
        Swal.fire({
            title: 'Loading...',
            html: 'Approving...',
            timerProgressBar: true,
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
            },
        });
        var selectedIds = getSelectedProposal();
        var approvalNote = $('#approvalNote').val();
        $.ajax({
            type: "POST",
            url: approveProposalsUrl,
            data: { "approvalIds": selectedIds, "approvalNote": approvalNote },
            success: function (data) {
                Swal.close();
                setTimeout(
                    function () {
                        window.location.href = indexUrl;
                    }, 800)
            },
            error: function () {
                Swal.close();
            }
        })
    });

    $('#requestNextTypeBtn').click(function () {
        Swal.fire({
            title: 'Loading...',
            html: 'Approving...',
            timerProgressBar: true,
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
            },
        });
        var approvalId = $('.approvalId').val();
        var approvalNote = $('#requestNote').val();
        $.ajax({
            type: "POST",
            url: reqNextTypeUrl,
            data: { "approvalId": approvalId, "approvalNote": approvalNote },
            success: function (data) {
                Swal.close();
                var ul = document.getElementById('error-messages');
                ul.innerHTML = '';

                if (data.value) {
                    if (data.value.errorMessages) {
                        var errorMessages = data.value.errorMessages;
                        var ul = document.getElementById('error-messages');
                        ul.innerHTML = '';
                        for (var i = 0; i < errorMessages.length; i++) {
                            var li = document.createElement('li');
                            li.appendChild(document.createTextNode(errorMessages[i]));
                            ul.appendChild(li);
                        }

                        document.body.scrollTop = 0;
                        document.documentElement.scrollTop = 0;
                    }
                }
                else {
                    setTimeout(
                        function () {
                            window.location.href = indexUrl;
                        }, 1000)
                }
            }
        })
    });



    $("#rejectProposalsBtn").click(function () {
        Swal.fire({
            title: 'Loading...',
            html: 'Rejecting...',
            timerProgressBar: true,
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
            },
        });
        var selectedIds = getSelectedProposal();
        var approvalNote = $('#approvalNoteReject').val();
        $.ajax({
            type: "POST",
            url: rejectProposalsUrl,
            data: { "approvalIds": selectedIds, "approvalNote": approvalNote },
            success: function (data) {
                Swal.close();
                setTimeout(
                    function () {
                        window.location.href = indexUrl;
                    }, 500)
            },
            error: function () {
                Swal.close();
            }
        })
    });

    $('#checkAll').click(function () {
        if ($(this).is(':checked')) {
            $('#dataTableApproval TBODY TR').each(function () {
                var row = $(this);
                row.find("TD").eq(0).find("INPUT").prop("checked", true);
            });
        }
        else {
            $('#dataTableApproval TBODY TR').each(function () {
                var row = $(this);
                row.find("TD").eq(0).find("INPUT").prop("checked", false);
            });       
        }
    });

    $('#dataTableApproval').on('change', 'input[type="checkbox"]', function () {
        if ($('#dataTableApproval TBODY INPUT:checkbox:checked').length > 0) {
            $('#openConfirmApproveModalBtn').prop('disabled', false);
            $('#openConfirmRejectModalBtn').prop('disabled', false);
        }
        else {
            $('#openConfirmApproveModalBtn').prop('disabled', true);
            $('#openConfirmRejectModalBtn').prop('disabled', true);
        }
    });

    function getSelectedProposal() {
        var ids = [];
        $('#dataTableApproval TBODY TR').each(function () {
            var row = $(this);
            var selected = row.find("TD").eq(0).find("INPUT").is(':checked')
            if (selected) {
                var id = row.find("TD").eq(10).html();
                ids.push(id);
            }
        });
        return ids;
    }
}); 


