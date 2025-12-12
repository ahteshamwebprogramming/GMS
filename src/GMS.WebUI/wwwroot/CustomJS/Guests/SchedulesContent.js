// Store calendar instance globally for popover handling
var globalCalendarInstance = null;

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');

    if (!calendarEl) {
        return; // Exit if calendar element doesn't exist
    }

    var calendar = new FullCalendar.Calendar(calendarEl, {
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'myCustomButton dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },
        views: {
            listWeek: { buttonText: 'Agenda' },
        },
        selectable: true,
        editable: true,
        nextDayThreshold: '00:00',
        navLinks: true,
        dayMaxEvents: true,
        eventTimeFormat: { hour: 'numeric', minute: '2-digit' },
        customButtons: {
            myCustomButton: {
                text: 'Add Event',
                click: function () {
                    $('#crtevents').modal('toggle');
                    initFlatPickerDuration();
                    initCreateEventModal();
                    SetDates(null, null, 'AddEvent');
                }
            }
        },
        select: function (info) {
            $('#crtevents').modal('toggle');
            initFlatPickerDuration();
            initCreateEventModal();
            SetDates(info.startStr, null, 'DateClick');
        },
        events: function (fetchInfo, successCallback, failureCallback) {
            jQuery.ajax({
                url: "/Guests/GetGuestsEventForCalender?GuestId=" + $("#GuestSchedule_GuestId").val(),
                type: "GET",
                success: function (res) {
                    JSON.stringify(res)
                    var events = new Array();
                    $.each(res, function (i, data) {
                        const stringDuration = moment.duration(data.duration);
                        let durationText;

                        if (stringDuration.hours() < 1) {
                            const minutes = stringDuration.minutes();
                            durationText = minutes + " minute" + (minutes !== 1 ? "s" : "");
                        } else {
                            const hours = stringDuration.hours();
                            durationText = hours + " hour" + (hours !== 1 ? "s" : "");
                        }
                        events.push(
                            {
                                id: data.id,
                                title: data.taskName + "(" + durationText + ")",
                                description: data.description,
                                start: data.startDateTime,
                                end: data.endDateTime,
                                color: data.colorCode,
                            }
                        );
                    });
                    successCallback(events);
                },
            });
        },
        eventDrop: function (event) {
            // Handle drag and drop if needed
        },
        eventClick: function (event) {
            var id = event.event._def.publicId;
            var inputDTO = {
                "Id": id
            };
            $.ajax({
                type: 'POST',
                url: '/Guests/GetGuestsEventForCalenderById',
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $('#crtevents').modal('toggle');
                    initFlatPickerDuration();

                    let tasks = data.tasks;
                    let $taskId = $("#AddSchedule").find("[name='TaskId']");
                    $taskId.empty();
                    $taskId.append('<option value="0">Select Task</option>');
                    if (tasks != null && tasks.length > 0) {
                        for (var i = 0; i < tasks.length; i++) {
                            $taskId.append('<option department="' + tasks[i].department + '" value="' + tasks[i].id + '">' + tasks[i].taskName + '</option>');
                        }
                    }

                    let guestSchedule = data.guestSchedule;
                    $taskId.attr("employeeid1", guestSchedule.employeeId1)
                    $taskId.attr("employeeid2", guestSchedule.employeeId2)
                    $taskId.attr("resourceid", guestSchedule.resourceId)
                    $("#AddSchedule").find("[name='TaskId']").val(guestSchedule.taskId);

                    let guestId = guestSchedule.guestId;
                    let taskName = guestSchedule.taskName;
                    let [startDateT, startTimeT] = guestSchedule.startDateTime.split("T");
                    let startDate = moment(startDateT, "YYYY-MM-DD").format("DD-MMM-YYYY");
                    let startTime = startTimeT;

                    let [endDateT, endTimeT] = guestSchedule.endDateTime.split("T");
                    let endDate = moment(endDateT, "YYYY-MM-DD").format("DD-MMM-YYYY");
                    let endTime = endTimeT;

                    let duration = moment(guestSchedule.duration, "HH:mm").format("HH:mm");
                    $taskId.attr("duration", duration)

                    $("#AddSchedule").find("[name='TaskName']").val(taskName);
                    $("#AddSchedule").find("[name='StartDate']").val(startDate);
                    $("#AddSchedule").find("[name='StartTime']").val(startTime);
                    $("#AddSchedule").find("[name='EndDate']").val(endDate);
                    $("#AddSchedule").find("[name='EndTime']").val(endTime);
                    $("#AddSchedule").find("[name='ScheduleId']").val(guestSchedule.id);

                    $taskId.change();
                },
                error: function (error) {
                    console.log('Failed');
                }
            });
        },
    });
    
    // Store calendar instance globally
    globalCalendarInstance = calendar;
    
    calendar.render();
});

