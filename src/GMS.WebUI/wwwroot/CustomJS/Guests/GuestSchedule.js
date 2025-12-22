let durationPicker;
function initCreateEventModal() {
    resetAddScheduleForm();
    getTaskName();
    initTherapistDropdowns();
}

function initTherapistDropdowns() {
    $EmployeeId1 = $("#AddSchedule").find("[name='EmployeeId1']");
    $EmployeeId2 = $("#AddSchedule").find("[name='EmployeeId2']");
    $EmployeeId3 = $("#AddSchedule").find("[name='EmployeeId3']");
    $("[name='EmployeeId1'], [name='EmployeeId2'], [name='EmployeeId3']").change(function () {
        validateTherapistDropdowns($(this));
    });
}
function validateTherapistDropdowns(currentDropdown) {
    let dropdown1Value = $("#AddSchedule").find("[name='EmployeeId1']").val();
    let dropdown2Value = $("#AddSchedule").find("[name='EmployeeId2']").val();
    let dropdown3Value = $("#AddSchedule").find("[name='EmployeeId3']").val();

    // Clear all previous error messages
    clearTherapistErrors();

    // Check for duplicates - no two therapists can be the same
    let currentDropdownName = currentDropdown.attr('name');
    let currentDropdownValue = currentDropdown.val();
    
    // If current dropdown value is 0 or empty, no error
    if (currentDropdownValue === "" || currentDropdownValue === "0") {
        return;
    }
    
    // Check if current dropdown matches any other dropdown
    if (currentDropdownName === "EmployeeId1") {
        if (currentDropdownValue === dropdown2Value && dropdown2Value !== "0") {
            currentDropdown.val(0);
            showTherapistError(currentDropdown, "Cannot select same healer");
        } else if (currentDropdownValue === dropdown3Value && dropdown3Value !== "0") {
            currentDropdown.val(0);
            showTherapistError(currentDropdown, "Cannot select same healer");
        }
    } else if (currentDropdownName === "EmployeeId2") {
        if (currentDropdownValue === dropdown1Value && dropdown1Value !== "0") {
            currentDropdown.val(0);
            showTherapistError(currentDropdown, "Cannot select same healer");
        } else if (currentDropdownValue === dropdown3Value && dropdown3Value !== "0") {
            currentDropdown.val(0);
            showTherapistError(currentDropdown, "Cannot select same healer");
        }
    } else if (currentDropdownName === "EmployeeId3") {
        if (currentDropdownValue === dropdown1Value && dropdown1Value !== "0") {
            currentDropdown.val(0);
            showTherapistError(currentDropdown, "Cannot select same healer");
        } else if (currentDropdownValue === dropdown2Value && dropdown2Value !== "0") {
            currentDropdown.val(0);
            showTherapistError(currentDropdown, "Cannot select same healer");
        }
    }
}

function clearTherapistErrors() {
    // Remove all error messages
    $("[name='EmployeeId1'], [name='EmployeeId2'], [name='EmployeeId3']").each(function() {
        $(this).removeClass('is-invalid');
        $(this).closest('.col-4').find('.healer-error-message').remove();
    });
}

function showTherapistError(field, message) {
    // Add error class to the field
    field.addClass('is-invalid');
    
    // Remove existing error message if any
    field.closest('.col-4').find('.healer-error-message').remove();
    
    // Add error message below the field
    field.closest('.col-4').append('<div class="healer-error-message text-danger small mt-1">' + message + '</div>');
}

function validateTherapistDropdownsOnSave() {
    clearTherapistErrors();
    let dropdown1Value = $("#AddSchedule").find("[name='EmployeeId1']").val();
    let dropdown2Value = $("#AddSchedule").find("[name='EmployeeId2']").val();
    let dropdown3Value = $("#AddSchedule").find("[name='EmployeeId3']").val();

    let hasError = false;
    let errorMessages = [];

    // Check for duplicates
    if (dropdown1Value !== "" && dropdown1Value !== "0") {
        if (dropdown1Value === dropdown2Value && dropdown2Value !== "0") {
            hasError = true;
            showTherapistError($("[name='EmployeeId2']"), "Cannot select same healer");
            errorMessages.push("Healer 1 and Healer 2 cannot be the same");
        }
        if (dropdown1Value === dropdown3Value && dropdown3Value !== "0") {
            hasError = true;
            showTherapistError($("[name='EmployeeId3']"), "Cannot select same healer");
            if (!errorMessages.includes("Healer 1 and Healer 3 cannot be the same")) {
                errorMessages.push("Healer 1 and Healer 3 cannot be the same");
            }
        }
    }
    
    if (dropdown2Value !== "" && dropdown2Value !== "0") {
        if (dropdown2Value === dropdown1Value && dropdown1Value !== "0") {
            hasError = true;
            showTherapistError($("[name='EmployeeId1']"), "Cannot select same healer");
            if (!errorMessages.includes("Healer 1 and Healer 2 cannot be the same")) {
                errorMessages.push("Healer 1 and Healer 2 cannot be the same");
            }
        }
        if (dropdown2Value === dropdown3Value && dropdown3Value !== "0") {
            hasError = true;
            showTherapistError($("[name='EmployeeId3']"), "Cannot select same healer");
            if (!errorMessages.includes("Healer 2 and Healer 3 cannot be the same")) {
                errorMessages.push("Healer 2 and Healer 3 cannot be the same");
            }
        }
    }
    
    if (dropdown3Value !== "" && dropdown3Value !== "0") {
        if (dropdown3Value === dropdown1Value && dropdown1Value !== "0") {
            hasError = true;
            showTherapistError($("[name='EmployeeId1']"), "Cannot select same healer");
            if (!errorMessages.includes("Healer 1 and Healer 3 cannot be the same")) {
                errorMessages.push("Healer 1 and Healer 3 cannot be the same");
            }
        }
        if (dropdown3Value === dropdown2Value && dropdown2Value !== "0") {
            hasError = true;
            showTherapistError($("[name='EmployeeId2']"), "Cannot select same healer");
            if (!errorMessages.includes("Healer 2 and Healer 3 cannot be the same")) {
                errorMessages.push("Healer 2 and Healer 3 cannot be the same");
            }
        }
    }

    return {
        isValid: !hasError,
        messages: errorMessages
    };
}

// Clear create-event form to avoid leaking values from previous edits
function resetAddScheduleForm() {
    const $form = $("#AddSchedule");
    if (!$form.length) return;

    // Reset basic inputs
    $form[0].reset();
    $form.find("[name='ScheduleId']").val('');
    $form.find("[name='TaskId']").val('0').removeAttr("employeeid1 employeeid2 employeeid3 resourceid duration");
    $form.find("[name='ResourceId']").val('0');
    $form.find("[name='EmployeeId1']").val('0');
    $form.find("[name='EmployeeId2']").val('0');
    $form.find("[name='EmployeeId3']").val('0');

    // Clear dates/times and duration helpers
    $form.find("[name='StartDate']").val('');
    $form.find("[name='StartTime']").val('');
    $form.find("[name='EndDate']").val('');
    $form.find("[name='EndTime']").val('');
    $form.find("[name='NoOfDays']").val('1');
    $("#selectedDuration").val('');

    if (durationPicker) {
        durationPicker.clear();
        durationPicker.setDate("00:00", false);
    }
    
    // Clear error messages
    clearTherapistErrors();
    $("#modalFooterErrorMessage").hide();
    
    // Show X field for new schedules (ScheduleId is empty)
    $("#noOfDaysContainer").show();
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
            $taskId.append('<option value="0"></option>');
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
    GetTaskByTaskId(currCtrl)
    let Id = $(currCtrl).val();
    let employeeId1 = $(currCtrl).attr("employeeid1");
    let employeeId2 = $(currCtrl).attr("employeeid2");
    let employeeId3 = $(currCtrl).attr("employeeid3");
    var inputDTO = {
        "Id": Id
    };
    $.ajax({
        type: "POST",
        url: "/Guests/GetEmployeeByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            let $employeeId1 = $("#AddSchedule").find("[name='EmployeeId1']");
            let $employeeId2 = $("#AddSchedule").find("[name='EmployeeId2']");
            let $employeeId3 = $("#AddSchedule").find("[name='EmployeeId3']");
            $employeeId1.empty();
            $employeeId2.empty();
            $employeeId3.empty();
            $employeeId1.append('<option value="0"></option>');
            $employeeId2.append('<option value="0"></option>');
            $employeeId3.append('<option value="0"></option>');
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    // Therapist 1
                    if (employeeId1 == data[i].employeeId) {
                        $employeeId1.append('<option selected="selected" value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    }
                    else {
                        $employeeId1.append('<option value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    }

                    // Therapist 2
                    if (employeeId2 == data[i].employeeId) {
                        $employeeId2.append('<option selected="selected" value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    }
                    else {
                        $employeeId2.append('<option value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    }

                    // Therapist 3
                    if (employeeId3 == data[i].employeeId) {
                        $employeeId3.append('<option selected="selected" value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    }
                    else {
                        $employeeId3.append('<option value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
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
            $resourceId.append('<option value="0"></option>');
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

function GetTaskByTaskId(currCtrl) {
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



function initFlatPickerDuration() {
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

function SetDates(StartDate, EndDate, opt) {

    let guestcheckInDateTime = moment($("#guestCheckinDate").val(), "YYYY-MM-DD hh:mm");
    let guestcheckOutDateTime = moment($("#guestCheckoutDate").val(), "YYYY-MM-DD hh:mm");
    let selectedDate;
    if (opt == 'AddEvent') {
        selectedDate = getCurrentDateTime(guestcheckInDateTime);
    }
    else if (opt == 'DateClick') {
        selectedDate = moment(StartDate, "YYYY-MM-DD");
        if (isDateWithinRange(selectedDate, guestcheckInDateTime, guestcheckOutDateTime)) {
            selectedDate.set({ hour: 14, minute: 0 }); // Set time to 2 PM (14:00)
        } else {
            selectedDate = guestcheckInDateTime; // Use guest check-in date
        }
    }
    $("#AddSchedule").find("[name='StartDate']").val(selectedDate.format("DD-MMM-YY"));
    $("#AddSchedule").find("[name='StartTime']").val(selectedDate.format("HH:mm"));

    let differenceInDays = guestcheckOutDateTime.diff(selectedDate, 'days');
    if (isNaN(differenceInDays)) {
        differenceInDays = 1;
    }
    else {
        differenceInDays = parseInt(differenceInDays) + 1;
    }
    $("#AddSchedule").find("[name='NoOfDays']").attr("max", differenceInDays);



}
function getCurrentDateTime(selectedDateTime) {
    let currentDateTime = moment(); // Get current date & time
    if (selectedDateTime.isBefore(currentDateTime)) {
        selectedDateTime = currentDateTime;
    }
    //return selectedDateTime.format("YYYY-MM-DD HH:mm"); // Return future date-time
    return selectedDateTime
}
function isDateWithinRange(startDate, checkInDate, checkOutDate) {
    return moment(startDate, "YYYY-MM-DD").isBetween(checkInDate, checkOutDate, undefined, '[]');
}