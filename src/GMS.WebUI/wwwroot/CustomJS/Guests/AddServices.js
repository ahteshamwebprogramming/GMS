$(document).ready(function () {







});
function TaskChangeEvent(currCtrl) {
    GetDurationByService(currCtrl);
    GetPriceByService(currCtrl);
}
function GetPriceByService(currCtrl) {
    let price = $(currCtrl).find("option:selected").attr('price');    
    $("#AddSchedule").find("[name='Price']").val(price);
}
function GetDurationByService(currCtrl) {
    let durationFromDBUpdate = $(currCtrl).attr("duration");
    let Id = $(currCtrl).val();
    var inputDTO = {
        "Id": Id
    };
    let resourceId = $(currCtrl).attr("resourceid");
    $.ajax({
        type: "POST",
        url: "/Guests/GetTaskByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {

            if (durationFromDBUpdate == null || durationFromDBUpdate == undefined || durationFromDBUpdate == "") {
                if (data != null) {

                    durationPicker.setDate(data.duration, false);
                    $("#selectedDuration").val(data.duration);
                    CalculateEndTime();
                }
            }
            else {
                durationPicker.setDate(durationFromDBUpdate, false);
                $("#selectedDuration").val(durationFromDBUpdate);
                CalculateEndTime();
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}
function CalculateEndTime() {
    let startDate = $("#AddSchedule").find("[name='StartDate']").val();
    let startTime = $("#AddSchedule").find("[name='StartTime']").val();
    let duration = $("#AddSchedule").find("[name='Duration']").val();

    // Validate inputs
    if (!startDate || !startTime || !duration) {
        $("#AddSchedule").find("[name='EndDate']").val("");
        $("#AddSchedule").find("[name='EndTime']").val("Invalid input");
        return;
    }

    // Parse inputs
    let durationParts = duration.split(":").map(Number); // Split duration into [hours, minutes]
    let startDateTime = moment(`${startDate} ${startTime}`, "DD-MMM-YYYY HH:mm"); // Combine date and time

    if (!startDateTime.isValid()) {
        $("#EndDate").val(""); // Clear End Date
        $("#endtime").val("Invalid input"); // Set placeholder for invalid input
        return;
    }

    // Calculate End DateTime
    let endDateTime = startDateTime
        .clone()
        .add(durationParts[0], "hours")  // Add hours from duration
        .add(durationParts[1], "minutes") // Add minutes from duration
        .subtract(1, "seconds"); // Subtract 1 second

    // Update End Date and End Time fields
    $("#EndDate").val(endDateTime.format("DD-MMM-YYYY")); // Set End Date
    $("#endtime").val(endDateTime.format("HH:mm")); // Set End Time
}
function initFlatPickerServices() {
    const now = new Date();
    durationPicker = flatpickr("#durationPicker", {
        enableTime: true,
        noCalendar: true,
        time_24hr: true,
        defaultHour: 0,
        defaultMinute: 0,
        dateFormat: "H:i",
        onChange: function (selectedDates, dateStr) {
            $("#selectedDuration").val(dateStr);
            CalculateEndTime();
        }
    });
    flatpickr("#StartDate", {
        enableTime: false,
        noCalendar: false,
        dateFormat: "d-M-y",
        defaultDate: now, // Set current date
        onChange: function (selectedDates, dateStr) {
            $("#selectedDuration").val(dateStr);
            CalculateEndTime();
        }
    });
    // Set current time to begintime input
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    $("#begintime").val(`${hours}:${minutes}`);
    $("#begintime").change(function () {
        CalculateEndTime();
    });
}