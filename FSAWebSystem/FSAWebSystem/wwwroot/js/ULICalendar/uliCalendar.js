$(document).ready(function () {
    var month = $('#dropDownListMonths option:selected').val();
    var year = $('#dropDownListYears option:selected').val();

    $('#dropDownListMonths').change(function (e) {
        month = $('#dropDownListMonths option:selected').val();
        reload();
        index = 0;
        e.stopImmediatePropagation();
        return false;
    });
    var index = 0;
    function reload() {
        return $.ajax({
            type: "GET",
            url: "/ULICalendars/Reload",
            data: { "month": month, "year": year },
            async: false,
            success: function (dt) {
                $('#tableCalendar tr').find('td:first').each(function () {
                    
                    var cell = $(this);
                    var z = dt;
                    cell.find('INPUT').val(dt.uliCalendarDetails[index].week);
                    index++;
                });
            }
        });
       
    }

    $('#dropDownListYears').change(function () {
        year = $('#dropDownListYears option:selected').val();
        redrawTable();
        if (year != currDate.getFullYear()) {

            $('#submitProposalBtn').attr('disabled', 'disabled');
        }
        else {
            $('#submitProposalBtn').removeAttr('disabled');
        }
    }); 
});