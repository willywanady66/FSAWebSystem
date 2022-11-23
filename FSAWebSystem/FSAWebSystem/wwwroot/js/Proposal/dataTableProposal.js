$(document).ready(function () {
    var currDate = new Date();
    var day = currDate.getDay();
    var hour = currDate.getHours();

    function redrawTable() {
        tableProposals.draw();
        tableHistory.draw();
        tableMonthlyBucketHistory.draw();
        tableWeeklyBucketHistory.draw();
        if (month != currDate.getMonth() + 1 || year != currDate.getFullYear()) {
            $('#openConfirmSubmitModalBtn').prop('disabled', true);
        }
        else {
            $('#openConfirmSubmitModalBtn').prop('disabled', false);
        }
    }

    var month = $('#dropDownMonth option:selected').val();
    var year = $('#dropDownYear option:selected').val();
    let proposalInputs = new Array();
    let remarks = ["Big Promotion Period", "Grand Opening", "Additional Store", "Rephase", "Spike Order", "No Baseline Last Year", "Other"];
    //var proposals = getUserInput(proposalInputs);


    console.log(day + " " + hour);
    var tableProposals = $("#dataTableProposal").DataTable({
        "processing": true,
        "serverSide": true,
        autoWidth: false,
        "ajax": {
            url: getProposalPaginationUrl,
            type: "POST",
            data: function (d) {
                return $.extend(d, { "proposalInputs": getUserInput(proposalInputs), "month": month, "year": year });
            }
        },
        "columns": [
            { "data": "bannerName" }, //0
            { "data": "pcMap" },       //1
            { "data": "descriptionMap" }, //2
            { "data": "ratingRate" },      //3 
            { "data": "monthlyBucket" },   //4
            { "data": "currentBucket" },   //5
            { "data": "nextBucket" },      //6
            { "data": "validBJ" },         //7
            { "data": "remFSA" },         //8
            { "data": "rephase" },         //9
            { "data": "proposeAdditional" }, //10
            { "data": "remark" },           //11
            { "data": "id" }, //12
            { "data": "isWaitingApproval" },    //13
            { "data": "bnrId" },    //14
            { "data": "skuId" },    //15
            { "data": "kam" },   //16
            { "data": "cdm" },   //17
        ],
        "order": [[0, 'asc']],
        "drawCallback": function (data) {
            proposals = data.ajax.proposalInputs;
            $("#week").text("Week: " + data.json.week);
        },
        "columnDefs": [
            {
                "targets": [12, 13, 14, 15, 16, 17],
                "className": "hide_column"
            },
            {
                "targets": [5, 6],
                "render": function (data) {
                    if (data < 0) {
                        data = '(' + data.toString().substring(1) + ')';
                    }
                    return data;
                },
                "orderable": false
            },
            {
                "targets": 9,
                "render": function (data, type, full, meta) {
                    let input = "";
                    if ((day > 4 || (day == 4 && hour > 12) || day == 0) || full.isWaitingApproval) {
                        input = `<input type="number" class="form-control" id="row-${meta.row}-rephase" name="row-${meta.row}-rephase" disabled min=0 value=${full.rephase} />`;
                    }
                    else {
                        input = `<input type="number" class="form-control" id="row-${meta.row}-rephase" name="row-${meta.row}-rephase" min=0 value=${full.rephase} />`
                    }
                    return input;
                },
                "orderable": false
            },
            {
                "targets": 10,
                "render": function (data, type, full, meta) {
                    if (full.week != 1 && !full.isWaitingApproval) {
                        input = `<input type="number" class="form-control" id="row-${meta.row}-additional" name="row-${meta.row}-additional" min=0 value=${full.proposeAdditional} />`;
                    }
                    else {
                        input = `<input type="number" class="form-control" id="row-${meta.row}-additional" name="row-${meta.row}-additional" disabled min=0 value=${full.proposeAdditional} />`;
                    }
                    return input;
                },
                "orderable": false
            },
            {
                "targets": 11,
                "render": function (data, type, full, meta) {
                    var options = "";
                    //var remarks2 = ["Big Promotion Period", "Grand Opening", "Additional Store", "Rephase", "Spike Order", "No Baseline Last Year"];


                    remarks.forEach(function (item) {
                        var remark = item;
                        if (remark == full.remark) {
                            options += `<option selected>${remark}</option>`
                        }
                        else {
                            options += `<option>${remark}</option>`
                        }
                    });

                    var select = '';
                    if (full.isWaitingApproval) {

                        if (!remarks.includes(full.remark)) {
                            options += `<option selected>Other</option>`
                        }

                        select = `<select class="form-control form-select" style="width:100%" disabled id="row-${meta.row}-remark" name="row-${meta.row}-remark">${options}</select>`;
                        if (!remarks.includes(full.remark)) {

                            select += `<input type="text" class="form-control mt-2" disabled value="${full.remark}" id="row-${meta.row}-otherRemark" name="row-${meta.row}-otherRemark">`;
                        }
                    }
                    else {
                        select = `<select class="form-control form-select" onChange="remarkChanged(this)" style="width:100%" id="row-${meta.row}-remark" name="row-${meta.row}-remark">${options}</select>
                                  <input type="text" class="form-control mt-2" style="display:none" id="row-${meta.row}-otherRemark" name="row-${meta.row}-otherRemark">`;
                    }
                    return select;
                },
                "orderable": false,

            }
        ],
        "rowCallback": function (row, data, index) {
            if (data.nextBucket < 0) {
                $('td:eq(6)', row).css({ color: "red" });
            }
            if (data.currentBucket < 0) {
                $('td:eq(5)', row).css({ color: "red" });
            }

            if (data.remFSA < 0) {
                $('td:eq(8)', row).css({ color: "red" });
            }
        },
        fixedColumns: true,
        error: {

        }
    });


    function getUserInput(proposalInputs) { 
        $("#dataTableProposal TBODY TR").each(function () {
            var proposal = {};
            var row = $(this);
            //proposal.WeeklyBucketId = row.find("TD").eq(12).html();
            proposal.Rephase = row.find("TD").eq(9).find("INPUT").val();
            proposal.ProposeAdditional = row.find("TD").eq(10).find("INPUT").val();
            proposal.IsWaitingApproval = row.find("TD").eq(13).html() == 'false' ? false : true;
            if ((proposal.Rephase > 0 || proposal.ProposeAdditional > 0) && !proposal.IsWaitingApproval) {
                proposal.Remark = row.find("TD").eq(11).find("SELECT").val();
                if (proposal.Remark == "Other") {
                    proposal.Remark = row.find("TD").eq(11).find("INPUT").val();
                }
                proposal.BannerName = row.find("TD").eq(0).html();
                proposal.PcMap = row.find("TD").eq(1).html();
                //proposal.PlantName = row.find("TD").eq(1).html();
                proposal.Id = row.find("TD").eq(12).html();
                proposal.NextWeekBucket = row.find("TD").eq(6).html();
                proposal.IsWaitingApproval = row.find("TD").eq(14).html();
                proposal.BannerId = row.find("TD").eq(14).html();
                proposal.SKUId = row.find("TD").eq(15).html();
                proposal.kam = row.find("TD").eq(16).html();
                proposal.cdm = row.find("TD").eq(17).html();
                if (proposalInputs.length == 0) {
                    proposalInputs.push(proposal);
                }
                else {
                    if (!proposalInputs.some(element => element.BannerId === proposal.BannerId)) {
                        proposalInputs.push(proposal);
                    }
                    else {
                        var index = proposalInputs.findIndex((obj => obj.BannerId == proposal.BannerId && obj.PcMap == proposal.PcMap));
                        if (index < 0) {
                            proposalInputs.push(proposal);
                        }
                        else {
                            proposalInputs[index] = proposal;
                        }

                    }
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
        Swal.fire({
            title: 'Loading...',
            html: 'Submitting Proposal',
            timerProgressBar: true,
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
            },
        });
        $.ajax({
            type: "POST",
            url: submitProposalUrl,
            data: { "proposals": proposals },
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
                    redrawTable();
                }
            },
            error: function (data) {

            }
        });
    });

    var tableHistory = $("#dataTableProposalHistory").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getProposalHistoryPaginationUrl,
            type: "POST",
            data: function (d) {
                return $.extend(d, { "month": month, "year": year });
            }
        },
        columns: [
            { "data": "submittedAt" }, //0
            { "data": "week" }, //1
            { "data": "uliWeek", orderable: false }, //2
            { "data": "bannerName" }, //3
            { "data": "pcMap" },       //4
            { "data": "descriptionMap" }, //5
            { "data": "rephase" },      //6
            { "data": "proposeAdditional" },   //7
            { "data": "remark" },   //8
            { "data": "approvalStatus" },   //9
            { "data": "approvedBy" },   //10
            { "data": "approvalNote" },   //11

        ],
        "columnDefs": [{
            "targets": 9,
            "orderable": false
        },
        {
            "targets": 0,
            "render": function (data) {
                var date = moment(data).format("DD-MMM-yyyy HH:mm");
                return date;
            },
        }
        ],
        "rowCallback": function (row, data, index) {
            if (data.approvalStatus == "Approved") {
                $('td:eq(9)', row).css({ color: "green" });
            }
            else if (data.approvalStatus == "Rejected") {
                $('td:eq(9)', row).css({ color: "red" });
            }
        },
        error: {
        }
    });


    var tableMonthlyBucketHistory = $('#dataTableMonthlyBucketHistory').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getMonthlyHistoryUrl,
            type: "POST",
            data: function (d) {
                return $.extend(d, {
                    "month": month, "year": year
                });
            }
        },
        columns: [
            { "data": "createdAt" }, //0
            { "data": "year" }, //1
            { "data": "month" }, //2
            { "data": "bannerName" },//3
            { "data": "pcMap" }, //4
            { "data": "descriptionMap" }, //5
            { "data": "price" }, //6
            { "data": "plantContribution" }, //7
            { "data": "ratingRate" }, //8
            { "data": "tct" }, //9
            { "data": "monthlyTarget" }, //10
        ],
        columnDefs: [
            {
                targets: 0,
                render: function (data) {
                    var date = moment(data).format("DD-MMM-yyyy HH:mm");
                    return date;
                },
            },
            {
                targets: 6,
                render: $.fn.dataTable.render.number(',', '.', 0, '')
            },
            {
                targets: [7, 8, 9],
                render: function (data, type, row) {
                    return data + ' %';
                }
            }
        ],
        error: {
        }

    });


    var tableWeeklyBucketHistory = $('#dataTableWeeklyBucketHistory').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getWeeklyHistoryUrl,
            type: "POST",
            data: function (d) {
                return $.extend(d, {
                    "month": month, "year": year
                });
            }
        },
        columns: [
            { "data": "createdAt" },
            { "data": "year" },
            { "data": "month" },
            { "data": "week" },
            { "data": "uliWeek", orderable: false },
            { "data": "bannerName" },
            { "data": "plantName" },
            { "data": "pcMap" },
            { "data": "descriptionMap" },
            { "data": "dispatchConsume" },
        ],
        columnDefs: [
            {
                targets: 0,
                render: function (data) {
                    var date = moment(data).format("DD-MMM-yyyy HH:mm");
                    return date;
                },
            },
        ],
        error: {

        }
    });

    $('#dropDownMonth').change(function () {
        month = $('#dropDownMonth option:selected').val();
        redrawTable();

    });


    $('#dropDownYear').change(function () {
        year = $('#dropDownYear option:selected').val();
        redrawTable();
        if (year != currDate.getFullYear()) {

            $('#submitProposalBtn').attr('disabled', 'disabled');
        }
        else {
            $('#submitProposalBtn').removeAttr('disabled');
        }
    });
});

function remarkChanged(obj) {
    var index = obj.selectedIndex;
    var id = obj.id.split('-');
    var inputRemarkId = '#' + id[0] + '-' + id[1] + '-' + 'otherRemark';
    if (obj.options[index].text == "Other") {

        $(`#dataTableProposal ${inputRemarkId}`).show();
    }
    else {
        $(`#dataTableProposal ${inputRemarkId}`).hide();
    }

}