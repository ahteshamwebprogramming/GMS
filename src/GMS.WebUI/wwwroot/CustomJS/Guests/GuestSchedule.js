function initCreateEventModal() {
    getTaskName();
}

function getTaskName() {
    $.ajax({
        type: "POST",
        url: "/Guests/GetTaskName",
        contentType: 'application/json',
        //data: JSON.stringify(inputDTO),
        success: function (data) {
            let $taskId = $("#AddSchedule").find("[name='TaskId']");
            $taskId.empty();
            $taskId.append('<option value="0">Select Task</option>');
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    $taskId.append('<option department="' + data[i].department + '" value="' + data[i].id + '">' + data[i].taskName + '</option>');
                }
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function GetEmployeeByTaskId(currCtrl) {
    GetResourcesByTaskId(currCtrl);
    let Id = $(currCtrl).val();
    let employeeId = $(currCtrl).attr("employeeid");
    var inputDTO = {
        "Id": Id
    };
    $.ajax({
        type: "POST",
        url: "/Guests/GetEmployeeByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            let $employeeId = $("#AddSchedule").find("[name='EmployeeId']");
            $employeeId.empty();
            $employeeId.append('<option value="0">Select Employee</option>');
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (employeeId == data[i].employeeId) {
                        $employeeId.append('<option selected="selected" value="' + data[i].employeeId + '">' + data[i].employeeName + '(' + data[i].employeeCode + ')' + '</option>');
                    }
                    else {
                        $employeeId.append('<option value="' + data[i].employeeId + '">' + data[i].employeeName + '(' + data[i].employeeCode + ')' + '</option>');
                    }
                }
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}
function GetResourcesByTaskId(currCtrl) {

    let Id = $(currCtrl).val();
    var inputDTO = {
        "Id": Id
    };
    let resourceId = $(currCtrl).attr("resourceid");
    $.ajax({
        type: "POST",
        url: "/Guests/GetResourcesByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            let $resourceId = $("#AddSchedule").find("[name='ResourceId']");
            $resourceId.empty();
            $resourceId.append('<option value="0">Select Resources</option>');
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (resourceId == data[i].id) {
                        $resourceId.append('<option selected="selected" value="' + data[i].id + '">' + data[i].resourceName + '</option>');
                    }
                    else {
                        $resourceId.append('<option value="' + data[i].id + '">' + data[i].resourceName + '</option>');
                    }

                }
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function initFlatPickerDuration() {
    flatpickr("#durationPicker", {
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
        onChange: function (selectedDates, dateStr) {
            $("#selectedDuration").val(dateStr);
            CalculateEndTime();
        }
    });
    $("#begintime").change(function () {
        CalculateEndTime();
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


function CalculateEndTime1() {
    let startTime = $("#AddSchedule").find("[name='StartTime']").val();
    let duration = $("#AddSchedule").find("[name='Duration']").val();

    if (!startTime || !duration) {
        // Handle invalid input (optional)
        $("#AddSchedule").find("[name='EndTime']").val("Invalid input");
        return;
    }
    // Parse StartTime and Duration
    let startMoment = moment(startTime, "HH:mm"); // Create a Moment.js object for StartTime
    let durationParts = duration.split(":").map(Number); // Split Duration into [hours, minutes]

    // Add Duration to StartTime
    let endMoment = startMoment
        .add(durationParts[0], "hours")  // Add hours
        .add(durationParts[1], "minutes")  // Add minutes
        .subtract(1, "seconds"); // Subtract 1 second

    // Format EndTime as HH:mm and set the value
    $("#AddSchedule").find("[name='EndTime']").val(endMoment.format("HH:mm"));

    /*$("#AddSchedule").find("[name='EndTime']").val("00:10");*/
}