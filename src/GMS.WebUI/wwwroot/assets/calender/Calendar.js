$(function () {                                                            //DateRangePicker for Create Event
    $('[name = "daterange"]').daterangepicker({
        opens: 'left'
    }, function (start, end, label) {
        var checkbox = document.getElementById('thisrepeat');
        if (checkbox.checked == true) {
            document.getElementById("startdate").value = start.format('MM/DD/YYYY');
        }
        else {
            document.getElementById("startdate").value = start.format('MM/DD/YYYY');
            document.getElementById("enddate").value = end.format('MM/DD/YYYY');
        }
    });
});

$(function () {                                                            //DateRangePicker for Edit Event
    $('[name = "editdaterange"]').daterangepicker({
        opens: 'left'
    }, function (start, end, label) {
        var checkbox = document.getElementById('editthisrepeat');
        if (checkbox.checked == true) {
            document.getElementById("editeventstartdate").value = start.format('MM/DD/YYYY');
        }
        else {
            document.getElementById("editeventstartdate").value = start.format('MM/DD/YYYY');
            document.getElementById("editeventenddate").value = end.format('MM/DD/YYYY');
        }
    });
});

//$(function () {
//    $('.begintime').datetimepicker({
//        showSecond: true,
//        dateFormat: 'mm/dd/yy',
//        timeFormat: 'hh:mm',
//        stepHour: 1,
//        stepMinute: 15,
//    });
//});

function myFunction() {                                                   //This is used for checkbox of Allday in AddEvent to hide the time.
    var checkbox = document.getElementById("AllDay");
    if (checkbox.checked == true) {
        document.getElementById("begintime").value = "";
        document.getElementById("endtime").value = "";
        document.getElementById("begintime").disabled = true;
        document.getElementById("endtime").disabled = true;
    }
    else {
        document.getElementById("begintime").disabled = false;
        document.getElementById("endtime").disabled = false;
    }
}
function myeditFunction() {
    debugger;
    var checkbox = document.getElementById("editAllday");
    if (checkbox.checked == true) {
        document.getElementById("editeventstarttime").value = "";
        document.getElementById("editeventendtime").value = "";
        document.getElementById("editeventstarttime").disabled = true;
        document.getElementById("editeventendtime").disabled = true;
    }
    else {
        document.getElementById("editeventstarttime").disabled = false;
        document.getElementById("editeventendtime").disabled = false;
    }
}

function repeatevent() {
    var checkbox = document.getElementById('thisrepeat');

    if (checkbox.checked == true) {
        $('#eventreccdropdown').css("display", "block");
        document.getElementById("enddate").disabled = true;
        document.getElementById("enddate").value = "";
    }
    else {
        $('#eventreccdropdown').css("display", "none");
        $("#dailyview").css("display", "none");
        $("#weekview").css("display", "none");
        $("#monthview").css("display", "none");
        $("#yearview").css("display", "none");
        $("#weekdays").css("display", "none");
        $("#endson").css("display", "none");
        document.getElementById("enddate").disabled = false;
        document.getElementById("startdate").value = "";
        document.getElementById("enddate").value = "";
    }
}

function editrepeatevent() {
    var checkbox = document.getElementById('editthisrepeat');
    if (checkbox.checked == true) {
        $('#editeventreccdropdown').css("display", "block");
        document.getElementById("editeventenddate").disabled = true;
        document.getElementById("editeventenddate").value = "";
    }
    else {
        $('#editeventreccdropdown').css("display", "none");
        $("#editdailyview").css("display", "none");
        $("#editweekview").css("display", "none");
        $("#editmonthview").css("display", "none");
        $("#edityearview").css("display", "none");
        $("#weekdays").css("display", "none");
        $("#endson").css("display", "none");
        document.getElementById("editeventenddate").disabled = false;
        document.getElementById("editeventstartdate").value = "";
        document.getElementById("editeventenddate").value = "";
    }
}

var selectedRepetition, value;
function repeateventdrpdown(val) {
    debugger;
    value = val;
    var daily = document.getElementById("daily").value;
    var week = document.getElementById("weekly").value;
    var month = document.getElementById("monthly").value;
    var year = document.getElementById("yearly").value;

    if (val == daily) {
        selectedRepetition = "daily";
        $("#dailyview").css("display", "block");
        $("#endson").css("display", "block");
        $("#weekview").css("display", "none");
        $("#monthview").css("display", "none");
        $("#yearview").css("display", "none");
        $("#weekdays").css("display", "none");
        document.getElementById("enddate").disabled = true;
    }
    else if (val == week) {
        selectedRepetition = "weekly";
        $("#dailyview").css("display", "none");
        $("#weekview").css("display", "block");
        $("#endson").css("display", "block");
        $("#monthview").css("display", "none");
        $("#yearview").css("display", "none");
        $("#weekdays").css("display", "block");
        document.getElementById("enddate").disabled = true;
    }
    else if (val == month) {
        selectedRepetition = "monthly";
        $("#dailyview").css("display", "none");
        $("#weekview").css("display", "none");
        $("#weekdays").css("display", "none");
        $("#monthview").css("display", "block");
        $("#endson").css("display", "block");
        $("#yearview").css("display", "none");
        document.getElementById("enddate").disabled = true;
    }
    else if (val == year) {
        selectedRepetition = "Yearly";
        $("#dailyview").css("display", "none");
        $("#weekview").css("display", "none");
        $("#monthview").css("display", "none");
        $("#endson").css("display", "block");
        $("#yearview").css("display", "block");
        $("#weekdays").css("display", "none");
        document.getElementById("enddate").disabled = true;
    }
    else {
        $("#dailyview").css("display", "none");
        $("#weekview").css("display", "none");
        $("#weekdays").css("display", "none");
        $("#monthview").css("display", "none");
        $("#yearview").css("display", "none");
        $("#endson").css("display", "none");
        document.getElementById("enddate").disabled = true;
    }
}

function selectStatus(val) {
    debugger;
    var busy = document.getElementById("selectbusy").value;
    //var free = document.getElementById("selectfree").value;
    if (val == busy) {
        eventData.Status = "Busy";
        eventData.ColorCode = "#ff0000";
    }
    else {
        eventData.Status = "Free";
        eventData.ColorCode = "";
    }
}

function selectAccessibility(val) {
    debugger;
    var public = document.getElementById("selectpublic").value;
    // var private = document.getElementById("selectprivate").value;
    if (val == public) {
        eventData.Accessibility = "Public";
    }
    else {
        eventData.Accessibility = "Private";
    }
}

var Eventenddate;
function editrepeatevent() {
    debugger;
    Eventenddate = document.getElementById("editeventenddate").value;
    var checkbox = document.getElementById('editthisrepeat');

    if (checkbox.checked == true) {
        $('#editeventreccdropdown').css("display", "block");
        document.getElementById("editeventenddate").disabled = true;
        document.getElementById("editeventenddate").value = "";
    }
    else {
        $('#editeventreccdropdown').css("display", "none");
        $("#editdailyview").css("display", "none");
        $("#editweekview").css("display", "none");
        $("#editmonthview").css("display", "none");
        $("#edityearview").css("display", "none");
        $("#editweekdays").css("display", "none");
        $("#editendson").css("display", "none");
        document.getElementById("editeventenddate").disabled = false;
        document.getElementById("editeventstartdate").value = "";
        document.getElementById("editeventenddate").value = "";
    }
}

var selectedRepetition, value;
function editrepeateventdrpdown(val) {
    debugger;
    value = val;
    var daily = document.getElementById("editdaily").value;
    var week = document.getElementById("editweekly").value;
    var month = document.getElementById("editmonthly").value;
    var year = document.getElementById("edityearly").value;

    if (val == daily) {
        selectedRepetition = "daily";
        $("#editdailyview").css("display", "block");
        $("#editendson").css("display", "block");
        $("#editweekview").css("display", "none");
        $("#editmonthview").css("display", "none");
        $("#edityearview").css("display", "none");
        $("#editweekdays").css("display", "none");
        document.getElementById("editeventenddate").disabled = true;
    }
    else if (val == week) {
        selectedRepetition = "weekly";
        $("#editdailyview").css("display", "none");
        $("#editweekview").css("display", "block");
        $("#editendson").css("display", "block");
        $("#editmonthview").css("display", "none");
        $("#edityearview").css("display", "none");
        $("#editweekdays").css("display", "block");
        document.getElementById("editeventenddate").disabled = true;
    }
    else if (val == month) {
        selectedRepetition = "monthly";
        $("#editdailyview").css("display", "none");
        $("#editweekview").css("display", "none");
        $("#editweekdays").css("display", "none");
        $("#editmonthview").css("display", "block");
        $("#editendson").css("display", "block");
        $("#edityearview").css("display", "none");
        document.getElementById("editeventenddate").disabled = true;
    }
    else if (val == year) {
        selectedRepetition = "Yearly";
        $("#editdailyview").css("display", "none");
        $("#editweekview").css("display", "none");
        $("#editmonthview").css("display", "none");
        $("#editendson").css("display", "block");
        $("#edityearview").css("display", "block");
        $("#editweekdays").css("display", "none");
        document.getElementById("editeventenddate").disabled = true;
    }
    else {
        $("#editdailyview").css("display", "none");
        $("#editweekview").css("display", "none");
        $("#editweekdays").css("display", "none");
        $("#editmonthview").css("display", "none");
        $("#edityearview").css("display", "none");
        $("#editendson").css("display", "none");
        document.getElementById("editeventenddate").disabled = true;
    }
}

var eventData = {
    TaskName: "",
    StartDate: "",
    StartTime: "",
    Duration: "",
    EndDate: "",
    EndTime: ""
}

var recurrenceenddate;

function setproperty_New() {
    let eventData_New = {};
    eventData_New.TaskName = $("#AddSchedule").find("[name='TaskName']").val();
    eventData_New.StartDate = $("#AddSchedule").find("[name='StartDate']").val();
    eventData_New.StartTime = $("#AddSchedule").find("[name='StartTime']").val();
    eventData_New.Duration = $("#AddSchedule").find("[name='Duration']").val();
    eventData_New.EndDate = $("#AddSchedule").find("[name='EndDate']").val();
    eventData_New.EndTime = $("#AddSchedule").find("[name='EndTime']").val();
    eventData_New.GuestId = $("#GuestSchedule_GuestId").val();
    eventData_New.Id = $("#AddSchedule").find("[name='ScheduleId']").val();
    return eventData_New;
}
function setproperty1() {
    debugger;
    eventData.Title = $("#txtTitle").val();
    eventData.Description = $("#txtDescription").val();
    eventData.location = $("#txtlocation").val();

    if (eventData.Status == "Busy" || eventData.Status == "Free") {

    }
    else {
        eventData.Status = "Busy";
        eventData.ColorCode = "#ff0000";
    }

    if (eventData.Accessibility == "Public" || eventData.Accessibility == "Private") {

    }
    else {
        eventData.Accessibility = "Public";
    }

    var day = $("#daily").val();
    var week = $("#weekly").val();
    var month = $("#monthly").val();
    var year = $("#yearly").val();

    if (value == day) {
        eventData.Recurrenceinterval = $("#daynumber").val();
    }
    else if (value == week) {
        eventData.Recurrenceinterval = $("#weeknumber").val();
    }
    else if (value == month) {
        eventData.Recurrenceinterval = $("#monthnumber").val();
    }
    else if (value == year) {
        eventData.Recurrenceinterval = $("#yearnumber").val();
    }
    else {
        console.log("error");
    }

    var endchecked = document.getElementById("endcheck");
    //if (endchecked.checked == true) {
    //    recurrenceenddate = $("#reccenddate").val();
    //}

    var checkbox = document.getElementById("AllDay");
    if (checkbox.checked == true) {
        eventData.StartDate = $("#startdate").val() + " " + "00:00";
        eventData.EndDate = $("#enddate").val() + " " + "23:59";
    }
    var repeatcheck = document.getElementById("thisrepeat");
    if (repeatcheck.checked == true) {
        var begintime = document.getElementById('begintime').value;
        var endtime = document.getElementById('endtime').value;
        eventData.StartDate = $("#startdate").val() + " " + begintime;
        eventData.EndDate = $("#startdate").val() + " " + endtime;
    }
    if (checkbox.checked == true && repeatcheck.checked == true) {
        eventData.StartDate = $("#startdate").val() + " " + "00:00";
        eventData.EndDate = $("#startdate").val() + " " + "23:59";
    }
    if (checkbox.checked == false && repeatcheck.checked == false) {
        var begintime = document.getElementById('begintime').value;
        var endtime = document.getElementById('endtime').value;
        eventData.StartDate = $("#startdate").val() + " " + begintime;
        eventData.EndDate = $("#enddate").val() + " " + endtime;
    }
}

function isValidate() {                                //Validation of the textfields in the AddEvent
    debugger;
    var isValid = true;
    var title = $("#txtTitle").val();
    var description = $("#txtDescription").val();
    var StartDate = $("#startdate").val();
    var EndDate = $("#enddate").val();
    var Email = $("#reminderemail_1").val();
    if (title == null || title == '') {
        $("#valTitle").text("Please enter Title")
        isValid = false;
    }
    if (description == null || description == '') {
        $("#valDescription").text("Please Enter Description")
        isValid = false;
    }
    if (StartDate == null || StartDate == '') {
        $("#startdate").text("Please Select StartDate")
        isValid = false;
    }
    //if (Email == null || Email == '') {
    //    $("#reminderemail_1").text("Please Enter Valid Email Id")
    //    isValid = false;
    //}
    //if (IsEmail(Email) == false) {
    //    $("#reminderemail_1").text("Please enter Valid Email Id");
    //    return false;
    //}
    var checkbox = document.getElementById('thisrepeat');
    if (checkbox.checked) {
        isValid = true;
    }
    else {
        if (EndDate == null || EndDate == '') {
            $("#enddate").text("Please Select EndDate")
            isValid = false;
        }
    }
    return isValid;
}

function Guestids() {

    var guestIds = "";
    $('#guestUser .guestList').each(function () {
        var guestId = $(this).attr("id");
        guestIds += guestId + ",";
    });
    if (guestIds != "") {
        guestIds = guestIds.substring(0, guestIds.length - 1);
    }
    return guestIds;
}

function CreateEvent() {

    //document.getElementById("btnadd").disabled = true;
    var guestIds = Guestids();

    var emailReminderData = [];
    $('.reminderselection').each(function () {
        var emailId = $(this).attr("id");
        var email_split_id = emailId.split("_");

        var indexNo = email_split_id[1];
        var reminderemail = "#reminderemail_" + indexNo;
        var remindernumber = "#remindernumber_" + indexNo;
        var reminderduration = "#reminderduration_" + indexNo;

        var reminderemailValue = $(reminderemail).val();
        var remindernumberValue = $(remindernumber).val();
        var reminderdurationValue = $(reminderduration).val();

        if (reminderemailValue != "") {
            emailReminderData.push({
                email: reminderemailValue,
                number: remindernumberValue,
                duration: reminderdurationValue
            });
        }
    });

    var jsonemailRemindersString = JSON.stringify(emailReminderData);
    $("#hidemailreminders").val(jsonemailRemindersString);

    var dates = new Array();
    var test = new Array();
    var d = new Date();
    var date = d.getDate();
    d = d.getDay();
    $("input[name='programming']:checked").each(function () {
        test.push($(this).val());
    });
    for (var i = 0; i < test.length; i++) {
        if (d <= test[i]) {
            var t = test[i] - d;
            var d1 = new Date();
            d1.setDate(d1.getDate() + t);
            var d2 = d1.toLocaleDateString();
            dates.push(d2);
        }
        else {
            var t = 7 - d + Number(test[i]);
            var d1 = new Date();
            d1.setDate(d1.getDate() + t);
            var d2 = d1.toLocaleDateString();
            dates.push(d2);
        }
    }

    //pondFiles = pond.getFiles();
    var formdata = new FormData();
    //var uploadCount = pondFiles.length;
    //for (var i = 0; i < pondFiles.length; i++) {
    //    formdata.append('uploadFile', pondFiles[i].file);
    //}

    var checkbox = document.getElementById("thisrepeat");
    var count = 0;
    //if (checkbox.checked == true) {
    //    count = 1;
    //}

    //if (isValidate()) {
    if (true) {
        //let inputDTO = setproperty_New();
        let startDate = $("#AddSchedule").find("[name='StartDate']").val();
        let startTime = $("#AddSchedule").find("[name='StartTime']").val();
        let endDate = $("#AddSchedule").find("[name='EndDate']").val();
        let endTime = $("#AddSchedule").find("[name='EndTime']").val();
        let duration = $("#AddSchedule").find("[name='Duration']").val();

        let inputDTO = {};

        inputDTO.TaskId = $("#AddSchedule").find("[name='TaskId']").val();
        inputDTO.EmployeeId1 = $("#AddSchedule").find("[name='EmployeeId1']").val();
        inputDTO.EmployeeId2 = $("#AddSchedule").find("[name='EmployeeId2']").val();
        inputDTO.EmployeeId3 = $("#AddSchedule").find("[name='EmployeeId3']").val();
        inputDTO.ResourceId = $("#AddSchedule").find("[name='ResourceId']").val();

        // Format StartDateTime (Start Date + Start Time)
        //inputDTO.StartDateTime = moment(`${startDate} ${startTime}`, "DD-MMM-YYYY HH:mm").format("YYYY-MM-DD HH:mm");
        inputDTO.StartDateTime = moment(startDate, "DD-MMM-YYYY").format("YYYY-MM-DD") + "T" + moment(startTime, "HH:mm").format("HH:mm");

        // Format EndDateTime (End Date + End Time)
        //inputDTO.EndDateTime = moment(`${endDate} ${endTime}`, "DD-MMM-YYYY HH:mm").format("YYYY-MM-DD HH:mm");
        inputDTO.EndDateTime = moment(endDate, "DD-MMM-YYYY").format("YYYY-MM-DD") + "T" + moment(endTime, "HH:mm").format("HH:mm");

        // Format Duration (Duration field is in HH:mm format)
        inputDTO.Duration = moment(duration, "HH:mm:ss").format("HH:mm:ss");
        //inputDTO.Duration = "01:00:00";

        // Add other necessary fields
        inputDTO.Id = $("#AddSchedule").find("[name='ScheduleId']").val();
        inputDTO.Id = inputDTO.Id == "" ? 0 : inputDTO.Id;
        inputDTO.GuestId = $("#GuestSchedule_GuestId").val(); // If you have a guest ID field
        inputDTO.SessionId = $("#AddSchedule").find("[name='NoOfDays']").val();
        $.ajax({
            type: 'POST',
            url: '/Guests/CreateGuestScheduleByCalendar',
            contentType: 'application/json',
            data: JSON.stringify(inputDTO),
            success: function (response) {
                // Show success notification
                if (typeof showNotification === 'function') {
                    showNotification('Schedule saved successfully', 'success');
                } else {
                    alert('Schedule saved successfully');
                }
                
                // Close the modal
                $('#crtevents').modal('hide');
                
                // Refresh calendar instead of reloading page
                if (window.globalCalendarInstance) {
                    window.globalCalendarInstance.refetchEvents();
                }
                
                // Refresh list view if visible
                if ($('#listViewContainer').is(':visible') && window.SchedulesListView) {
                    window.SchedulesListView.loadAndRender();
                }
                
                // Reset form
                $("#AddSchedule")[0].reset();
                $("#AddSchedule").find("[name='ScheduleId']").val('');
                $('#btnDelete').hide();
                //debugger
                //document.getElementById("btnadd").disabled = false;
                //if (response.status == 400) {
                //    toastr.error(response.message);
                //}
                //else if (response.status == 200) {
                //    var id = response.id;
                //    if (uploadCount > 0) {
                //        $.ajax({
                //            url: "/Calendar/UploadFile?id=" + id,
                //            data: formdata,
                //            processData: false,
                //            contentType: false,
                //            method: "post"
                //        });
                //    }
                //    toastr.success(response.message, { timeOut: 300 });
                //    window.setTimeout(function () {
                //        location.reload();
                //    }, 30);
                //}
            },
            error: function (error) {
                console.error('Failed to save schedule:', error);
                var errorMessage = 'Failed to save schedule. Please try again.';
                if (error.responseJSON && error.responseJSON.message) {
                    errorMessage = error.responseJSON.message;
                }
                
                // Show error notification
                if (typeof showNotification === 'function') {
                    showNotification(errorMessage, 'error');
                } else {
                    alert(errorMessage);
                }
            }
        });
    }
}



var title, des, loc, status, accessibility;
var eventid, taskName, startDate, startTime, duration, endDate, endTime;

// DISABLED: FullCalendar initialization moved to SchedulesContent.js to prevent duplicate initialization
// Calendar initialization is now handled exclusively in SchedulesContent.js for the Guest Schedules module
/*
document.addEventListener('DOMContentLoaded', function () {

    let eventsCustom = [];
    eventsCustom.push({
        id: 1,
        title: 'Bedtime',
        description: 'time to sleep',
        start: '2024-12-18 12:00',
        end: '2024-12-18 14:00',
        color: 'green',
    });
    var calendarEl = document.getElementById('calendar');

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
        //dateClick: function (info) {
        //    document.getElementById("startdate").value = info.dateStr;
        //    document.getElementById("enddate").value = info.dateStr;
        //},
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
        //events: eventsCustom,
        events: function (fetchInfo, successCallback, failureCallback, start, end) {
            jQuery.ajax({
                url: "/Guests/GetGuestsEventForCalender?GuestId=" + $("#GuestSchedule_GuestId").val(),
                type: "GET",
                success: function (res) {
                    //debugger;
                    JSON.stringify(res)
                    var events = new Array();
                    $.each(res, function (i, data) {

                        const stringDuration = moment.duration(data.duration);
                        let durationText;

                        if (stringDuration.hours() < 1) {
                            const minutes = stringDuration.minutes();
                            durationText = minutes + " minute" + (minutes !== 1 ? "s" : ""); // Add "s" for plural
                        } else {
                            const hours = stringDuration.hours();
                            durationText = hours + " hour" + (hours !== 1 ? "s" : ""); // Add "s" for plural
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

        //Drag and Drop
        eventDrop: function (event) {
            var eventData = {
                Id: "",
                Title: "",
                Description: "",
                StartDate: "",
                EndDate: "",
                //color:""
            }

            //function setproperty() {
            //    debugger;
            //    eventData.Id = event.event._def.publicId;
            //    eventData.Title = event.event._def.title;
            //    eventData.Description = event.event._def.extendedProps.description;
            //    eventData.StartDate = event.event._instance.range.start.toUTCString();
            //    eventData.EndDate = event.event._instance.range.end.toUTCString();
            //}
            //setproperty();

            //$.ajax({
            //    type: 'POST',
            //    url: '/Calendar/DragAndDrop',
            //    data: { EventRequest: eventData },
            //    success: function (response) {
            //        if (response.status == 400) {
            //            toastr.error(response.message);
            //        }
            //        else if (response.status == 200) {
            //            toastr.success(response.message, { timeOut: 5000 });
            //            window.setTimeout(function () {
            //                location.reload();
            //            }, 3000);
            //        }
            //    },
            //    error: function () {
            //        console.log('Failed');
            //    }
            //})
        },


        eventClick: function (event) {
            debugger;
            var id = event.event._def.publicId;
            eventid = id;

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

                    //$taskId.change();

                    eventid = guestSchedule.id;
                    let guestId = guestSchedule.guestId;
                    taskName = guestSchedule.taskName;
                    let [startDateT, startTimeT] = guestSchedule.startDateTime.split("T");
                    startDate = moment(startDateT, "YYYY-MM-DD").format("DD-MMM-YYYY");
                    startTime = startTimeT;

                    let [endDateT, endTimeT] = guestSchedule.endDateTime.split("T");
                    endDate = moment(endDateT, "YYYY-MM-DD").format("DD-MMM-YYYY");
                    endTime = endTimeT;

                    duration = moment(guestSchedule.duration, "HH:mm").format("HH:mm");
                    $taskId.attr("duration", duration)
                    //durationPicker.setDate(duration, false);
                    //$("#AddSchedule").find("[name='Duration']").val(duration);

                    $("#AddSchedule").find("[name='TaskName']").val(taskName);
                    $("#AddSchedule").find("[name='StartDate']").val(startDate);
                    //flatpickr("#StartDate").setDate(startDate);
                    $("#AddSchedule").find("[name='StartTime']").val(startTime);



                    //let timeParts = duration.split(":");
                    //let validDateTime = "1970-01-01T" + timeParts[0] + ":" + timeParts[1];
                    //flatpickr("#durationPicker").setDate(validDateTime);


                    $("#AddSchedule").find("[name='EndDate']").val(endDate);
                    $("#AddSchedule").find("[name='EndTime']").val(endTime);
                    $("#AddSchedule").find("[name='ScheduleId']").val(eventid);


                    $taskId.change();
                    //des = data.description;
                    //startdate = event.event._instance.range.start.toLocaleDateString();
                    //startdate = event.event._instance.range.start.toISOString().split("T")[0].split(".")[0];
                    //enddate = event.event._instance.range.end.toLocaleDateString();
                    //enddate = event.event._instance.range.end.toISOString().split("T")[0].split(".")[0];
                    //starttime = event.event._instance.range.start.toISOString().split("T")[1].split(".")[0];
                    //endtime = event.event._instance.range.end.toISOString().split("T")[1].split(".")[0];
                    //loc = data.location;
                    //status = data.status;
                    //accessibility = data.accessibility;

                    //document.getElementById("showeventtitle").value = title;
                    //document.getElementById("showeventdes").value = des;
                    //document.getElementById("showeventstartdate").value = startdate;
                    //document.getElementById("showeventenddate").value = enddate;
                    //document.getElementById("showeventstarttime").value = starttime;
                    //document.getElementById("showeventendtime").value = endtime;
                    //document.getElementById("showeventlocation").value = loc;
                    //document.getElementById("showStatus").value = status;
                    //document.getElementById("showAccessibility").value = accessibility;
                },
                error: function (error) {
                    console.log('Failed');
                }
            });


            //$("#btnDelete").click(function () {
            //    var data = event.event._def.publicId;
            //    Swal.fire({
            //        title: "Confirm?",
            //        text: "Are you sure you want to delete this Event?",
            //        icon: "warning",
            //        showCancelButton: true,
            //        confirmButtonColor: '#3085d6',
            //        cancelButtonColor: '#d33',
            //        confirmButtonText: 'Yes, delete it!'
            //    })
            //        .then((willDelete) => {
            //            if (willDelete.isConfirmed) {
            //                $.ajax({
            //                    type: 'GET',
            //                    url: '/Calendar/DeleteEvent',
            //                    data: {
            //                        id: data
            //                    }
            //                })
            //                    .done(function (response) {
            //                        if (response.status == 400) {
            //                            toastr.error(response.message);
            //                        }
            //                        else if (response.status == 200) {
            //                            toastr.success('', 'Event Deleted successfully',
            //                                {
            //                                    timeOut: 5,
            //                                    fadeOut: 5,
            //                                    onHidden: function () {
            //                                        location.reload();
            //                                    },
            //                                });
            //                        }
            //                    }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
            //                        alert("FAIL");
            //                    });
            //            }
            //        });
            //    $('#eventdetails').modal('hide');
            //});


        },

        //dateClick: function (info) {
        //    alert(info.dateStr);
        //},

    });
    calendar.render();

});
*/


var filename = new Array();
var arrFileName = new Array();
function changeFileName() {
    var fileName = $("#hidUploadFileValue").val();
    arrFileName = fileName.split(",");
    $('.filepond--file-info-main').each(function () {
        var filePathData = $(this).text();
        for (var iloop = 0; iloop < arrFileName.length; iloop++) {
            if (filePathData == arrFileName[iloop].split("~")[0]) {
                $(this).text(arrFileName[iloop].split("~")[1]);
            }
        }
    });

    //$("fieldset").each(function () {
    //    //debugger;
    //    var className = $(this).attr("class");
    //    if (className == "filepond--file-wrapper") {
    //        var tagName = $(this).closest('legend').prevObject[0].children[0].tagName.toLowerCase();
    //        if (tagName == "legend") {
    //            var txt = $(this).closest('legend').prevObject[0].children[0].textContent;
    //            for (var iloop = 0; iloop < arrFileName.length; iloop++) {
    //                if (txt == arrFileName[iloop].split("~")[0]) {
    //                    debugger;
    //                    $(this).closest('legend').prevObject[0].children[0].textContent = arrFileName[iloop].split("~")[1];
    //                }
    //            }
    //        }
    //    }
    //});
}

function EditselectStatus(val) {
    debugger;
    var busy = document.getElementById("editselectbusy").value;
    if (val == busy) {
        editeventData.Status = "Busy";
        editeventData.ColorCode = "#ff0000";
    }
    else {
        editeventData.Status = "Free";
    }
}

function EditselectAccessibility(val) {
    debugger;
    var public = document.getElementById("editselectpublic").value;
    if (val == public) {
        editeventData.Accessibility = "Public";
    }
    else {
        editeventData.Accessibility = "Private";
    }
}

var editeventData = {
    Id: "",
    Title: "",
    Description: "",
    StartDate: "",
    EndDate: "",
    StartTime: "",
    EndTime: "",
    Location: "",
    Status: "",
    Accessibility: "",
    ColorCode: "",
}

$(document).ready(function () {
    $("#btnEdit").click(function () {

        debugger;
        $('#eventdetails').modal('hide');
        $('#editevent').modal('show');
        document.getElementById("editeventtitle").value = title;
        document.getElementById("editeventdes").value = des;
        document.getElementById("editeventstartdate").value = startdate;
        document.getElementById("editeventenddate").value = enddate;
        document.getElementById("editeventstarttime").value = starttime;
        document.getElementById("editeventendtime").value = endtime;
        document.getElementById("editeventlocation").value = loc;

        editpond.removeFiles();
        var filePathName = "";
        $.ajax({
            url: '/Calendar/EventFileDetails',
            data: { Id: eventid },
            type: "GET",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var filedata = data;
                $.each(filedata, function (key, value) {
                    filePathName += value.filePath + "~" + value.fileName + ",";
                    editpond.addFiles('././EventAttachment/' + value.filePath);
                })

                filePathName = filePathName.substring(0, filePathName.length - 1);
                $("#hidUploadFileValue").val(filePathName);
                setTimeout(changeFileName, 1000);
            }
        });

        $.ajax({
            url: '/Calendar/EventDetail',
            data: { Id: eventid },
            type: "GET",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $("#dvstatusaccessibility").empty();
                var selectstatusbusy = "";
                var selectstatusfree = "";
                var selectaccessibilitypublic = "";
                var selectaccessibilityprivate = "";
                var status = data.status;
                var accessibility = data.accessibility;
                if (status == "Busy" && accessibility == "Public") {
                    selectstatusbusy = "selected";
                    selectaccessibilitypublic = "selected";
                }
                else if (status == "Busy" && accessibility == "Private") {
                    selectstatusbusy = "selected";
                    selectaccessibilityprivate = "selected";
                }
                else if (status == "Free" && accessibility == "Public") {
                    selectstatusfree = "selected";
                    selectaccessibilitypublic = "selected";
                }
                else {
                    selectstatusfree = "selected";
                    selectaccessibilityprivate = "selected";
                }
                $("#dvstatusaccessibility").append("<div class='form-group col-sm-3 col-lg-2'><select class='form-control' onchange='EditselectStatus(this.value)'><option value='0' id='editselectbusy' " + selectstatusbusy + ">Busy</option><option  value='1' id='editselectfree' " + selectstatusfree + ">Free</option></select></div><div class='form-group col-sm-3 col-lg-3'><select class='form-control' onchange='EditselectAccessibility(this.value)'><option value='0' id='editselectpublic' " + selectaccessibilitypublic + ">Public</option><option value='1' id='editselectprivate' " + selectaccessibilityprivate + ">Private</option></select></div>");
            },
            error: function () {
                console.log('Failed');
            }
        });

        $.ajax({
            type: 'POST',
            url: '/Calendar/GetReminders',
            data: { Id: eventid },
            success: function (data) {
                var reminderArr = new Array();
                reminderArr = data;
                $("#dvReminderEdit").empty();
                $("#dvReminderEdit").addClass("form-row align-items-center");
                var count = 0;
                $.each(reminderArr, function (i, item) {
                    debugger;
                    count += 1;
                    var time = item.remindersTime;
                    var remindNumber = 1;
                    var Minute = "";
                    var Hours = "";
                    var Days = "";

                    if (parseInt(time) < 60) {
                        remindNumber = parseInt(time);
                        Minute = "selected";
                    }
                    else if (parseInt(time) >= 60 && parseInt(time) < 1440) {
                        remindNumber = parseInt(time) / 60;
                        Hours = "selected";
                    }
                    else if (parseInt(time) >= 1440) {
                        remindNumber = parseInt(time) / 1440;
                        Days = "selected";
                    }

                    if (count == 1) {
                        $("#dvReminderEdit").append("<div class='remindereditselection d-flex' id='divReminder_" + count + "'><div class='form-group'><input type='email' class='form-control' id='remindereditemail_" + count + "' placeholder='Email' value='" + item.email + "' /></div ><div class='form-group mx-3'><input type='number' min='1' class='form-control px-2 mr-6' max='59' id='remindereditnumber_" + count + "' style='width:50px;' value='" + remindNumber + "' /></div><div class='form-group'><select class='form-control' style='width:150px;' id='remindereditduration_" + count + "' onselect='selectremindersduration(this.value)'><option " + Days + ">Days</option><option " + Hours + ">Hours</option><option " + Minute + ">Minutes</option></select></div></div>");
                    }
                    else {
                        $("#dvReminderEdit").append("<div class='remindereditselection d-flex' id='divReminder_" + count + "'><div class='form-group'><input type='email' class='form-control' id='remindereditemail_" + count + "' placeholder='Email' value='" + item.email + "' /></div ><div class='form-group mx-3'><input type='number' min='1' class='form-control px-2 mr-6' max='59' id='remindereditnumber_" + count + "' style='width:50px;' value='" + remindNumber + "' /></div><div class='form-group'><select class='form-control' style='width:150px;' id='remindereditduration_" + count + "' onselect='selectremindersduration(this.value)'><option " + Days + ">Days</option><option " + Hours + ">Hours</option><option " + Minute + ">Minutes</option></select></div><div class='form-group'><div class='m-3'><a id='removeReminder_" + count + "' class='removeElement' style='float: right;' href='#'>Delete</a></div></div>");
                    }
                })
                if (count == 0) {
                    count = 1;
                    $("#dvReminderEdit").append("<div class='remindereditselection d-flex' id='divReminder_" + count + "'><div class='form-group'><input type='email' class='form-control' id='remindereditemail_" + count + "' placeholder='Email' /></div ><div class='form-group mx-3'><input type='number' min='1' value='1' max='59' class='form-control px-2 mr-6' id='remindereditnumber_" + count + "' style='width:50px;' /></div><div class='form-group'><select class='form-control' style='width:150px;' id='remindereditduration_" + count + "' onselect='selectremindersduration(this.value)'><option>Days</option><option selected>Hours</option><option>Minutes</option></select></div></div>");
                }
            },
            error: function () {
                console.log('Failed');
            }
        });


        $("#editguestUser").empty();
        $.ajax({
            type: 'POST',
            url: '/Calendar/GetEventGuestName',
            data: { Id: eventid },
            success: function (Inviteemail) {
                var nameArr = new Array();
                nameArr = Inviteemail;
                $.each(nameArr, function (i, item) {
                    var id = item.split("~");
                    $("#editguestUser").append("<div class='alert alert-dismissible fade show' role='alert'>" + id[0] + "<button type = 'button' class='close'  data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button><input class='editguestList' type='hidden' id='" + id[1] + "'></div>");
                })
            },
            error: function () {
                console.log('Failed');
            }
        });

        $("#editGuestName").autocomplete({
            source: function (request, response) {
                var param = { ContactName: $('#editGuestName').val() };
                $.ajax({
                    url: "/Calendar/getFirmContactListOnUserId?ContactName=" + $('#editGuestName').val(),
                    data: JSON.stringify(param),
                    dataType: "json",
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function (data) {
                        return data;
                    },
                    success: function (data) {
                        response(data);
                    }
                });
            },
            minLength: 2
        });

        $("#editGuestName").change(function () {
            var guestName = $(this).val();
            if (guestName != "") {
                $.ajax({
                    url: "/Calendar/getContactIdByFullName?GuestName=" + guestName,
                    dataType: "json",
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function (data) {
                        return data;
                    },
                    success: function (result) {
                        var data = result.message;
                        if (data != "") {
                            $("#editguestUser").append("<div class='alert alert-dismissible fade show' id='editguestemail' role='alert'>" + guestName + "<button type = 'button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button><input class='editguestList' type='hidden' id='" + data + "'></div>");
                            document.getElementById('editGuestName').value = "";
                            return false;
                        }
                        else {
                            var conf = confirm(guestName + " does not exists in contact list. Do you want to add it?");
                            if (conf) {
                                $('#createcontact').modal('toggle');
                                document.getElementById('editGuestName').value = "";
                            }
                        }
                    }
                });
            }
        });
        return false;
    });

    //Edit Code start from Here....
    //var editeventData = {
    //    Id: "",
    //    Title: "",
    //    Description: "",
    //    StartDate: "",
    //    EndDate: "",
    //    StartTime: "",
    //    EndTime: "",
    //    Location: "",
    //    Status:"",
    //    ColorCode:"",
    //}

    var recurrenceenddate;
    function setproperty() {
        debugger;
        editeventData.Id = eventid;
        editeventData.Title = $("#editeventtitle").val();
        editeventData.Description = $("#editeventdes").val();
        editeventData.StartDate = $("#editeventstartdate").val();
        editeventData.EndDate = $("#editeventenddate").val();
        var checkbox = document.getElementById('editAllday');
        if (checkbox.checked == true) {
            editeventData.StartTime = "00:00";
            editeventData.EndTime = "23:59";
        }
        else {
            editeventData.StartTime = $("#editeventstarttime").val();
            editeventData.EndTime = $("#editeventendtime").val();
        }
        editeventData.Location = $("#editeventlocation").val();
        var endchecked = document.getElementById("editendcheck");
        //if (endchecked.checked == true) {
        //    recurrenceenddate = $("#editreccenddate").val();
        //}
        if (editeventData.Status == '') {
            editeventData.Status = status;
            if (editeventData.Status == "Busy") {
                editeventData.ColorCode = "#ff0000";
            }
        }
        if (editeventData.Accessibility == '') {
            editeventData.Accessibility = accessibility;
        }
        console.log(editeventData);
    }

    function isValidate() {
        debugger;
        var isValid = true;
        var title = $("#editeventtitle").val();
        var description = $("#editeventdes").val();
        var startDate = $("#editeventstartdate").val();
        var endDate = $("#editeventenddate").val();
        var startTime = $("#editeventstarttime").val();
        var endTime = $("#editeventendtime").val();
        var location = $("#editeventlocation").val();

        if (title == null || title == '') {
            $("#title-name").text("Please enter Title")
            isValid = false;
        }
        if (description == null || description == '') {
            $("#des-name").text("Please Enter Description")
            isValid = false;
        }
        if (startDate == null || startDate == '') {
            $("#start-name").text("Please Select StartDate")
            isValid = false;
        }
        var checkbox = document.getElementById('editthisrepeat');
        if (checkbox.checked) {
            isValid = true;
        }
        else {
            if (endDate == null || endDate == '') {
                $("#end-name").text("Please Select EndDate")
                isValid = false;
            }
        }
        var checkbox = document.getElementById('editAllday');
        if (checkbox.checked == true) {
            isValid = true;
        }
        else {
            if (startTime == null || startTime == '') {
                $("#end-name").text("Please Select StartTime")
                isValid = false;
            }
            if (endTime == null || endTime == '') {
                $("#end-name").text("Please Select EndTime")
                isValid = false;
            }
        }
        if (location == null || location == '') {
            $("#end-name").text("Please Select Location")
            isValid = false;
        }
        return isValid;
    }

    $("#Editdata").click(function () {
        debugger;
        document.getElementById("Editdata").disabled = true;

        var existingAttachmentIds = "";
        $("fieldset").each(function () {
            var className = $(this).attr("class");
            if (className == "filepond--file-wrapper") {
                var tagName = $(this).closest('legend').prevObject[0].children[0].tagName.toLowerCase();
                if (tagName == "legend") {
                    var txt = $(this).closest('legend').prevObject[0].children[0].textContent;
                    var arrTxt = txt.split(".");
                    existingAttachmentIds += arrTxt[0] + ",";
                }
            }
        });
        if (existingAttachmentIds != "") {
            existingAttachmentIds = existingAttachmentIds.substring(0, existingAttachmentIds.length - 1);
        }

        var guestIds = "";
        $('#editguestUser .editguestList').each(function () {
            var guestId = $(this).attr("id");
            guestIds += guestId + ",";
        });
        if (guestIds != "") {
            guestIds = guestIds.substring(0, guestIds.length - 1);
        }

        var emailReminderData = [];
        $('.remindereditselection').each(function () {
            var emailId = $(this).attr("id");
            var email_split_id = emailId.split("_");
            var indexNo = email_split_id[1];
            var reminderemail = "#remindereditemail_" + indexNo;
            var remindernumber = "#remindereditnumber_" + indexNo;
            var reminderduration = "#remindereditduration_" + indexNo;
            var reminderemailValue = $(reminderemail).val();
            var remindernumberValue = $(remindernumber).val();
            var reminderdurationValue = $(reminderduration).val();
            if (reminderemailValue != "") {
                emailReminderData.push({
                    email: reminderemailValue,
                    number: remindernumberValue,
                    duration: reminderdurationValue
                });
            }
        });

        var jsonemailRemindersString = JSON.stringify(emailReminderData);
        $("#hidemailreminders").val(jsonemailRemindersString);

        var dates = new Array();
        var test = new Array();
        var d = new Date();
        var date = d.getDate();
        d = d.getDay();
        $("input[name='editweekdays']:checked").each(function () {
            test.push($(this).val());
        });
        var i;
        for (i = 0; i < test.length; i++) {
            if (d <= test[i]) {
                var t = test[i] - d;
                var d1 = new Date();
                d1.setDate(d1.getDate() + t);
                var d2 = d1.toLocaleDateString();
                dates.push(d2);
            }
            else {
                var t = 7 - d + Number(test[i]);
                var d1 = new Date();
                d1.setDate(d1.getDate() + t);
                var d2 = d1.toLocaleDateString();
                dates.push(d2);
            }
        }

        editpondFiles = editpond.getFiles();
        var formdata = new FormData();
        var edituploadCount = editpondFiles.length;
        for (var i = 0; i < editpondFiles.length; i++) {
            formdata.append('uploadFile', editpondFiles[i].file);
        }

        $.each(arrFileName, function (i, item) {
            var file = item.split("~")[1];
            filename.push(file);
        })


        var checkbox = document.getElementById("editthisrepeat");
        var count = 0;
        if (checkbox.checked == true) {
            count = 1;
        }
        if (isValidate()) {
            setproperty();
            $.ajax({
                type: 'POST',
                url: '/Calendar/Edit',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                data: {
                    EventRequest: editeventData,
                    starttime: editeventData.StartTime,
                    endtime: editeventData.EndTime,
                    selectedvalue: selectedRepetition,
                    GuestIds: guestIds,
                    DateArray: String(dates),
                    GuestIds: guestIds,
                    uploadCount: edituploadCount,
                    emailreminders: jsonemailRemindersString,
                    checkRecurrence: count,
                    EndDate: recurrenceenddate,
                    Eventend: Eventenddate,
                },
                success: function (response) {
                    document.getElementById("Editdata").disabled = false;
                    if (response.status == 400) {
                        toastr.error(response.message);
                    }
                    else if (response.status == 200) {
                        debugger;
                        var id = response.id;
                        if (edituploadCount > 0) {
                            $.ajax({
                                url: "/Calendar/EditUploadFile?id=" + id + "&filename=" + filename + "&Attachment=" + existingAttachmentIds,
                                data: formdata,
                                processData: false,
                                contentType: false,
                                method: "post"
                            });
                        }
                        toastr.success(response.message, { timeOut: 300 });
                        window.setTimeout(function () {
                            location.reload();
                        }, 30);
                    }
                }
            });
        }
    })
})
