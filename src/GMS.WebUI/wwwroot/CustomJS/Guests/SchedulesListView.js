/**
 * SchedulesListView.js
 * Handles List View rendering for Guest Schedules
 * Groups events by date and displays in table format
 */

(function() {
    'use strict';

    var SchedulesListView = {
        // Render list view from events array
        render: function(events) {
            if (!events || events.length === 0) {
                $('#schedulesListTableBody').html('<tr><td colspan="9" class="text-center">No schedules found</td></tr>');
                return;
            }

            // Group events by date
            var groupedEvents = this.groupByDate(events);
            var html = '';

            // Sort dates
            var sortedDates = Object.keys(groupedEvents).sort(function(a, b) {
                return moment(a).diff(moment(b));
            });

            $.each(sortedDates, function(index, date) {
                var dateEvents = groupedEvents[date];
                $.each(dateEvents, function(i, event) {
                    var startMoment = moment(event.start);
                    var endMoment = moment(event.end);
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

                    var timeRange = startMoment.format('HH:mm') + ' - ' + endMoment.format('HH:mm');
                    var taskName = event.extendedProps && event.extendedProps.taskName ? event.extendedProps.taskName : (event.title || 'N/A');
                    // Remove duration from title if present
                    taskName = taskName.replace(/\(\d+.*?\)/g, '').trim();
                    var resource = event.extendedProps && event.extendedProps.resourceName ? event.extendedProps.resourceName : '-';
                    var therapist1 = event.extendedProps && event.extendedProps.therapist1Name ? event.extendedProps.therapist1Name : '-';
                    var therapist2 = event.extendedProps && event.extendedProps.therapist2Name ? event.extendedProps.therapist2Name : '-';
                    var therapist3 = event.extendedProps && (event.extendedProps.therapist3Name || event.extendedProps.employeeName3) ? (event.extendedProps.therapist3Name || event.extendedProps.employeeName3) : '-';

                    // Show date only for first event of the day
                    var dateCell = i === 0 ? startMoment.format('DD-MMM-YYYY') : '';

                    var eventId = event.id || event.extendedProps?.id || '';
                    html += '<tr>';
                    html += '<td>' + dateCell + '</td>';
                    html += '<td>' + timeRange + '</td>';
                    html += '<td>' + this.escapeHtml(taskName) + '</td>';
                    html += '<td>' + durationText + '</td>';
                    html += '<td>' + this.escapeHtml(resource) + '</td>';
                    html += '<td>' + this.escapeHtml(therapist1) + '</td>';
                    html += '<td>' + this.escapeHtml(therapist2) + '</td>';
                    html += '<td>' + this.escapeHtml(therapist3) + '</td>';
                    html += '<td class="text-center">';
                    html += '<div class="btn-group btn-group-sm" role="group">';
                    html += '<button type="button" class="btn btn-sm btn-primary edit-schedule-btn" data-schedule-id="' + eventId + '" title="Edit Schedule">';
                    html += '<i class="bi bi-pencil"></i>';
                    html += '</button>';
                    html += '<button type="button" class="btn btn-sm btn-danger delete-schedule-btn" data-schedule-id="' + eventId + '" title="Delete Schedule">';
                    html += '<i class="bi bi-trash"></i>';
                    html += '</button>';
                    html += '</div>';
                    html += '</td>';
                    html += '</tr>';
                }.bind(this));
            }.bind(this));

            $('#schedulesListTableBody').html(html);
            
            // Attach delete and edit button handlers
            this.attachDeleteHandlers();
            this.attachEditHandlers();
        },
        
        // Attach edit button click handlers
        attachEditHandlers: function() {
            var self = this;
            $('.edit-schedule-btn').off('click').on('click', function() {
                var scheduleId = $(this).data('schedule-id');
                if (scheduleId) {
                    self.editSchedule(scheduleId);
                }
            });
        },
        
        // Attach delete button click handlers
        attachDeleteHandlers: function() {
            var self = this;
            $('.delete-schedule-btn').off('click').on('click', function() {
                var scheduleId = $(this).data('schedule-id');
                if (scheduleId) {
                    self.deleteSchedule(scheduleId);
                }
            });
        },
        
        // Edit schedule from list view
        editSchedule: function(scheduleId) {
            var self = this;
            
            if (!scheduleId) {
                if (typeof showNotification === 'function') {
                    showNotification('No schedule ID found', 'warning');
                } else {
                    alert('No schedule ID found');
                }
                return;
            }
            
            // Load event data and open edit modal
            var inputDTO = {
                "Id": scheduleId
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
        
        // Delete schedule - Shows confirmation modal
        deleteSchedule: function(scheduleId) {
            var self = this;
            
            if (!scheduleId) {
                if (typeof showNotification === 'function') {
                    showNotification('No schedule ID found', 'warning');
                } else {
                    alert('No schedule ID found');
                }
                return;
            }
            
            // Store schedule ID for confirmation
            $('#deleteConfirmModal').data('schedule-id', scheduleId);
            $('#deleteConfirmModal').data('source', 'listview'); // Mark as coming from list view
            
            // Show confirmation modal
            var deleteModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
            deleteModal.show();
            
            // Override the confirm button handler for list view
            $('#btnConfirmDelete').off('click.listview').on('click.listview', function() {
                self.confirmDeleteSchedule(scheduleId);
            });
        },
        
        // Confirm Delete - Actually performs the deletion from list view
        confirmDeleteSchedule: function(scheduleId) {
            var self = this;
            
            if (!scheduleId) {
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
                url: '/Guests/DeleteGuestSchedule',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ Id: scheduleId }),
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
                    
                    // Reload the list view
                    self.loadAndRender();
                    
                    // Also refresh calendar if it's visible
                    if (window.globalCalendarInstance) {
                        window.globalCalendarInstance.refetchEvents();
                    }
                    
                    // Reset delete button
                    $('#btnConfirmDelete').prop('disabled', false).html('<i class="bi bi-trash"></i> Delete');
                    $('#btnConfirmDelete').off('click.listview');
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
                    $('#btnConfirmDelete').off('click.listview');
                }
            });
        },

        // Group events by date
        groupByDate: function(events) {
            var grouped = {};
            
            $.each(events, function(i, event) {
                var dateKey = moment(event.start).format('YYYY-MM-DD');
                if (!grouped[dateKey]) {
                    grouped[dateKey] = [];
                }
                grouped[dateKey].push(event);
            });

            // Sort events within each date by time
            $.each(grouped, function(date, dateEvents) {
                dateEvents.sort(function(a, b) {
                    return moment(a.start).diff(moment(b.start));
                });
            });

            return grouped;
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
            return text.replace(/[&<>"']/g, function(m) { return map[m]; });
        },

        // Load and render schedules
        loadAndRender: function() {
            var guestId = $('#GuestSchedule_GuestId').val();
            if (!guestId) {
                $('#schedulesListTableBody').html('<tr><td colspan="9" class="text-center text-danger">Guest ID not found</td></tr>');
                return;
            }

            $('#schedulesListTableBody').html('<tr><td colspan="9" class="text-center">Loading schedules...</td></tr>');

            $.ajax({
                url: "/Guests/GetGuestsEventForCalender?GuestId=" + guestId,
                type: "GET",
                success: function(res) {
                    var events = [];
                    
                    $.each(res, function(i, data) {
                        var startMoment = moment(data.startDateTime);
                        var endMoment = moment(data.endDateTime);
                        
                        events.push({
                            id: data.id,
                            title: data.taskName || 'N/A',
                            start: data.startDateTime,
                            end: data.endDateTime,
                            color: data.colorCode,
                            extendedProps: {
                                taskId: data.taskId,
                                taskName: data.taskName || 'N/A',
                                resourceId: data.resourceId,
                                resourceName: data.resourceName || '-',
                                employeeId1: data.employeeId1,
                                therapist1Name: data.employeeName1 || '-',
                                employeeId2: data.employeeId2,
                                therapist2Name: data.employeeName2 || '-',
                                employeeId3: data.employeeId3,
                                therapist3Name: data.employeeName3 || '-',
                                employeeName3: data.employeeName3 || '-',
                                duration: data.duration
                            }
                        });
                    });

                    // Apply filters if available
                    if (window.SchedulesFilters) {
                        events = window.SchedulesFilters.applyFilters(events);
                    }

                    SchedulesListView.render(events);
                },
                error: function(error) {
                    console.error('Failed to load schedules:', error);
                    $('#schedulesListTableBody').html('<tr><td colspan="9" class="text-center text-danger">Failed to load schedules</td></tr>');
                }
            });
        }
    };

    // Expose to global scope
    window.SchedulesListView = SchedulesListView;

})();

