﻿$(document).ready(function () {
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

});