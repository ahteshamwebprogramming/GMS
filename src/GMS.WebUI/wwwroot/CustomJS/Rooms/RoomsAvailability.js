function RoomAvailabilityListPartialView() {

    let dates = {}
    dates.StartDate = moment($("#flatpickr-startdate").val(), "DD-MMM-YYYY").format("YYYY-MM-DD"); //'2025-01-01';//$("#flatpickr-startdate").val();
    dates.EndDate = moment($("#flatpickr-enddate").val(), "DD-MMM-YYYY").format("YYYY-MM-DD"); // '2025-01-11';//$("#flatpickr-enddate").val();

    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/MasterScheduleListPartialView',
        data: JSON.stringify(dates),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            //UnblockUI();
            $('#div_RoomAvailabilityListPartial').html(data);
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function ShowFilteredRecords() {
    RoomAvailabilityListPartialView();
}

function ViewGuestsInRoom(RoomNumber, date) {
    let inputDTO = {}
    inputDTO.RNumber = RoomNumber; //'2025-01-01';//$("#flatpickr-startdate").val();
    inputDTO.DateValue = moment(date, "YYYY-MM-DD").format("YYYY-MM-DD"); // '2025-01-11';//$("#flatpickr-enddate").val();

    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/ViewGuestsInRoom',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            //UnblockUI();
            $('#div_GuestsDetailsPartial').html(data);
            $("#btnModelGuestsDetails").click();            
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}