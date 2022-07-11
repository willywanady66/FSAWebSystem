$(document).ready(function () {
    $("#dataTableProposal").DataTable({
        "serverSide": true,
        "bProcessing": true,
        "sAjaxSource": "Proposals/GetProposalPagination",
        "bSort": true,
        "aoColumns": [
            { "mData": "bannerName" },
            { "mData": "pcMap" },
            { "mData": "descriptionMap" },
            { "mData": "ratingRate" },
            { "mData": "monthlyBucket" },
            { "mData": "currentBucket" },
            { "mData": "nextBucket" },
            { "mData": "validBJ" },
            { "mData": "remFSA" },
            { "mData": "rephase" },
            { "mData": "proposeAddional" },
            { "mData": "remark" },
            //{ "mData": "weeklyBucketId" },
        ],
        "fnServerData": function (source, data, callback) {
            $.ajax({
                "dataType": 'json',
                "contentType": "application/json; charset=utf-8",
                "type": "GET",
                "url": source,
                "data": data,
                "success": function (response) {
                    callback(response);
                }
            });
        },
        "order": [0, "asc"]
    });

   
});