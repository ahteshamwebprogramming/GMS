let editDurationPicker;
let isInitializingEditSchedule = false;

$(document).ready(function () {
    ListPartialView();
    initEditScheduleModal();
});

/* ----------------------------------------------------------
   INIT PICKERS
---------------------------------------------------------- */

function initEditScheduleModal() {

    if (!$('#EditDuration').length) return;

    // Duration Picker (HH:mm) - 5-minute intervals
    editDurationPicker = flatpickr("#EditDuration", {
        enableTime: true,
        noCalendar: true,
        time_24hr: true,
        minuteIncrement: 5, // Enforce 5-minute intervals
        dateFormat: "H:i",
        onChange: CalculateEndTimeForEdit
    });

    // Start Date Picker
    flatpickr("#EditStartDate", {
        altInput: true,
        altFormat: "d-M-Y",   // UI → 01-Nov-2025
        dateFormat: "Y-m-d",  // Internal → 2025-11-01
        allowInput: false,
        onChange: CalculateEndTimeForEdit
    });

    initEditStartTimeHandler();
}

function initEditStartTimeHandler() {
    $("#EditStartTime")
        .off("change")
        .on("change", CalculateEndTimeForEdit);
}

/* ----------------------------------------------------------
   END DATE CALCULATION (SINGLE SOURCE OF TRUTH)
---------------------------------------------------------- */

function CalculateEndTimeForEdit() {

    if (isInitializingEditSchedule) return;

    const startPicker = document.getElementById("EditStartDate")?._flatpickr;
    if (!startPicker || !editDurationPicker) return;

    const startDate = startPicker.selectedDates[0];
    const startTime = $("#EditStartTime").val();
    const durationDate = editDurationPicker.selectedDates[0];

    if (!startDate || !startTime || !durationDate) {
        clearEndDateTime();
        return;
    }

    const [h, m] = startTime.split(":").map(Number);
    const [dh, dm] = editDurationPicker
        .formatDate(durationDate, "H:i")
        .split(":")
        .map(Number);

    const startDateTime = moment(startDate)
        .set({ hour: h, minute: m, second: 0 });

    if (!startDateTime.isValid()) {
        clearEndDateTime();
        return;
    }

    const endDateTime = startDateTime
        .clone()
        .add(dh, "hours")
        .add(dm, "minutes")
        .subtract(1, "seconds");

    $("#EditEndDate").val(endDateTime.format("DD-MMM-YYYY"));
    $("#EditEndTime").val(endDateTime.format("HH:mm"));
}

function clearEndDateTime() {
    $("#EditEndDate").val("");
    $("#EditEndTime").val("");
}

/**
 * Rounds duration minutes to the nearest 5-minute interval
 * @param {string} durationStr - Duration in "HH:mm" format
 * @returns {string} Duration rounded to nearest 5 minutes in "HH:mm" format
 */
function roundDurationToNearest5(durationStr) {
    if (!durationStr || typeof durationStr !== 'string') return durationStr;
    
    const parts = durationStr.split(":");
    if (parts.length !== 2) return durationStr;
    
    const hours = parseInt(parts[0], 10) || 0;
    let minutes = parseInt(parts[1], 10) || 0;
    
    // Round minutes to nearest 5
    minutes = Math.round(minutes / 5) * 5;
    
    // Handle overflow (e.g., 60 minutes becomes 1 hour)
    if (minutes >= 60) {
        hours += Math.floor(minutes / 60);
        minutes = minutes % 60;
    }
    
    return `${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}`;
}

/* ----------------------------------------------------------
   LIST
---------------------------------------------------------- */

function ListPartialView() {
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/ListPartialView",
        dataType: "html",
        success: function (html) {
            UnblockUI();
            $("#div_ListPartial").html(html);
            $("table").DataTable({
                order: [] // Disable default sorting on first column
            });
        },
        error: function (err) {
            UnblockUI();
            $erroralert("Transaction Failed!", err.responseText);
        }
    });
}

/* ----------------------------------------------------------
   EDIT SCHEDULE
---------------------------------------------------------- */

function EditSchedule(scheduleId) {

    BlockUI();
    isInitializingEditSchedule = true;

    $.get("/ResourceAllocation/GetScheduleById", { Id: scheduleId })
        .done(function (schedule) {

            UnblockUI();
            if (!schedule) return;

            const startDT = moment(schedule.startDateTime || schedule.StartDateTime);
            const endDT = moment(schedule.endDateTime || schedule.EndDateTime);

            $("#EditScheduleId").val(schedule.id || schedule.Id);
            $("#EditGuestName").text(schedule.guestName || schedule.GuestName || "-");
            $("#EditRoomNo").text(schedule.roomNo ? `Room ${schedule.roomNo}` : "-");

            // Set Start Date
            const startPicker = document.getElementById("EditStartDate")?._flatpickr;
            startPicker?.setDate(startDT.toDate(), false);

            // Start Time
            $("#EditStartTime").val(startDT.format("HH:mm"));

            // Duration - round to nearest 5 minutes
            const duration = moment.duration(endDT.diff(startDT));
            const durationStr = `${String(duration.hours()).padStart(2, "0")}:${String(duration.minutes()).padStart(2, "0")}`;
            const roundedDuration = roundDurationToNearest5(durationStr);
            editDurationPicker?.setDate(roundedDuration, false);
            
            // Store original task ID and duration for GetTaskByTaskIdForEdit
            const originalTaskId = schedule.taskId || schedule.TaskId;
            const $task = $("#EditTaskId");
            $task.attr("originalTaskId", originalTaskId);
            $task.attr("originalDuration", roundedDuration);

            // End Date/Time
            $("#EditEndDate").val(endDT.format("DD-MMM-YYYY"));
            $("#EditEndTime").val(endDT.format("HH:mm"));

            loadTasksForEdit(
                schedule.taskId || schedule.TaskId,
                schedule.employeeId1 || schedule.EmployeeId1 || null,
                schedule.employeeId2 || schedule.EmployeeId2 || null,
                schedule.employeeId3 || schedule.EmployeeId3 || null,
                schedule.resourceId || schedule.ResourceId || null,
                endDT
            );
        })
        .fail(function (err) {
            UnblockUI();
            isInitializingEditSchedule = false;
            $erroralert("Transaction Failed!", err.responseText);
        });
}

/* ----------------------------------------------------------
   TASK / EMPLOYEE / RESOURCE LOADERS
---------------------------------------------------------- */

function loadTasksForEdit(taskId, emp1, emp2, emp3, resourceId, endDT) {

    $.post("/ResourceAllocation/GetTaskName")
        .done(function (tasks) {

            const $task = $("#EditTaskId");
            
            // Preserve original task ID and duration attributes before emptying
            const originalTaskId = $task.attr("originalTaskId") || taskId || 0;
            const originalDuration = $task.attr("originalDuration") || "";
            
            $task.empty().append(`<option value="0"></option>`);

            tasks?.forEach(t =>
                $task.append(`<option value="${t.id}" ${t.id === taskId ? "selected" : ""}>${t.taskName}</option>`)
            );

            // Restore attributes after rebuilding dropdown
            $task.attr({
                employeeid1: emp1 != null ? emp1 : "",
                employeeid2: emp2 != null ? emp2 : "",
                employeeid3: emp3 != null ? emp3 : "",
                resourceid: resourceId != null ? resourceId : "",
                originalTaskId: originalTaskId,
                originalDuration: originalDuration
            });

            if (taskId) GetEmployeeByTaskIdForEdit($task[0]);

            $("#editScheduleModal").modal("show");

            setTimeout(() => {
                $("#EditEndDate").val(endDT.format("DD-MMM-YYYY"));
                $("#EditEndTime").val(endDT.format("HH:mm"));
                isInitializingEditSchedule = false;
            }, 300);
        });
}

function GetEmployeeByTaskIdForEdit(ctrl) {

    GetResourcesByTaskIdForEdit(ctrl);
    GetTaskByTaskIdForEdit(ctrl);

    const id = $(ctrl).val();
    const employeeId1 = $(ctrl).attr("employeeid1");
    const employeeId2 = $(ctrl).attr("employeeid2");
    const employeeId3 = $(ctrl).attr("employeeid3");
    const input = { Id: id };

    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/GetEmployeeByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(input),
        success: function (data) {
            // Populate Therapist 1 dropdown
            const $employeeId1 = $("#EditEmployeeId1").empty().append(`<option value="0"></option>`);
            if (data != null && data.length > 0) {
                data.forEach(e => {
                    // Check if this employee matches the stored employeeId1
                    // Handle both string and number comparisons
                    if (employeeId1 && (employeeId1 == e.employeeId || String(employeeId1) === String(e.employeeId))) {
                        $employeeId1.append(`<option selected="selected" value="${e.employeeId}">${e.employeeName}</option>`);
                    } else {
                        $employeeId1.append(`<option value="${e.employeeId}">${e.employeeName}</option>`);
                    }
                });
            }

            // Populate Therapist 2 dropdown
            const $employeeId2 = $("#EditEmployeeId2").empty().append(`<option value="0"></option>`);
            if (data != null && data.length > 0) {
                data.forEach(e => {
                    // Check if this employee matches the stored employeeId2
                    // Handle both string and number comparisons
                    if (employeeId2 && (employeeId2 == e.employeeId || String(employeeId2) === String(e.employeeId))) {
                        $employeeId2.append(`<option selected="selected" value="${e.employeeId}">${e.employeeName}</option>`);
                    } else {
                        $employeeId2.append(`<option value="${e.employeeId}">${e.employeeName}</option>`);
                    }
                });
            }

            // Populate Therapist 3 dropdown
            const $employeeId3 = $("#EditEmployeeId3").empty().append(`<option value="0"></option>`);
            if (data != null && data.length > 0) {
                data.forEach(e => {
                    // Check if this employee matches the stored employeeId3
                    // Handle both string and number comparisons
                    if (employeeId3 && (employeeId3 == e.employeeId || String(employeeId3) === String(e.employeeId))) {
                        $employeeId3.append(`<option selected="selected" value="${e.employeeId}">${e.employeeName}</option>`);
                    } else {
                        $employeeId3.append(`<option value="${e.employeeId}">${e.employeeName}</option>`);
                    }
                });
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

function GetResourcesByTaskIdForEdit(ctrl) {

    const input = { Id: $(ctrl).val() };
    const selected = $(ctrl).attr("resourceid");

    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/GetResourcesByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(input),
        success: function (data) {
            const $res = $("#EditResourceId").empty().append(`<option value="0"></option>`);
            data?.forEach(r =>
                $res.append(`<option value="${r.id}" ${r.id == selected ? "selected" : ""}>${r.resourceName}</option>`)
            );
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

function GetTaskByTaskIdForEdit(ctrl) {

    const currentTaskId = $(ctrl).val();
    const originalTaskId = $(ctrl).attr("originalTaskId");
    const originalDuration = $(ctrl).attr("originalDuration");
    
    if (!currentTaskId || currentTaskId === "0") return;

    const input = { Id: currentTaskId };

    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/GetTaskByTaskId",
        contentType: 'application/json',
        data: JSON.stringify(input),
        success: function (data) {

            if (!data) return;

            let durationToSet = null;

            // Only use original duration if we're still on the original task
            // Otherwise, use the new task's default duration
            // Convert to string for comparison since .val() returns string
            if (String(currentTaskId) === String(originalTaskId) && originalDuration != null && originalDuration !== undefined && originalDuration !== "") {
                durationToSet = originalDuration;
            }
            // Use the task's default duration for new/changed tasks
            else if (data.duration != null) {
                // Handle different duration formats (string or TimeSpan object)
                if (typeof data.duration === 'string') {
                    // Parse string format like "01:30:00" or "01:30"
                    const durationParts = data.duration.split(":");
                    durationToSet = durationParts[0] + ":" + durationParts[1];
                } else if (typeof data.duration === 'object') {
                    // If it's a TimeSpan object with hours/minutes properties
                    const hours = data.duration.hours || 0;
                    const minutes = data.duration.minutes || 0;
                    durationToSet = `${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}`;
                }
            }

            // Set duration if we have a value
            if (durationToSet) {
                // Round to nearest 5 minutes
                const roundedDuration = roundDurationToNearest5(durationToSet);
                
                // Set duration using flatpickr (without triggering onChange to avoid recursion)
                if (editDurationPicker) {
                    editDurationPicker.setDate(roundedDuration, false);
                }
                
                // Trigger EndDate recalculation
                CalculateEndTimeForEdit();
            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });
}

/* ----------------------------------------------------------
   UPDATE & DELETE
---------------------------------------------------------- */

function UpdateSchedule() {

    const startPicker = document.getElementById("EditStartDate")?._flatpickr;
    const startDate = startPicker?.selectedDates[0];
    const startTime = $("#EditStartTime").val();

    if (!startDate || !startTime) {
        $erroralert("Validation Error", "Invalid start date or time");
        return;
    }

    const dto = {
        Id: $("#EditScheduleId").val(),
        TaskId: $("#EditTaskId").val(),
        EmployeeId1: $("#EditEmployeeId1").val() || null,
        EmployeeId2: $("#EditEmployeeId2").val() || null,
        EmployeeId3: $("#EditEmployeeId3").val() || null,
        ResourceId: $("#EditResourceId").val() || null,
        StartDateTime: moment(startDate).format("YYYY-MM-DD") + "T" + startTime,
        EndDateTime: moment($("#EditEndDate").val(), "DD-MMM-YYYY").format("YYYY-MM-DD") + "T" + $("#EditEndTime").val(),
        Duration: $("#EditDuration").val() + ":00"
    };

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/ResourceAllocation/UpdateSchedule",
        contentType: "application/json",
        data: JSON.stringify(dto),
        success: function () {
            UnblockUI();
            $("#editScheduleModal").modal("hide");
            ListPartialView();
        },
        error: function (e) {
            UnblockUI();
            $erroralert("Transaction Failed!", e.responseText);
        }
    });
}

function DeleteSchedule(id) {

    Swal.fire({
        title: "Are you sure?",
        text: "This schedule will be deleted permanently!",
        icon: "warning",
        showCancelButton: true
    }).then(res => {

        if (!res.value) return;

        BlockUI();
        $.post("/ResourceAllocation/DeleteSchedule", JSON.stringify({ Id: id }), function () {
            UnblockUI();
            ListPartialView();
        });
    });
}
