/**
 * SchedulesContent.js
 * Main calendar initialization and view management for Guest Schedules
 * ONLY place where FullCalendar is initialized
 * Uses custom Bootstrap modal for "+more" link instead of FullCalendar popover
 */

(function() {
    'use strict';

    // SINGLE global calendar instance
var globalCalendarInstance = null;

    var SchedulesContent = {
        initialized: false,
        popoverObserver: null,
        
        // Initialize the module
        init: function() {
            // Prevent multiple initializations
            if (this.initialized) {
                return;
            }
            
            this.setupViewSwitching();
            this.setupPrintButton();
            this.setupFilterChangeHandlers();
            this.initializeCalendar();
            this.initialized = true;
        },

        // Setup view switching (Calendar/List)
        setupViewSwitching: function() {
            var self = this;

            $('#btnCalendarView').on('click', function() {
                self.showCalendarView();
            });

            $('#btnListView').on('click', function() {
                self.showListView();
            });
        },

        // Show calendar view
        showCalendarView: function() {
            $('#calendarViewContainer').show();
            $('#listViewContainer').hide();
            $('#btnCalendarView').removeClass('btn-outline-primary').addClass('btn-primary');
            $('#btnListView').removeClass('btn-primary').addClass('btn-outline-primary');
            
            // Refresh calendar if initialized - use refetchEvents, NOT reinit
            if (globalCalendarInstance) {
                globalCalendarInstance.refetchEvents();
                // Small delay to ensure calendar updates properly
                setTimeout(function() {
                    if (globalCalendarInstance) {
                        globalCalendarInstance.updateSize();
                    }
                }, 100);
            }
        },

        // Show list view
        showListView: function() {
            $('#calendarViewContainer').hide();
            $('#listViewContainer').show();
            $('#btnListView').removeClass('btn-outline-primary').addClass('btn-primary');
            $('#btnCalendarView').removeClass('btn-primary').addClass('btn-outline-primary');
            
            // Load and render list view
            if (window.SchedulesListView) {
                window.SchedulesListView.loadAndRender();
            }
        },

        // Setup print button
        setupPrintButton: function() {
            var self = this;
            $('#btnPrint').on('click', function() {
                self.openPrintPage();
            });
            
            // Setup Excel download button
            $('#btnDownloadExcel').on('click', function() {
                self.downloadExcel();
            });
        },

        // Open print page in new tab
        openPrintPage: function() {
            var guestId = $('#GuestSchedule_GuestId').val();
            var queryString = '';
            
            if (window.SchedulesFilters) {
                queryString = window.SchedulesFilters.buildQueryString();
            }
            
            var printUrl = '/Guests/ReviewMemberDetails/' + guestId + '/Schedules/Print' + queryString;
            window.open(printUrl, '_blank');
        },

        // Download Excel file
        downloadExcel: function() {
            var guestId = $('#GuestSchedule_GuestId').val();
            if (!guestId) {
                alert('Guest ID not found');
                return;
            }

            var queryString = '';
            if (window.SchedulesFilters) {
                queryString = window.SchedulesFilters.buildQueryString();
            }
            
            var excelUrl = '/Guests/ReviewMemberDetails/' + guestId + '/Schedules/ExportExcel' + queryString;
            window.location.href = excelUrl;
        },

        // Setup filter change handlers
        setupFilterChangeHandlers: function() {
            var self = this;
            
            // Refresh calendar when filters change (only if calendar view is active)
            // Use refetchEvents, NOT reinitialization
            $('#filterFromDate, #filterToDate, #filterTaskId, #filterEmployeeId, #filterResourceId').on('change', function() {
                if ($('#calendarViewContainer').is(':visible')) {
                    if (globalCalendarInstance) {
                        globalCalendarInstance.refetchEvents();
                    }
                } else {
                    // Refresh list view
                    if (window.SchedulesListView) {
                        window.SchedulesListView.loadAndRender();
                    }
                }
            });
        },

        // Open day events modal with all events for the selected day
        openDayEventsModal: function(date, allSegs) {
            if (!allSegs || allSegs.length === 0) {
                console.warn('No events to display');
                return;
            }

            // Format the date for display
            var dateStr = moment(date).format('dddd, MMMM DD, YYYY');
            $('#dayEventsModalDate').text(dateStr);

            // Build table rows from segments
            var html = '';
            
            // Sort segments by start time
            var sortedSegs = allSegs.slice().sort(function(a, b) {
                var eventA = a.event || a;
                var eventB = b.event || b;
                var timeA = eventA.start ? moment(eventA.start) : moment(a.start);
                var timeB = eventB.start ? moment(eventB.start) : moment(b.start);
                return timeA.diff(timeB);
            });

            $.each(sortedSegs, function(i, seg) {
                // Handle both segment.event and direct event objects
                var event = seg.event || seg;
                if (!event || !event.start) {
                    console.warn('Invalid event segment:', seg);
                    return;
                }

                var startMoment = moment(event.start);
                var endMoment = moment(event.end);
                var timeRange = startMoment.format('hh:mm A') + ' - ' + endMoment.format('hh:mm A');
                
                // Calculate duration
                var duration = moment.duration(endMoment.diff(startMoment));
                var durationText = '';
                if (duration.hours() < 1) {
                    var minutes = duration.minutes();
                    durationText = minutes + " min" + (minutes !== 1 ? "s" : "");
                } else {
                    var hours = duration.hours();
                    var mins = duration.minutes();
                    if (mins > 0) {
                        durationText = hours + "h " + mins + "m";
                    } else {
                        durationText = hours + " hour" + (hours !== 1 ? "s" : "");
                    }
                }

                // Extract task name (remove duration from title if present)
                var taskName = event.title || 'N/A';
                taskName = taskName.replace(/\(\d+.*?\)/g, '').trim();
                
                // Get extended properties
                var extendedProps = event.extendedProps || {};
                var resourceName = extendedProps.resourceName || '-';
                var therapist1 = extendedProps.therapist1Name || extendedProps.employeeName1 || '-';
                var therapist2 = extendedProps.therapist2Name || extendedProps.employeeName2 || '-';
                var therapist3 = extendedProps.therapist3Name || extendedProps.employeeName3 || '-';
                
                // Get event ID for edit/delete
                var eventId = event.id || event._def?.publicId || '';

                // Build row with edit and delete buttons
                html += '<tr>';
                html += '<td>' + this.escapeHtml(timeRange) + '</td>';
                html += '<td>' + this.escapeHtml(taskName) + '</td>';
                html += '<td>' + durationText + '</td>';
                html += '<td>' + this.escapeHtml(resourceName) + '</td>';
                html += '<td>' + this.escapeHtml(therapist1) + '</td>';
                html += '<td>' + this.escapeHtml(therapist2) + '</td>';
                html += '<td>' + this.escapeHtml(therapist3) + '</td>';
                html += '<td class="text-center">';
                html += '<div class="btn-group btn-group-sm" role="group">';
                html += '<button type="button" class="btn btn-sm btn-primary edit-event-btn" data-event-id="' + eventId + '" title="Edit Schedule">';
                html += '<i class="bi bi-pencil"></i>';
                html += '</button>';
                html += '<button type="button" class="btn btn-sm btn-danger delete-event-btn" data-event-id="' + eventId + '" title="Delete Schedule">';
                html += '<i class="bi bi-trash"></i>';
                html += '</button>';
                html += '</div>';
                html += '</td>';
                html += '</tr>';
            }.bind(this));

            // Update modal body
            if (html === '') {
                html = '<tr><td colspan="8" class="text-center">No events found</td></tr>';
            }
            $('#dayEventsModalBody').html(html);
            
            // Attach event handlers for edit and delete buttons
            this.attachDayEventsModalHandlers();

            // Show modal using Bootstrap 5 (with fallback for jQuery Bootstrap)
            var modalEl = document.getElementById('dayEventsModal');
            if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
                // Bootstrap 5 native API
                var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                modal.show();
            } else if (typeof $ !== 'undefined' && $.fn.modal) {
                // jQuery Bootstrap fallback
                $('#dayEventsModal').modal('show');
            } else {
                console.error('Bootstrap modal not available');
            }
        },
        
        // Attach handlers for edit and delete buttons in day events modal
        attachDayEventsModalHandlers: function() {
            var self = this;
            
            // Edit button handler
            $('.edit-event-btn').off('click').on('click', function() {
                var eventId = $(this).data('event-id');
                if (eventId) {
                    self.editEventFromModal(eventId);
                }
            });
            
            // Delete button handler
            $('.delete-event-btn').off('click').on('click', function() {
                var eventId = $(this).data('event-id');
                if (eventId) {
                    self.deleteEventFromModal(eventId);
                }
            });
        },
        
        // Edit event from day events modal
        editEventFromModal: function(eventId) {
            var self = this;
            
            // Close the day events modal
            var dayModal = bootstrap.Modal.getInstance(document.getElementById('dayEventsModal'));
            if (dayModal) {
                dayModal.hide();
            }
            
            // Load event data and open edit modal (same as eventClick)
            var inputDTO = {
                "Id": eventId
            };
            
            $.ajax({
                type: 'POST',
                url: '/Guests/GetGuestsEventForCalenderById',
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $('#crtevents').modal('toggle');
                    if (typeof initFlatPickerDuration === 'function') {
                        initFlatPickerDuration();
                    }

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
                    $taskId.attr("employeeid1", guestSchedule.employeeId1 || 0)
                    $taskId.attr("employeeid2", guestSchedule.employeeId2 || 0)
                    $taskId.attr("employeeid3", guestSchedule.employeeId3 || 0)
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

                            // Show delete button when editing
                            $('#btnDelete').show();
                            
                            // Hide X field when editing
                            $("#noOfDaysContainer").hide();

                    $taskId.change();
                },
                error: function (error) {
                    console.error('Failed to load event details:', error);
                    if (typeof showNotification === 'function') {
                        showNotification('Failed to load event details', 'error');
                    } else {
                        alert('Failed to load event details');
                    }
                }
            });
        },
        
        // Delete event from day events modal
        deleteEventFromModal: function(eventId) {
            // Close the day events modal
            var dayModal = bootstrap.Modal.getInstance(document.getElementById('dayEventsModal'));
            if (dayModal) {
                dayModal.hide();
            }
            
            // Store schedule ID for confirmation
            $('#deleteConfirmModal').data('schedule-id', eventId);
            $('#deleteConfirmModal').data('source', 'dayeventsmodal');
            
            // Show confirmation modal
            var deleteModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
            deleteModal.show();
            
            // Override the confirm button handler for day events modal
            $('#btnConfirmDelete').off('click.dayevents').on('click.dayevents', function() {
                confirmDeleteFromDayEventsModal(eventId);
            });
        },

        // Escape HTML to prevent XSS
        escapeHtml: function(text) {
            if (!text) return '';
            var map = {
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#039;'
            };
            return String(text).replace(/[&<>"']/g, function(m) { return map[m]; });
        },

        // Initialize FullCalendar (ONLY place where calendar is initialized)
        initializeCalendar: function() {
            // Destroy existing calendar instance if it exists
            if (globalCalendarInstance) {
                try {
                    // Disconnect popover observer if it exists
                    if (this.popoverObserver) {
                        this.popoverObserver.disconnect();
                        this.popoverObserver = null;
                    }
                    
                    // Remove any existing popovers
                    $('.fc-popover').remove();
                    
                    // Destroy the calendar instance
                    globalCalendarInstance.destroy();
                    globalCalendarInstance = null;
                } catch (err) {
                    console.warn('Error destroying existing calendar:', err);
                    globalCalendarInstance = null;
                }
            }

    var calendarEl = document.getElementById('calendar');
    if (!calendarEl) {
                console.warn('Calendar element not found');
                return;
            }

            // Check if FullCalendar is available
            var FullCalendarLib = window.FullCalendar || FullCalendar;
            if (!FullCalendarLib) {
                console.error('FullCalendar library not found');
                return;
            }

            var self = this;

            // Create new calendar instance
            globalCalendarInstance = new FullCalendarLib.Calendar(calendarEl, {
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
                dayMaxEvents: true, // Enable "+more" link
                moreLinkClick: function(arg) {
                    // Custom handler for "+more" link - use Bootstrap modal instead of popover
                    // Completely prevent default FullCalendar popover behavior
                    if (arg.jsEvent) {
                        arg.jsEvent.preventDefault();
                        arg.jsEvent.stopPropagation();
                        arg.jsEvent.stopImmediatePropagation();
                    }
                    
                    // Get the date and all segments for this day
                    var date = arg.date;
                    var allSegs = arg.allSegs;
                    
                    // Open custom Bootstrap modal
                    self.openDayEventsModal(date, allSegs);
                    
                    // Return false to prevent any default behavior
                    return false;
                },
        eventTimeFormat: { hour: 'numeric', minute: '2-digit' },
        customButtons: {
            myCustomButton: {
                text: 'Add Event',
                click: function () {
                    if (typeof resetAddScheduleForm === 'function') {
                        resetAddScheduleForm();
                    }
                    $('#crtevents').modal('toggle');
                            if (typeof initFlatPickerDuration === 'function') {
                    initFlatPickerDuration();
                            }
                            if (typeof initCreateEventModal === 'function') {
                    initCreateEventModal();
                            }
                            if (typeof SetDates === 'function') {
                    SetDates(null, null, 'AddEvent');
                            }
                }
            }
        },
        select: function (info) {
                    // Hide delete button when creating new event from date selection
                    $('#btnDelete').hide();
                    // Clear ScheduleId
                    $("#AddSchedule").find("[name='ScheduleId']").val('');
                    // Show X field for new schedules
                    $("#noOfDaysContainer").show();
                    if (typeof resetAddScheduleForm === 'function') {
                        resetAddScheduleForm();
                    }
            $('#crtevents').modal('toggle');
                    if (typeof initFlatPickerDuration === 'function') {
            initFlatPickerDuration();
                    }
                    if (typeof initCreateEventModal === 'function') {
            initCreateEventModal();
                    }
                    if (typeof SetDates === 'function') {
            SetDates(info.startStr, null, 'DateClick');
                    }
        },
        events: function (fetchInfo, successCallback, failureCallback) {
                    var guestId = $('#GuestSchedule_GuestId').val();
                    if (!guestId) {
                        console.warn('GuestId not found, returning empty events');
                        successCallback([]);
                        return;
                    }

            jQuery.ajax({
                        url: "/Guests/GetGuestsEventForCalender?GuestId=" + guestId,
                type: "GET",
                success: function (res) {
                            console.log('Calendar events API response:', res);
                            var events = [];
                            
                            if (!res || res.length === 0) {
                                console.warn('No events returned from API');
                                successCallback([]);
                                return;
                            }
                            
                    $.each(res, function (i, data) {
                                try {
                                    // Handle Duration - it might be a string (HH:mm:ss) or TimeSpan object
                                    var durationValue = data.duration || data.Duration;
                                    var durationObj;
                                    
                                    if (typeof durationValue === 'string') {
                                        // Parse string format like "01:30:00" or "01:30"
                                        durationObj = moment.duration(durationValue);
                                    } else if (durationValue && typeof durationValue === 'object') {
                                        // If it's already a duration object or has hours/minutes properties
                                        if (durationValue.hours !== undefined && durationValue.minutes !== undefined) {
                                            durationObj = moment.duration({
                                                hours: durationValue.hours,
                                                minutes: durationValue.minutes,
                                                seconds: durationValue.seconds || 0
                                            });
                                        } else {
                                            durationObj = moment.duration(durationValue);
                                        }
                                    } else {
                                        // Calculate from start and end times
                                        var start = moment(data.startDateTime || data.StartDateTime);
                                        var end = moment(data.endDateTime || data.EndDateTime);
                                        durationObj = moment.duration(end.diff(start));
                                    }
                                    
                        let durationText;
                                    if (durationObj.hours() < 1) {
                                        const minutes = durationObj.minutes();
                            durationText = minutes + " minute" + (minutes !== 1 ? "s" : "");
                        } else {
                                        const hours = durationObj.hours();
                                        const mins = durationObj.minutes();
                                        if (mins > 0) {
                                            durationText = hours + "h " + mins + "m";
                                        } else {
                            durationText = hours + " hour" + (hours !== 1 ? "s" : "");
                        }
                                    }
                                    
                                    // Get property values (handle both camelCase and PascalCase)
                                    var eventId = data.id || data.Id;
                                    var taskName = data.taskName || data.TaskName || 'Untitled';
                                    var startDateTime = data.startDateTime || data.StartDateTime;
                                    var endDateTime = data.endDateTime || data.EndDateTime;
                                    var taskId = data.taskId || data.TaskId;
                                    var resourceId = data.resourceId || data.ResourceId;
                                    var resourceName = data.resourceName || data.ResourceName || '-';
                                    var employeeId1 = data.employeeId1 || data.EmployeeId1;
                                    var employeeId2 = data.employeeId2 || data.EmployeeId2;
                                    var employeeId3 = data.employeeId3 || data.EmployeeId3;
                                    var employeeName1 = data.employeeName1 || data.EmployeeName1 || '-';
                                    var employeeName2 = data.employeeName2 || data.EmployeeName2 || '-';
                                    var employeeName3 = data.employeeName3 || data.EmployeeName3 || '-';
                                    
                                    // Default color if not provided
                                    var eventColor = data.colorCode || data.ColorCode || '#3788d8';
                                    
                                    if (!startDateTime || !endDateTime) {
                                        console.warn('Event missing start or end time:', data);
                                        return;
                                    }
                                    
                                    events.push({
                                        id: eventId,
                                        title: taskName + " (" + durationText + ")",
                                        description: data.description || '',
                                        start: startDateTime,
                                        end: endDateTime,
                                        color: eventColor,
                                        extendedProps: {
                                            taskId: taskId,
                                            taskName: taskName,
                                            resourceId: resourceId,
                                            resourceName: resourceName,
                                            employeeId1: employeeId1,
                                            therapist1Name: employeeName1,
                                            employeeId2: employeeId2,
                                            therapist2Name: employeeName2,
                                            employeeId3: employeeId3,
                                            therapist3Name: employeeName3,
                                            employeeName3: employeeName3,
                                            duration: durationValue
                                        }
                                    });
                                } catch (err) {
                                    console.error('Error processing event:', err, data);
                                }
                            });

                            console.log('Processed events for calendar:', events.length);

                            // Apply filters if available
                            if (window.SchedulesFilters) {
                                events = window.SchedulesFilters.applyFilters(events);
                                console.log('Events after filtering:', events.length);
                            }

                            successCallback(events);
                        },
                        error: function(error) {
                            console.error('Failed to load calendar events:', error);
                            console.error('Error details:', error.responseText || error.statusText);
                            if (failureCallback) {
                                failureCallback(error);
                            } else {
                                successCallback([]);
                            }
                        }
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
                            if (typeof initFlatPickerDuration === 'function') {
                    initFlatPickerDuration();
                            }

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
                            $taskId.attr("employeeid1", guestSchedule.employeeId1 || 0)
                            $taskId.attr("employeeid2", guestSchedule.employeeId2 || 0)
                            $taskId.attr("employeeid3", guestSchedule.employeeId3 || 0)
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

                            // Show delete button when editing
                            $('#btnDelete').show();
                            
                            // Hide X field when editing
                            $("#noOfDaysContainer").hide();

                    $taskId.change();
                },
                error: function (error) {
                            console.error('Failed to load event details:', error);
                }
            });
        },
    });
    
            // Render calendar
            globalCalendarInstance.render();
            
            // Remove any existing FullCalendar popovers immediately after render
            // This prevents default popover from appearing
            setTimeout(function() {
                $('.fc-popover').remove();
            }, 0);
            
            // Set up observer to remove popovers if they appear
            var observer = new MutationObserver(function(mutations) {
                mutations.forEach(function(mutation) {
                    mutation.addedNodes.forEach(function(node) {
                        if (node.nodeType === 1) { // Element node
                            var $node = $(node);
                            if ($node.hasClass('fc-popover') || $node.find('.fc-popover').length > 0) {
                                $('.fc-popover').remove();
                            }
                        }
                    });
                });
            });
            
            // Observe document body for popover additions
            observer.observe(document.body, {
                childList: true,
                subtree: true
            });
            
            // Store observer for cleanup if needed
            this.popoverObserver = observer;
            
            // Store globally for backward compatibility
            window.globalCalendarInstance = globalCalendarInstance;
            
            // Store in module for internal use
            this.calendarInstance = globalCalendarInstance;
        }
    };

    // Initialize when DOM is ready or when custom event is triggered
    function setupInitialization() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', function() {
                setTimeout(function() {
                    SchedulesContent.init();
                }, 100);
            });
        } else {
            setTimeout(function() {
                SchedulesContent.init();
            }, 100);
        }

        // Listen for custom initialization event
        document.addEventListener('initializeCalendar', function() {
            setTimeout(function() {
                SchedulesContent.init();
            }, 100);
        });
    }

    // Start initialization
    setupInitialization();

    // Expose to global scope
    window.SchedulesContent = SchedulesContent;

})();

// Delete Schedule Function - Shows confirmation modal
function DeleteSchedule() {
    var scheduleId = $("#AddSchedule").find("[name='ScheduleId']").val();
    
    if (!scheduleId || scheduleId === '') {
        // Use Bootstrap toast or alert for better UX
        showNotification('No schedule selected to delete', 'warning');
        return;
    }
    
    // Store schedule ID for confirmation
    $('#deleteConfirmModal').data('schedule-id', scheduleId);
    
    // Show confirmation modal
    var deleteModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
    deleteModal.show();
}

// Confirm Delete - Actually performs the deletion
function ConfirmDeleteSchedule() {
    var scheduleId = $('#deleteConfirmModal').data('schedule-id');
    
    if (!scheduleId) {
        showNotification('No schedule ID found', 'error');
        return;
    }
    
    // Disable delete button during request
    $('#btnConfirmDelete').prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>Deleting...');
    
    $.ajax({
        type: 'POST',
        url: '/Guests/DeleteGuestSchedule',
        contentType: 'application/json',
        data: JSON.stringify({ Id: parseInt(scheduleId) }),
        success: function(response) {
            // Close both modals
            var deleteModal = bootstrap.Modal.getInstance(document.getElementById('deleteConfirmModal'));
            if (deleteModal) {
                deleteModal.hide();
            }
            $('#crtevents').modal('hide');
            
            // Show success notification
            showNotification('Schedule deleted successfully', 'success');
            
            // Refresh calendar if visible
            if (window.globalCalendarInstance) {
                window.globalCalendarInstance.refetchEvents();
            }
            
            // Refresh list view if visible
            if ($('#listViewContainer').is(':visible') && window.SchedulesListView) {
                window.SchedulesListView.loadAndRender();
            }
            
            // Reset delete button
            $('#btnConfirmDelete').prop('disabled', false).html('<i class="bi bi-trash"></i> Delete');
        },
        error: function(error) {
            console.error('Failed to delete schedule:', error);
            var errorMessage = 'Failed to delete schedule. Please try again.';
            if (error.responseJSON && error.responseJSON.message) {
                errorMessage = error.responseJSON.message;
            }
            showNotification(errorMessage, 'error');
            
            // Reset delete button
            $('#btnConfirmDelete').prop('disabled', false).html('<i class="bi bi-trash"></i> Delete');
        }
    });
}

// Show notification (using Bootstrap toast if available, otherwise alert)
// Expose to global scope for use in other modules
window.showNotification = function(message, type) {
    // Try to use Bootstrap toast if available
    if (typeof bootstrap !== 'undefined' && bootstrap.Toast) {
        // Create toast element if it doesn't exist
        if ($('#notificationToast').length === 0) {
            $('body').append(`
                <div class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 9999;">
                    <div id="notificationToast" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
                        <div class="toast-header">
                            <strong class="me-auto">Notification</strong>
                            <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                        </div>
                        <div class="toast-body"></div>
                    </div>
                </div>
            `);
        }
        
        var bgClass = 'bg-primary';
        if (type === 'success') bgClass = 'bg-success';
        else if (type === 'error' || type === 'danger') bgClass = 'bg-danger';
        else if (type === 'warning') bgClass = 'bg-warning';
        
        $('#notificationToast').removeClass('bg-primary bg-success bg-danger bg-warning').addClass(bgClass);
        $('#notificationToast .toast-body').text(message);
        
        var toast = new bootstrap.Toast(document.getElementById('notificationToast'), {
            autohide: true,
            delay: 3000
        });
        toast.show();
    } else {
        // Fallback to alert
        alert(message);
    }
};

// Confirm Delete from Day Events Modal
function confirmDeleteFromDayEventsModal(eventId) {
    if (!eventId) {
        if (typeof showNotification === 'function') {
            showNotification('No schedule ID found', 'error');
        } else {
            alert('No schedule ID found');
        }
        return;
    }
    
    // Disable delete button during request
    $('#btnConfirmDelete').prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-1"></span>Deleting...');
    
    $.ajax({
        type: 'POST',
        url: '/Guests/DeleteGuestSchedule',
        contentType: 'application/json',
        data: JSON.stringify({ Id: parseInt(eventId) }),
        success: function(response) {
            // Close the modal
            var deleteModal = bootstrap.Modal.getInstance(document.getElementById('deleteConfirmModal'));
            if (deleteModal) {
                deleteModal.hide();
            }
            
            // Show success notification
            if (typeof showNotification === 'function') {
                showNotification('Schedule deleted successfully', 'success');
            } else {
                alert('Schedule deleted successfully');
            }
            
            // Refresh calendar if visible
            if (window.globalCalendarInstance) {
                window.globalCalendarInstance.refetchEvents();
            }
            
            // Refresh list view if visible
            if ($('#listViewContainer').is(':visible') && window.SchedulesListView) {
                window.SchedulesListView.loadAndRender();
            }
            
            // Reset delete button
            $('#btnConfirmDelete').prop('disabled', false).html('<i class="bi bi-trash"></i> Delete');
            $('#btnConfirmDelete').off('click.dayevents');
        },
        error: function(error) {
            console.error('Failed to delete schedule:', error);
            var errorMessage = 'Failed to delete schedule. Please try again.';
            if (error.responseJSON && error.responseJSON.message) {
                errorMessage = error.responseJSON.message;
            }
            
            if (typeof showNotification === 'function') {
                showNotification(errorMessage, 'error');
            } else {
                alert(errorMessage);
            }
            
            // Reset delete button
            $('#btnConfirmDelete').prop('disabled', false).html('<i class="bi bi-trash"></i> Delete');
            $('#btnConfirmDelete').off('click.dayevents');
        }
    });
}

// Initialize delete confirmation button handler
$(document).ready(function() {
    // Default handler for modal-based delete (from event edit modal)
    $('#btnConfirmDelete').on('click.modal', function() {
        var source = $('#deleteConfirmModal').data('source');
        // Only handle if not from list view or day events modal (they have their own handlers)
        if (source !== 'listview' && source !== 'dayeventsmodal') {
            ConfirmDeleteSchedule();
        }
    });
    
    // Clean up when modal is hidden
    $('#deleteConfirmModal').on('hidden.bs.modal', function() {
        $('#btnConfirmDelete').off('click.listview');
        $('#btnConfirmDelete').off('click.dayevents');
        $('#deleteConfirmModal').removeData('source');
        $('#deleteConfirmModal').removeData('schedule-id');
    });
});
