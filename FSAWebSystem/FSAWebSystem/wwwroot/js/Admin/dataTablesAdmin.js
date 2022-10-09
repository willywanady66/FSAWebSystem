let dataTableWl;
let dataTableUser;
let dataTableSKU;
let dataTableCategoy;
let dataTableAndromeda;
let dataTableBottomPrice;
let dataTableITrust;
let dataTableBanners;
let dataTablePlants;
$(document).ready(function () {

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        switch (e.target.id) {
            case "users-tab": {
                dataTableUser.draw();
                break;
            }
            case "wl-tab": {
                dataTableWl = $('#dataTableWorkLevels').DataTable({
                    "processing": true,
                    "serverSide": true,
                    "ajax": {
                        url: getWLUrl,
                        type: "POST"
                    },
                    "columns": [
                        { "data": "id" }, //0
                        { "data": "wl" },  //1
                        //{ "data": "status" },  //2
                        //{ "data": "id" }  //3

                    ],
                    columnDefs: [
                        {
                            targets: 0,
                            searchable: false,
                            orderable: false,
                            "render": function (data, type, full, meta) {
                                return meta.row + 1 + meta.settings._iDisplayStart;
                            },
                        },
                    ]
                });
                break;
            }
            case "skus-tab": {
                dataTableSKU = $('#dataTableSKUs').DataTable({
                    "processing": true,
                    "serverSide": true,
                    "ajax": {
                        url: getSKUUrl,
                        type: "POST"
                    },
                    "columns": [
                        { "data": "pcMap" },  //0
                        { "data": "descriptionMap" },       //1
                        { "data": "category" },  //2
                        { "data": "status" }, //3
                        { "data": "id" } //4
                    ],
                    columnDefs: [{
                        targets: 4,
                        orderable: false,
                        className: 'text-center',
                        "render": function (data, type, full, meta) {
                            return `<a href="${editSKUUrl}/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                        }
                    }],
                    "rowCallback": function (row, data, index) {
                        if (data.status == "Active") {
                            $('td:eq(3)', row).css({ color: "green" });
                        }
                        else {
                            $('td:eq(3)', row).css({ color: "red" });
                        }
                    }
                });
                break;
            }

            case "productCategories-tab": {
                dataTableCategoy = $('#dataTableProductCategory').DataTable({
                    "processing": true,
                    "serverSide": true,
                    "ajax": {
                        url: getCategUrl,
                        type: "POST"
                    },
                    "columns": [
                        { "data": "id" },  //0
                        { "data": "categoryProduct" }       //1
                    ],
                    columnDefs: [
                        {
                            targets: 0,
                            searchable: false,
                            orderable: false,
                            "render": function (data, type, full, meta) {
                                return meta.row + 1 + meta.settings._iDisplayStart;
                            }
                        }
                    ]
                });
                break;
            }
            case "andromeda-tab": {
                dataTableAndromeda.draw();
                break;
            }
            case "bottomPrice-tab": {
                dataTableBottomPrice = $('#dataTableBottomPrice').DataTable({
                    "processing": true,
                    "serverSide": true,
                    "ajax": {
                        url: getBottomPriceUrl,
                        type: "POST"
                    },
                    "columns": [
                        { "data": "pcMap" },  //0
                        { "data": "description" },  //1
                        { "data": "avgNormalPrice" },       //2
                        { "data": "avgBottomPrice" },       //3
                        { "data": "avgActualPrice" },       //4
                        { "data": "minActualPrice" },       //5
                        { "data": "gap" },       //6
                        { "data": "remarks" }       //7
                    ]
                });
                break;

            }
            case "itrust-tab": {
                dataTableITrust = $('#dataTableITrust').DataTable({
                    "processing": true,
                    "serverSide": true,
                    "ajax": {
                        url: getITrustUrl,
                        type: "POST"
                    },
                    "columns": [
                        { "data": "pcMap" },  //0
                        { "data": "description" },  //1
                        { "data": "sumIntransit" },       //2
                        { "data": "sumStock" },       //3
                        { "data": "sumFinalRpp" },       //4
                        { "data": "distStock" }       //5
                    ]
                });
                break;
            }
            case "banners-tab": {
                dataTableBanners = $('#dataTableBanners').DataTable({
                    "processing": true,
                    "serverSide": true,
                    "ajax": {
                        url: getBannerUrl,
                        type: "POST"
                    },
                    "columns": [
                        { "data": "id" },  //0
                        { "data": "bannerName" }       //1
                    ],
                    columnDefs: [
                        {
                            targets: 0,
                            searchable: false,
                            orderable: false,
                            "render": function (data, type, full, meta) {
                                return meta.row + 1 + meta.settings._iDisplayStart;
                            }
                        }
                    ]
                });
                break;
            }
            case "plants-tab":
                {
                    dataTablePlants = $('#dataTablePlants').DataTable({
                        "processing": true,
                        "serverSide": true,
                        "ajax": {
                            url: getPlantUrl,
                            type: "POST"
                        },
                        "columns": [
                            { "data": "plantCode" },       //1
                            { "data": "plantName" }       //2
                        ]
                    });
                    break;
                }

        }
    });


    $('a[data-toggle="tab"]').on('hidden.bs.tab', function (e) {
        switch (e.target.id) {
            case "user-tab": {
                dataTableUser.destroy();
            }
            case "wl-tab": {
                dataTableWl.destroy();
                break;
            }
            case "skus-tab": {
                dataTableSKU.destroy();
                break;
            }
            case "productCategories-tab": {
                dataTableCategoy.destroy();
                break;
            }
            case "bottomPrice-tab": {
                dataTableBottomPrice.destroy()
                break;
            }
            case "itrust-tab": {
                dataTableITrust.destroy();
                break;
            }
            case "banners-tab": {
                dataTableBanners.destroy();
                break;
            }
            case "plants-tab": {
                dataTablePlants.destroy();
                break;
            }

        }
    });

    dataTableUser = $('#dataTableUsers').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            async: true,
            url: getUsersUrl,
            type: "POST"
        },
        "columns": [
            { "data": "name" },  //0
            { "data": "wlName" }, //1
            { "data": "email" },       //2
            { "data": "role" }, //3
            { "data": "bannerName" },      //4
            { "data": "status" }, //5
            { "data": "userId" }, //6
            { "data": "id" }, //7
            { "data": "userId" }, //8
        ],
        columnDefs: [
            {
                "targets": [7, 8],
                "className": "hide_column"
            },
            {
                targets: 6,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {

                    return `<a href="${editUserUrl}/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`

                    return null;

                }
            }
        ],
        "rowCallback": function (row, data, index) {
            if (data.status == "Active") {
                $('td:eq(5)', row).css({ color: "green" });
            }
            else {
                $('td:eq(5)', row).css({ color: "red" });
            }
        }
    });


    $('#dataTableBannerPlants').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            async: true,
            url: getBannerPlantUrl,
            type: "POST"
        },
        "columns": [
            { "data": "banner.trade" },  //0
            { "data": "cdm" },  //1
            { "data": "kam" },  //2
            { "data": "banner.bannerName" },       //3
            { "data": "plant.plantName" }, //4
            { "data": "plant.plantCode" },      //5
            { "data": "isActive" },      //6
            { "data": "id" } //7
        ],
        columnDefs: [
            {
                targets: 7,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="${editBannerUrl}/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                }
            },
                {
                "targets": 6,
                "render": function (data, type, full, meta) {
                    if (full.isActive) {
                        data = "Active";
                    }
                    else {
                        data = "Non-Active";
                    }
                    return data;
                }
            },
        ],
        "rowCallback": function (row, data, index) {
            if (data.isActive) {
                $('td:eq(6)', row).css({ color: "green" });
            }
            else {
                $('td:eq(6)', row).css({ color: "red" });
            }
        }
    });

    dataTableAndromeda = $('#dataTableAndromeda').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getAndromedaUrl,
            type: "POST"
        },
        "columns": [
            { "data": "pcMap" },
            { "data": "description" },
            { "data": "uuStock" },
            { "data": "itThisWeek" },
            { "data": "rracT13Wk" },
            { "data": "weekCover" },
        ]
    })



    

    var month = $('#dropDownMonth option:selected').val();
    var year = $('#dropDownYear option:selected').val();


    var dtCalendar = $('#dataTableCalendar').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {

            url: getFSACal,
            type: "GET",
            data: function (d) {
                return $.extend(d, { "month": month, "year": year });
            }
        },
        "columns": [
            { "data": "week" },
            { "data": "startDate" },
            { "data": "endDate" },
            { "data": "year" },
            { "data": "month" },
            { "data": "id" }
        ],
        columnDefs: [
            {
                targets: 5,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="${editFSACalUrl}/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                }
            },
            {
                targets: 'all',
                defaultContent: ""
            }
        ],
        rowsGroup: [5],
        "rowCallback": function (row, data, index) {
            $('td:eq(5)', row).addClass('align-middle');
        },
        "paging": false,
        "info": false,
        searching: false
    });


    $('#dropDownMonth').change(function () {
        month = $('#dropDownMonth option:selected').val();
        dtCalendar.draw();
    });


    $('#dropDownYear').change(function () {
        year = $('#dropDownYear option:selected').val();
        dtCalendar.draw();
    });

    var loggedUser = $('#loggedUser').val();
    var docVal = $('#documentType option:selected').val();
    var doc = $('#documentType option:selected').text();
    if (doc !== "Monthly Bucked") {
        $('#uploadMonthGroup').hide();

    }


    //if (doc === "Andromeda" || doc === "Bottom Price" || doc === "I-TRUST") {
    //    $('#uploadBtn').show(false);
    //    $('#submitUploadDoc').hide(false);
    //}
    //else {
    //    $('#uploadBtn').hide(false);
    //    $('#submitUploadDoc').show(false);
    //}

    $('#documentType').change(function () {
        doc = $('#documentType option:selected').text();
        docVal = $('#documentType option:selected').val();
        if (doc !== "Monthly Bucket") {
            $('#uploadMonthGroup').hide();
        }
        else {
            $('#uploadMonthGroup').show();
        }

        //if (doc === "Andromeda" || doc === "Bottom Price" || doc === "I-TRUST") {
        //    $('#uploadBtn').show(false);
        //    $('#submitUploadDoc').hide(false);
        //}
        //else {
        //    $('#uploadBtn').hide(false);
        //    $('#submitUploadDoc').show(false);
        //}
    });

    var files;
    $('input[type=file]').on('change', function () {
        files = event.target.files;
    });

    $('#uploadBtn').click(function (e) {
        Swal.fire({
            title: 'Loading...',
            html: 'Uploading Document',
            timerProgressBar: true,
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading()
            },
        });

        var formData = new FormData();
        $.each(files, function (key, value) {
            formData.append("excelDocument", value);
        });

        formData.append("documentType", docVal);
        formData.append("loggedUser", loggedUser);

        $.ajax({
            url: uploadDocumentUrl,
            type: "POST",
            data: formData,
            contentType : 'multipart/form-data',
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.value.data) {
                    var errorFileData = data.value.data;
                    var content = atob(errorFileData);
                    const buffer = new ArrayBuffer(content.length);
                    const bytes = new Uint8Array(buffer);
                    var blob = new Blob([content], { type: "application/octet-stream" });
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    var fileName = "ListError.txt";
                    link.download = fileName;
                    link.click();

                }
                else if (data.value) {
                    if (data.value.errorMessages) {
                        var errorMessages = data.value.errorMessages;
                        var ul = document.getElementById('error-messages');
                        ul.innerHTML = '';
                        for (var i = 0; i < errorMessages.length; i++) {
                            var li = document.createElement('li');
                            li.appendChild(document.createTextNode(errorMessages[i]));
                            ul.appendChild(li);
                        }

                        //document.body.scrollTop = 0;
                        //document.documentElement.scrollTop = 0;
                    }
                }

                $("#uploadFileForm")[0].reset();
                Swal.close();
       
            },
       
        })
    });

    var monthUli = $('#dropDownMonthULI option:selected').val();
    var yearUli = $('#dropDownYearULI option:selected').val();
    $('#dropDownMonthULI').change(function () {
        monthUli = $('#dropDownMonthULI option:selected').val();
        dtULICalendar.draw();
    });


    $('#dropDownYearULI').change(function () {
        yearUli = $('#dropDownYearULI option:selected').val();
        dtULICalendar.draw();
    });
    var dtULICalendar = $('#dataTableUliCalendar').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: getULICal,
            type: "GET",
            data: function (d) {
                return $.extend(d, { "month": monthUli, "year": yearUli });
            }
        },
        "columns": [
            { "data": "week" },
            { "data": "startDate" },
            { "data": "endDate" },
            { "data": "year" },
            { "data": "month" },
            { "data": "id" }
        ],
        columnDefs: [
            {
                targets: 5,
                orderable: false,
                className: 'text-center',
                "render": function (data, type, full, meta) {
                    return `<a href="${editULICalUrl}/${full.id}">
                                <i class="fas fa-pen"></i>
                            <a/>`
                }
            },
            {
                targets: 'all',
                defaultContent: ""
            }
        ],
        rowsGroup: [5],
        "rowCallback": function (row, data, index) {
            $('td:eq(5)', row).addClass('align-middle');
        },
        "paging": false,
        "info": false,
        searching: false
    });

});



   
