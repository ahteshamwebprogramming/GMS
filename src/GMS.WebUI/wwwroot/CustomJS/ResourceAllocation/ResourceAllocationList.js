let editDurationPicker;
var isInitializingEditSchedule = false;

$(document).ready(function () {
    ListPartialView();
    initEditScheduleModal();
});

function initEditScheduleModal() {
    // Initialize flatpickr for duration picker
    if ($('#EditDuration').length > 0) {
        editDurationPicker = flatpickr("#EditDuration", {
            enableTime: true,
            noCalendar: true,
            time_24hr: true,
            defaultHour: 0,
            defaultMinute: 0,
            dateFormat: "H:i",
            onChange: function (selectedDates, dateStr) {
                $("#EditSelectedDuration").val(dateStr);
                CalculateEndTimeForEdit();
            }
        });

        // Initialize date picker for Start Date
        flatpickr("#EditStartDate", {
            enableTime: false,
            noCalendar: false,
            dateFormat: "d-M-y",
            onChange: function (selectedDates, dateStr) {
                CalculateEndTimeForEdit();
            }
        });
    }
}

// Initialize Start Time change handler (using native HTML5 time input like reference)
function initEditStartTimeHandler() {
    if ($("#EditStartTime").length > 0) {
        $("#EditStartTime").off('change').on('change', function () {
            CalculateEndTimeForEdit();
        });
    }
}

function ListPartialView() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/ResourceAllocation/ListPartialView',
        //data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ListPartial').html(data);
            $("table").DataTable();
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}



function AddPartialView(Id = 0) {

    let inputDTO = {};
    inputDTO.Id = Id;

    BlockUI();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/PackageMaster/AddPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_AddPartial').html(data);
            $("#btnOpenAddServicesPopup").click();
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}


function isValidForm(formId) {

    let res = isValidateForm(formId);

    return res;
}
function AddServices() {

    if (!isValidForm("AddServices")) {
        return;
    }

    let AddForm = $("#AddServices").find("[dbCol]");

    let dataVM = new FormData();

    AddForm.each((i, v) => {
        let currObj = $(v);
        let value = currObj.val();
        let id = currObj.attr("id");
        let name = currObj.attr("name");
        if (value != null && value != undefined && value !== "") {
            //value = value.toUpperCase();
        }

        if (currObj.hasClass('contactno') && value != "") {
            value = value.replace(/\s/g, "");
        }
        if (currObj.hasClass('dateonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let currentDateFormat = "DD-MM-YYYY";
                let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD";
                //value = moment(moment(value).format("DD-MM-YYYY")).format(dateformat);
                value = moment(value, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                value = "";
            }
        }
        if (currObj.hasClass('datetimeonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let currentDateFormat = "DD-MM-YYYY HH:mm";
                let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD HH:mm";
                value = moment(value, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                value = "";
            }
        }
        if (currObj.hasClass('timeonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let currentDateFormat = "HH:mm";
                let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "HH:mm";
                value = moment(value, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                value = "";
            }
        }

        //if (name == "MemberDetail.DateOfDepartment") {
        //    value += " " + $("[name='TimeOfDepartment']").val();
        //}

        dataVM.append(currObj.attr("name"), value);
    });

    BlockUI();
    $.ajax({
        url: '/PackageMaster/Save',
        data: dataVM,
        //dataType: "json",
        async: false,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (response) {
            $("#addServicesPopup").find(".btn-close").click();
            if (response != null) {
                Swal.fire({ title: '', text: "Added Successfully!", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                    if (result.isConfirmed) {
                        ListPartialView();
                    }
                });
            }

            UnblockUI();
        },
        error: function (xhr, ajaxOptions, error) {
            UnblockUI();
            $erroralert("Transaction Failed!", xhr.responseText + '!');

            //responseText
        }
    });

}




function DeleteList(Id) {
    Swal.fire({
        title: 'Are you sure?'
        , text: "This will get deleted permanently!"
        , icon: 'warning'
        , showCancelButton: true
        , confirmButtonText: 'Yes, delete it!'
        , customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }
        , buttonsStyling: false
    }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "Id": Id
            };
            $.ajax({
                type: "POST",
                url: "/PackageMaster/Delete",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Deleted Successful!");
                    ListPartialView();
                    UnblockUI();
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}

function EditSchedule(scheduleId) {
    BlockUI();
    isInitializingEditSchedule = true; // Set flag to prevent CalculateEndTimeForEdit from running
    
    $.ajax({
        type: "GET",
        url: '/ResourceAllocation/GetScheduleById',
        data: { Id: scheduleId },
        success: function (schedule) {
            UnblockUI();
            if (schedule) {
                // Populate form with schedule data
                $("#EditScheduleId").val(schedule.id || schedule.Id);
                
                // Set Guest Name and Room No (read-only text fields)
                // Handle both camelCase and PascalCase property names
                let guestName = schedule.guestName || schedule.GuestName || "-";
                let roomNo = schedule.roomNo || schedule.RoomNo || null;
                $("#EditGuestName").text(guestName);
                let roomNoText = roomNo ? "Room " + roomNo : "-";
                $("#EditRoomNo").text(roomNoText);
                
                // Set dates and times
                let startDateTime = moment(schedule.startDateTime || schedule.StartDateTime);
                let endDateTime = moment(schedule.endDateTime || schedule.EndDateTime);
                
                // Set flatpickr date values - use setDate with Date object for better compatibility
                let startDatePicker = flatpickr("#EditStartDate");
                if (startDatePicker) {
                    startDatePicker.setDate(startDateTime.toDate(), false);
                } else {
                    // Format date for flatpickr (d-M-y format - matches flatpickr config)
                    $("#EditStartDate").val(startDateTime.format("D-MMM-YY"));
                }
                
                // Set Start Time value in HH:mm format (native time input format)
                let timeStr = startDateTime.format("HH:mm");
                $("#EditStartTime").val(timeStr);
                
                // Set duration - handle different duration formats
                let durationStr = "";
                if (typeof schedule.duration === 'string') {
                    // Parse string format like "01:30:00" or "01:30"
                    let durationParts = schedule.duration.split(":");
                    durationStr = durationParts[0] + ":" + durationParts[1];
                } else if (schedule.duration && typeof schedule.duration === 'object') {
                    // If it's a TimeSpan object with hours/minutes properties
                    let hours = schedule.duration.hours || 0;
                    let minutes = schedule.duration.minutes || 0;
                    durationStr = String(hours).padStart(2, '0') + ":" + String(minutes).padStart(2, '0');
                } else {
                    // Calculate from start and end times
                    let startMoment = moment(schedule.startDateTime || schedule.StartDateTime);
                    let endMoment = moment(schedule.endDateTime || schedule.EndDateTime);
                    let duration = moment.duration(endMoment.diff(startMoment));
                    durationStr = String(duration.hours()).padStart(2, '0') + ":" + String(duration.minutes()).padStart(2, '0');
                }
                
                // Set duration WITHOUT triggering onChange to prevent CalculateEndTimeForEdit from clearing end date/time
                if (editDurationPicker) {
                    editDurationPicker.setDate(durationStr, false);
                }
                $("#EditSelectedDuration").val(durationStr);
                $("#EditDuration").val(durationStr);
                
                // Set end date and time AFTER duration is set to ensure they don't get cleared
                // Format to match reference implementation (DD-MMM-YYYY)
                $("#EditEndDate").val(endDateTime.format("DD-MMM-YYYY"));
                $("#EditEndTime").val(endDateTime.format("HH:mm")); // Native time input uses HH:mm format
                
                // Initialize Start Time change handler
                initEditStartTimeHandler();
                
                // Load tasks and set selected task
                loadTasksForEdit(schedule.taskId || schedule.TaskId, schedule.employeeId1 || schedule.EmployeeId1, schedule.employeeId2 || schedule.EmployeeId2, schedule.employeeId3 || schedule.EmployeeId3, schedule.resourceId || schedule.ResourceId);
                
                // Reset flag after a short delay to allow all initialization to complete
                setTimeout(function() {
                    isInitializingEditSchedule = false;
                }, 100);
            }
        },
        error: function (error) {
            UnblockUI();
            isInitializingEditSchedule = false;
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

function loadTasksForEdit(taskId, employeeId1, employeeId2, employeeId3, resourceId) {
    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/GetTaskName",
        contentType: 'application/json',
        success: function (tasks) {
            let $taskSelect = $("#EditTaskId");
            $taskSelect.empty();
            $taskSelect.append('<option value="0"></option>');
            if (tasks != null && tasks.length > 0) {
                for (var i = 0; i < tasks.length; i++) {
                    let selected = tasks[i].id == taskId ? 'selected="selected"' : '';
                    $taskSelect.append('<option department="' + tasks[i].department + '" value="' + tasks[i].id + '" ' + selected + '>' + tasks[i].taskName + '</option>');
                }
            }
            
            // Set employee IDs as attributes for GetEmployeeByTaskIdForEdit
            $("#EditTaskId").attr("employeeid1", employeeId1 || 0);
            $("#EditTaskId").attr("employeeid2", employeeId2 || 0);
            $("#EditTaskId").attr("employeeid3", employeeId3 || 0);
            $("#EditTaskId").attr("resourceid", resourceId || 0);
            
            // Load employees and resources
            if (taskId > 0) {
                GetEmployeeByTaskIdForEdit($("#EditTaskId")[0]);
            }
            
            // Show modal
            $('#editScheduleModal').modal('show');
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

function GetEmployeeByTaskIdForEdit(currCtrl) {
    GetResourcesByTaskIdForEdit(currCtrl);
    GetTaskByTaskIdForEdit(currCtrl);
    
    let Id = $(currCtrl).val();
    let employeeId1 = $(currCtrl).attr("employeeid1");
    let employeeId2 = $(currCtrl).attr("employeeid2");
    let employeeId3 = $(currCtrl).attr("employeeid3");
    
    var inputDTO = {
        "Id": Id
    };
    
    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/GetEmployeeByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            let $employeeId1 = $("#EditEmployeeId1");
            let $employeeId2 = $("#EditEmployeeId2");
            let $employeeId3 = $("#EditEmployeeId3");
            
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
                    } else {
                        $employeeId1.append('<option value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    }
                    
                    // Therapist 2
                    if (employeeId2 == data[i].employeeId) {
                        $employeeId2.append('<option selected="selected" value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    } else {
                        $employeeId2.append('<option value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    }
                    
                    // Therapist 3
                    if (employeeId3 == data[i].employeeId) {
                        $employeeId3.append('<option selected="selected" value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    } else {
                        $employeeId3.append('<option value="' + data[i].employeeId + '">' + data[i].employeeName + '</option>');
                    }
                }
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

function GetResourcesByTaskIdForEdit(currCtrl) {
    let Id = $(currCtrl).val();
    var inputDTO = {
        "Id": Id
    };
    let resourceId = $(currCtrl).attr("resourceid");
    
    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/GetResourcesByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            let $resourceId = $("#EditResourceId");
            $resourceId.empty();
            $resourceId.append('<option value="0"></option>');
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (resourceId == data[i].id) {
                        $resourceId.append('<option selected="selected" value="' + data[i].id + '">' + data[i].resourceName + '</option>');
                    } else {
                        $resourceId.append('<option value="' + data[i].id + '">' + data[i].resourceName + '</option>');
                    }
                }
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

function GetTaskByTaskIdForEdit(currCtrl) {
    let Id = $(currCtrl).val();
    var inputDTO = {
        "Id": Id
    };
    
    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/GetTaskByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            if (data != null && data.duration) {
                let durationStr = "";
                if (typeof data.duration === 'string') {
                    let durationParts = data.duration.split(":");
                    durationStr = durationParts[0] + ":" + durationParts[1];
                } else {
                    // If it's a TimeSpan object
                    let hours = data.duration.hours || 0;
                    let minutes = data.duration.minutes || 0;
                    durationStr = String(hours).padStart(2, '0') + ":" + String(minutes).padStart(2, '0');
                }
                if (editDurationPicker) {
                    editDurationPicker.setDate(durationStr, false);
                }
                $("#EditSelectedDuration").val(durationStr);
                CalculateEndTimeForEdit();
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

function CalculateEndTimeForEdit() {
    // Don't calculate during initialization to prevent clearing end date/time
    if (isInitializingEditSchedule) {
        return;
    }
    
    let startDate = $("#EditStartDate").val();
    let startTime = $("#EditStartTime").val();
    let duration = $("#EditDuration").val();
    
    // Validate inputs - match reference implementation
    if (!startDate || !startTime || !duration) {
        $("#EditEndDate").val("");
        $("#EditEndTime").val("");
        return;
    }
    
    // Parse inputs - match reference implementation exactly
    // Convert flatpickr date format (d-M-y) to DD-MMM-YYYY format for moment parsing
    let startDateParsed = moment(startDate, ["D-MMM-YY", "D-MMM-YYYY", "DD-MMM-YYYY"], true);
    if (!startDateParsed.isValid()) {
        $("#EditEndDate").val("");
        $("#EditEndTime").val("");
        return;
    }
    
    // Format date to DD-MMM-YYYY format (matching reference)
    let startDateFormatted = startDateParsed.format("DD-MMM-YYYY");
    
    // Combine date and time - match reference implementation exactly
    // Reference uses: moment(`${startDate} ${startTime}`, "DD-MMM-YYYY HH:mm")
    let startDateTime = moment(`${startDateFormatted} ${startTime}`, "DD-MMM-YYYY HH:mm");
    
    if (!startDateTime.isValid()) {
        $("#EditEndDate").val("");
        $("#EditEndTime").val("");
        return;
    }
    
    // Parse duration - match reference implementation
    let durationParts = duration.split(":").map(Number); // Split duration into [hours, minutes]
    
    // Calculate End DateTime - match reference implementation exactly
    let endDateTime = startDateTime
        .clone()
        .add(durationParts[0], "hours")  // Add hours from duration
        .add(durationParts[1], "minutes") // Add minutes from duration
        .subtract(1, "seconds"); // Subtract 1 second
    
    // Update End Date and End Time fields - match reference implementation
    $("#EditEndDate").val(endDateTime.format("DD-MMM-YYYY")); // Set End Date
    $("#EditEndTime").val(endDateTime.format("HH:mm")); // Set End Time
}

function UpdateSchedule() {
    let startDate = $("#EditStartDate").val();
    let startTime = $("#EditStartTime").val();
    let endDate = $("#EditEndDate").val();
    let endTime = $("#EditEndTime").val();
    let duration = $("#EditDuration").val();
    
    if (!startDate || !startTime || !duration) {
        $erroralert("Validation Error!", "Please fill in all required fields!");
        return;
    }
    
    let inputDTO = {};
    inputDTO.Id = $("#EditScheduleId").val();
    inputDTO.TaskId = $("#EditTaskId").val();
    inputDTO.EmployeeId1 = $("#EditEmployeeId1").val() || null;
    inputDTO.EmployeeId2 = $("#EditEmployeeId2").val() || null;
    inputDTO.EmployeeId3 = $("#EditEmployeeId3").val() || null;
    inputDTO.ResourceId = $("#EditResourceId").val() || null;
    
    // Format StartDateTime - match the reference implementation format
    // Native time input uses HH:mm format (24-hour) - no conversion needed
    // Parse date - flatpickr outputs "d-M-y" format (e.g., "1-Nov-25" or "1-Nov-2025")
    if (!startTime || startTime === "") {
        $erroralert("Validation Error!", "Invalid start time format!");
        return;
    }
    
    // Parse date - handle both "d-M-y" (flatpickr output) and "D-MMM-YYYY" formats
    let startDateParsed = moment(startDate, ["D-MMM-YY", "D-MMM-YYYY", "DD-MMM-YYYY"], true);
    if (!startDateParsed.isValid()) {
        $erroralert("Validation Error!", "Invalid start date format!");
        return;
    }
    inputDTO.StartDateTime = startDateParsed.format("YYYY-MM-DD") + "T" + startTime;
    
    // Format EndDateTime from end date and time
    if (endDate && endTime) {
        // Native time input uses HH:mm format (24-hour) - no conversion needed
        if (!endTime || endTime === "") {
            $erroralert("Validation Error!", "Invalid end time format!");
            return;
        }
        // Parse end date - handle both formats
        let endDateParsed = moment(endDate, ["D-MMM-YY", "D-MMM-YYYY", "DD-MMM-YYYY"], true);
        if (!endDateParsed.isValid()) {
            $erroralert("Validation Error!", "Invalid end date format!");
            return;
        }
        inputDTO.EndDateTime = endDateParsed.format("YYYY-MM-DD") + "T" + endTime;
    } else {
        // Calculate from start + duration
        let durationParts = duration.split(":").map(Number);
        let startMoment = moment(startDateParsed.format("YYYY-MM-DD") + " " + startTime, "YYYY-MM-DD HH:mm");
        let endMoment = startMoment
            .clone()
            .add(durationParts[0], "hours")
            .add(durationParts[1], "minutes")
            .subtract(1, "seconds");
        inputDTO.EndDateTime = endMoment.format("YYYY-MM-DD") + "T" + endMoment.format("HH:mm");
    }
    
    // Format Duration as HH:mm:ss string
    let durationParts = duration.split(":");
    inputDTO.Duration = durationParts[0] + ":" + durationParts[1] + ":00";
    
    // Get GuestId from the schedule
    BlockUI();
    $.ajax({
        type: "GET",
        url: '/ResourceAllocation/GetScheduleById',
        data: { Id: inputDTO.Id },
        success: function (schedule) {
            // Handle both camelCase and PascalCase property names
            inputDTO.GuestId = schedule.guestId || schedule.GuestId;
            inputDTO.SessionId = schedule.sessionId || schedule.SessionId || null;
            
            // Now update the schedule
            $.ajax({
                type: 'POST',
                url: '/ResourceAllocation/UpdateSchedule',
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (response) {
                    UnblockUI();
                    Swal.fire({ title: '', text: "Schedule updated successfully!", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                        if (result.isConfirmed) {
                            $('#editScheduleModal').modal('hide');
                            ListPartialView();
                        }
                    });
                },
                error: function (error) {
                    UnblockUI();
                    $erroralert("Transaction Failed!", error.responseText + '!');
                }
            });
        },
        error: function (error) {
            UnblockUI();
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

function DeleteSchedule(scheduleId) {
    Swal.fire({
        title: 'Are you sure?',
        text: "This schedule will be deleted permanently!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' },
        buttonsStyling: false
    }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "Id": scheduleId
            };
            $.ajax({
                type: "POST",
                url: "/ResourceAllocation/DeleteSchedule",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    Swal.fire({ title: '', text: "Schedule deleted successfully!", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                        if (result.isConfirmed) {
                            ListPartialView();
                        }
                    });
                },
                error: function (error) {
                    UnblockUI();
                    $erroralert("Transaction Failed!", error.responseText + '!');
                }
            });
        }
    });
}