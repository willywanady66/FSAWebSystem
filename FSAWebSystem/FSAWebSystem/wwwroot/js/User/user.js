$(document).ready(function () {
    $('#listSkuGroup').attr('hidden', true);
    $('#listCategoryGroup').attr('hidden', true);

    var wl = $('#dropDownListWorkLevels option:selected').text();
    showHideSkuCategDropDown();

    $('#dropDownListBanners').multiselect({
        enableFiltering:true,
        includeSelectAllOption: true,
        maxHeight: 450
    });

    $('#dropDownListMenu').multiselect({
        includeSelectAllOption: true
    });
   
    const selectedText = document.querySelector(".multiselect-selected-text");
    if (selectedText) {
        selectedText.classList.add("text-wrap");
    }
    $('#dropDownListCategory').multiselect({
        enableFiltering: true,
        includeSelectAllOption: true,
        maxHeight: 450
    });

    $('#dropDownListSku').multiselect({
        enableFiltering: true,
        includeSelectAllOption: true,
        maxHeight: 450
    });



    $('#dropDownListWorkLevels').change(function () {
        showHideSkuCategDropDown();
    });

    function showHideSkuCategDropDown() {
        if ($('#dropDownListWorkLevels option:selected').text() == "CCD") {
            $('#listSkuGroup').attr('hidden', false);
            $('#listCategoryGroup').attr('hidden', false);
        }
        else {
            $('#listSkuGroup').attr('hidden', true);
            $('#listCategoryGroup').attr('hidden', true);
        }
    }
});