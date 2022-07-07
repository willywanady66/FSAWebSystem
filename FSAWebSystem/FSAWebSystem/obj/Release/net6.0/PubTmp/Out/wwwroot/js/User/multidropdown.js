$(document).ready(function () {
    $('#dropDownListBanners').multiselect({
        includeSelectAllOption: true,
        maxHeight: 450
    });

    const selectedText = document.querySelector(".multiselect-selected-text");
    selectedText.classList.add("text-wrap");
});