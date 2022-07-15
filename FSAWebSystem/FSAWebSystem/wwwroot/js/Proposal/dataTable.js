$(document).ready(function () {

    let proposals = getUserInput();
    var table = $("#dataTableProposal").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "Proposals/GetProposalPagination",
            type: "POST",
            data: {"proposalInputs" : proposals}
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
            { "mData": "rephase" },         //10
            { "mData": "proposeAddional" }, //11
            { "mData": "remark" },           //12
            { "mData": "weeklyBucketId" }   //13
        ],
        //"columnDefs": [
        //    {
        //        "targets": 13,
        //        "className": "hide_column"
        //    },
        //    {
        //        "targets": [6, 7],
        //        "render": function (data) {
        //            if (data < 0) {
        //                data = '(' + data.toString().substring(1) + ')';
        //            }
        //            return data;
        //        }
        //    },
        //    {
        //        "targets": 10,
        //        "render": function (data, type, full, meta) {
        //            return `<input type="number" class="form-control" id="row-${meta.row}-rephase" name="row-${meta.row}-rephase" value=${0} />`;
        //        }
        //    },
        //    {
        //        "targets": 11,
        //        "render": function (data, type, full, meta) {
        //            return `<input type="number" class="form-control" id="row-${meta.row}-additional" name="row-${meta.row}-additional" value=${0} />`;
        //        }
        //    },

        //    {
        //        "targets": 12,
        //        "render": function (data, type, full, meta) {
        //            return `<select class="form-control" id="row-${meta.row}-remark" name="row-${meta.row}-remark"/>`;
        //        }
        //    }

        //],
        //"rowCallback": function (row, data, index) {
        //    if (data.nextBucket < 0) {
        //        $('td:eq(7)', row).css({ color: "red" });
        //    }
        //    if (data.currentBucket < 0) {
        //        $('td:eq(6)', row).css({ color: "red" });
        //    }
        //}
        
    });


    function getUserInput () {
        var proposalInputs = new Array();
        //$("#dataTableProposal TBODY TR").each(function () {
        //    var row = $(this);
        //    var proposal = {};
        //    proposal.WeeklyBucketId = row.find("TD").eq(13).html();
        //    proposal.Rephase = row.find("TD").eq(10).find("INPUT").val();
        //    proposal.ProposeAdditional = row.find("TD").eq(11).find("INPUT").val();
        //    proposal.Remark = row.find("TD").eq(12).find("INPUT").val();
        //    proposalInputs.push(proposal);
        //});
        var proposal = {};
            proposal.WeeklyBucketId = "ABC";
            proposal.Rephase = 500;
            proposal.ProposeAdditional =100;
            proposal.Remark = "ASD";
        proposalInputs.push(proposal);

        //proposalInputs = {
        //    "proposalInputs": proposalInputs};
        return proposalInputs;
    };

    $('#dataTableProposal').on('change', 'input', function () {
        //Get the cell of the input
        var cell = $(this).closest('td');

        var row = $(this).closest('tr');

        //update the input value
        $(this).attr('value', $(this).val());

        console.log($(this).val());
        //invalidate the DT cache
        //table.cell($(cell)).invalidate().draw();

    });
})