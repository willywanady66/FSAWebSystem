
$(document).ready(function () {
    let proposalInputs = new Array();
    let remarks = ["", "Big Promotion Period", "Grand Opening", "Additional Store", "Rephase", "Spike Order", "No Baseline Last Year"];
    //var proposals = getUserInput(proposalInputs);
    var table = $("#dataTableProposal").DataTable({
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
            { "data": "weeklyBucketId" }   //13
        ],
        "order": [[0, 'asc']],
        "drawCallback": function (data) {
            proposals = data.ajax.proposalInputs;
        },
        "columnDefs": [
            {
                "targets": 13,
                "className": "hide_column"
            },
            {
                "targets": [6, 7, 9],
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
            proposal.PcMap = row.find("TD").eq(1).html();
            if (proposalInputs.length == 0) {
                proposalInputs.push(proposal);
            }
            else {
                if (!proposalInputs.some(element => element.WeeklyBucketId === proposal.WeeklyBucketId)) {
                    proposalInputs.push(proposal);
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
            success: function () {
                table.clear().draw();
            }
        });
       
    })
})