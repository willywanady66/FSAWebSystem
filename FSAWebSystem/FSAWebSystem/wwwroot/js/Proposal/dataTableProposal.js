$(document).ready(function () {
    let proposalInputs = new Array();
    let remarks = ["", "Big Promotion Period", "Grand Opening", "Additional Store", "Rephase", "Spike Order", "No Baseline Last Year"];
    //var proposals = getUserInput(proposalInputs);
    var tableProposals = $("#dataTableProposal").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Proposals/GetProposalPagination",
            type: "POST",
            data: function (d) {
                return $.extend(d, { "proposalInputs": getUserInput(proposalInputs) });
            }
        },
        "columns": [
            { "data": "bannerName" }, //0
            { "data": "plantName" },  //1
            { "data": "pcMap" },       //2
            { "data": "descriptionMap" }, //3
            { "data": "ratingRate" },      //4 
            { "data": "monthlyBucket" },   //5
            { "data": "currentBucket" },   //6
            { "data": "nextBucket" },      //7
            { "data": "validBJ" },         //8
            { "data": "remFSA" },         //9
            { "data": "rephase" },         //10
            { "data": "proposeAddional" }, //11
            { "data": "remark" },           //12
            { "data": "weeklyBucketId" } ,  //13
            {"data": "id"}, //14
            {"data": "approvalId"} //15
        ],
        "order": [[0, 'asc']],
        "drawCallback": function (data) {
            proposals = data.ajax.proposalInputs;
        },
        "columnDefs": [
            {
                "targets": [13,14, 15],
                "className": "hide_column"
            },
            {
                "targets": [6, 7],
                "render": function (data) {
                    if (data < 0) {
                        data = '(' + data.toString().substring(1) + ')';
                    }
                    return data;
                }
            },
            {
                "targets": 10,
                "render": function (data, type, full, meta) {
                    let input = "";
                    if (full.week != 1) {
                        input = `<input type="number" class="form-control" id="row-${meta.row}-rephase" name="row-${meta.row}-rephase" min=0 value=${full.rephase} />`;
                    }
                    else {
                        input = `<input type="number" class="form-control" id="row-${meta.row}-rephase" name="row-${meta.row}-rephase" disabled min=0 value=${full.rephase} />`
                    }
                    return input;
                },
                "orderable": false
            },
            {
                "targets": 11,
                "render": function (data, type, full, meta) {
                    return `<input type="number" class="form-control" id="row-${meta.row}-additional" name="row-${meta.row}-additional" min=0 value=${full.proposeAdditional} />`;
                },
                "orderable": false
            },
            {
                "targets": 12,
                "render": function (data, type, full, meta) {
                    var options = "";

                    remarks.forEach(function (item) {
                        var remark = item;
                        if (remark == full.remark) {
                            options += `<option selected>${remark}</option>`
                        }
                        else {
                            options += `<option>${remark}</option>`
                        }
                    })

                    var select = `<select class="form-control" id="row-${meta.row}-remark" name="row-${meta.row}-remark">${options}</select>`;
                    return select;
                },
                "orderable": false
            }
        ],
        "rowCallback": function (row, data, index) {
            if (data.nextBucket < 0) {
                $('td:eq(7)', row).css({ color: "red" });
            }
            if (data.currentBucket < 0) {
                $('td:eq(6)', row).css({ color: "red" });
            }

            if (data.remFSA < 0) {
                $('td:eq(9)', row).css({ color: "red" });
            }
        }

    });

    function getUserInput(proposalInputs) {
        $("#dataTableProposal TBODY TR").each(function () {
            var proposal = {};
            var row = $(this);
            proposal.WeeklyBucketId = row.find("TD").eq(13).html();
            proposal.Rephase = row.find("TD").eq(10).find("INPUT").val();
            proposal.ProposeAdditional = row.find("TD").eq(11).find("INPUT").val();
            proposal.Remark = row.find("TD").eq(12).find("SELECT").val();
            proposal.BannerName = row.find("TD").eq(0).html();
            proposal.PcMap = row.find("TD").eq(2).html();
            proposal.PlantName = row.find("TD").eq(1).html();
            proposal.Id = row.find("TD").eq(14).html();
            proposal.NextWeekBucket = row.find("TD").eq(7).html();
            if (proposalInputs.length == 0) {
                proposalInputs.push(proposal);
            }
            else {
                if (!proposalInputs.some(element => element.WeeklyBucketId === proposal.WeeklyBucketId)) {
                    proposalInputs.push(proposal);
                }
                else {
                    var index = proposalInputs.findIndex((obj => obj.WeeklyBucketId == proposal.WeeklyBucketId));
                    proposalInputs[index] = proposal;
                }
            }
        });
        return proposalInputs;
    };

    $('#dataTableProposal').on('change', 'input', function () {
        $(this).attr('value', $(this).val());
    });

    $('#submitProposalBtn').click(function () {
        let proposals = getUserInput(proposalInputs);
        //$('#submitProposalModal').modal('hide'); 
        //$("#submitProposalModal").modal("hide");
        $.ajax({
            type: "POST",
            url: "Proposals/SaveProposal",
            data: { "proposals": proposals },
            success: function (data) {
                var ul = document.getElementById('error-messages');
                ul.innerHTML = '';
                window.location.reload();
            },
            error: function (data) {
                var errorMessages = data.responseJSON.value.errorMessages;
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
        });
       
    })


    var tableHistory = $("#dataTableProposalHistory").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Proposals/GetProposalHistoryPagination",
            type: "POST"
        },
        columns: [
            { "data": "submittedAt" }, //0
            { "data": "bannerName" }, //1
            { "data": "plantName" },  //2
            { "data": "pcMap" },       //3
            { "data": "descriptionMap" }, //4
            { "data": "rephase" },      //5
            { "data": "approvedRephase" },   //6
            { "data": "proposeAdditional" },   //7
            { "data": "approvedProposeAdditional" },   //8
            { "data": "remark" },   //9
            { "data": "status" },   //10
            { "data": "approvedBy" },   //11
            { "data": "rejectionReason" },   //12
        ],
        "rowCallback": function (row, data, index) {
            if (data.status == "Approved") {
                $('td:eq(10)', row).css({ color: "green" });
            }
            else if (data.status == "Rejected") {
                $('td:eq(10)', row).css({ color: "red" });
            }
        }
    });
})