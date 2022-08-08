$(document).ready(function () {
    var month = $('#dropDownMonth option:selected').val();
    var year = $('#dropDownYear option:selected').val();
    let remarks = ["", "Big Promotion Period", "Grand Opening", "Additional Store", "Rephase", "Spike Order", "No Baseline Last Year"];
    let proposalInputsReallocate = new Array();
    var tableProposalsReallocate = $("#dataTableProposalReallocate").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Proposals/GetProposalReallocatePagination",
            type: "POST",
            data: function (d) {
                return $.extend(d, { "proposalInputs": getUserInputReallocate(proposalInputsReallocate), "month": month, "year": year, "type": 2 });
            },
            dataSrc: function (json) {
                listBanners = json.dropdownBanner;
                data = json.data;
                return data;
            }
        },
        columns: [
            { "data": "bannerName" },    //0
            { "data": "plantCode" },    //1  
            { "data": "plantName" },    //2  
            { "data": "pcMap" },    //3  
            { "data": "category" },    //4  
            { "data": "currentBucket" }, //5
            { "data": "reallocate" },    //6  
            { "data": "bannerTarget", defaultContent: "" },    //7
            { "data": "remark", defaultContent: "" },    //8
            { "data": "bannerId" },    //9
            { "data": "weeklyBucketId" },    //10
            { "data": "id" },    //11
        ],
        columnDefs: [
            {
                "targets": [3, 4],
            },
            {
                "targets": 6,
                "render": function (data, type, full, meta) {
                    let input = "";

                    if (full.isWaitingApproval) {
                        input = `<input type="number" class="form-control" id="row-${meta.row}-reallocate" name="row-${meta.row}-rephase" disabled min=0 value=${full.reallocate} />`
                    }
                    else {
                        input = `<input type="number" class="form-control" id="row-${meta.row}-reallocate" name="row-${meta.row}-rephase"  min=0 value=${full.reallocate} />`
                    }

                   

                    return input;
                },
                "orderable": false
            },
            {
                targets: [7],
                render: function (data, type, full, meta, fnCallback) {
                    var options = `<option value=""></option>`;

                    listBanners.forEach(function (item) {
                        var listBanner = item;
                        if (listBanner.value == full.bannerTargetId) {
                            options += `<option selected>${listBanner.text}</option>`
                        }
                        else if (full.bannerId != listBanner.value) {
                            options += `<option value="${listBanner.value}">${listBanner.text}</option>`
                        }
                    })

                    var select = "";
                    if (full.isWaitingApproval) {
                        select = `<select class="form-control form-select" id="row-${meta.row}-bannerSource" disabled name="row-${meta.row}-bannerSource">${options}</select>`;
                    }
                    else {
                        select = `<select class="form-control form-select" id="row-${meta.row}-bannerSource" name="row-${meta.row}-bannerSource">${options}</select>`;
                    }
                    return select;
                },
                orderable: false
            },
            {
                targets: [8],
                render: function (data, type, full, meta, fnCallback) {
                    var options = "";

                    remarks.forEach(function (item) {
                        var remark = item;
                        if (remark == full.remark && full.type == 2) {
                            options += `<option selected>${remark}</option>`
                        }
                        else {
                            options += `<option>${remark}</option>`
                        }
                    })
                    var select = "";

                    if (full.isWaitingApproval) {
                        select = `<select class="form-control form-select" id="row-${meta.row}-remarkReallocate" disabled name="row-${meta.row}-remarkReallocate">${options}</select>`;
                    }
                    else {
                        select = `<select class="form-control form-select" id="row-${meta.row}-remarkReallocate" name="row-${meta.row}-remarkReallocate">${options}</select>`;
                    }
                    return select;
                },
                orderable: false
            },
            {
                "targets": [9, 10, 11],
                "className": "hide_column"
            }
        ],
        rowCallback: function (row, data, index) {

        },
        autoWidth: false

    });

    function getUserInputReallocate(proposalInputs) {
        $("#dataTableProposalReallocate TBODY TR").each(function () {
            var proposal = {};
            var row = $(this);
            proposal.BannerName = row.find("TD").eq(0).html();
            proposal.PlantCode = row.find("TD").eq(1).html();
            proposal.PlantName = row.find("TD").eq(2).html();
            proposal.PcMap = row.find("TD").eq(3).html();
            proposal.CurrentBucket = row.find("TD").eq(5).html();
            proposal.Reallocate = row.find("TD").eq(6).find("INPUT").val();
            proposal.BannerTargetId = row.find("TD").eq(7).find("SELECT").val();
            proposal.Remark = row.find("TD").eq(8).find("SELECT").val();
            proposal.BannerId = row.find("TD").eq(9).html();
            proposal.WeeklyBucketId = row.find("TD").eq(10).html();
            proposal.Id = row.find("TD").eq(11).html();
            if (proposalInputsReallocate.length == 0) {
                proposalInputsReallocate.push(proposal);
            }
            else {
                if (!proposalInputsReallocate.some(element => element.WeeklyBucketId === proposal.WeeklyBucketId)) {
                    proposalInputsReallocate.push(proposal);
                }
                else {
                    var index = proposalInputsReallocate.findIndex((obj => obj.WeeklyBucketId == proposal.WeeklyBucketId));
                    proposalInputsReallocate[index] = proposal;
                }
            }
        });
        return proposalInputsReallocate;
    };

    var tableReallocateHistory = $("#dataTableReallocateProposalHistory").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Proposals/GetProposalReallocateHistoryPagination",
            type: "POST",
            data: function (d) {
                return $.extend(d, { "month": month, "year": year, "type": 2 });
            }
        },
        columns: [
            { "data": "submittedAt" }, //0
            { "data": "week" }, //1
            { "data": "bannerName" }, //2
            { "data": "plantName" },  //3
            { "data": "pcMap" },       //4
            { "data": "descriptionMap" }, //5
            { "data": "reallocate" }, //5
            { "data": "bannerTargetName" }, //5
            { "data": "remark" },   //8
            { "data": "status" },   //9
            { "data": "approvedBy" },   //10
            { "data": "rejectionReason" },   //11
        ],
        "rowCallback": function (row, data, index) {
            if (data.status == "Approved") {
                $('td:eq(9)', row).css({ color: "green" });
            }
            else if (data.status == "Rejected") {
                $('td:eq(9)', row).css({ color: "red" });
            }
        }
    });

    $('#dropDownMonth').change(function () {
        month = $('#dropDownMonth option:selected').val();
        redrawTables();
    });


    $('#dropDownYear').change(function () {
        year = $('#dropDownYear option:selected').val();
        redrawTables();
    });

    function redrawTables() {
        tableProposalsReallocate.draw();
        tableReallocateHistory.draw();
    }


    $('#submitProposalReallocateBtn').click(function () {
        let proposals = getUserInputReallocate(proposalInputsReallocate);
        $.ajax({
            type: "POST",
            url: "/Proposals/SaveProposalReallocate",
            data: { "proposals": proposals },
            success: function (data) {
                var ul = document.getElementById('error-messages');
                ul.innerHTML = '';
                redrawTables();
            },
            error: function (data) {
                var errorMessages = data.responseJSON.value.errorMessages;
                var ul = document.getElementById('error-messages');
                ul.innerHTML = '';
                for (var i = 0; i < errorMessages.length; i++) {
                    var li = document.createElement('li');
                    li.appendChild(document.createTextNode(errorMessages[i]));
                    ul.appendChild(li);
                    redrawTables();
                }

                /*           document.body.scrollTop = 0;*/
                document.documentElement.scrollTop = 0;

            }
        });
    });
});