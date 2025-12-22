/**
 * SchedulesFilters.js
 * Handles filter management for Guest Schedules
 * No global variables - uses module pattern
 */

(function() {
    'use strict';

    var SchedulesFilters = {
        // Initialize filters
        init: function() {
            this.setupDateFilters();
            this.loadTaskDropdown();
            this.setupFilterChangeHandlers();
        },

        // Set default date range from guest check-in/check-out
        setupDateFilters: function() {
            var checkinDate = $('#guestCheckinDate').val();
            var checkoutDate = $('#guestCheckoutDate').val();

            if (checkinDate) {
                var checkin = moment(checkinDate).format('YYYY-MM-DD');
                $('#filterFromDate').val(checkin);
            }

            if (checkoutDate) {
                var checkout = moment(checkoutDate).format('YYYY-MM-DD');
                $('#filterToDate').val(checkout);
            } else if (checkinDate) {
                // If no checkout, default to 30 days from check-in
                var defaultTo = moment(checkinDate).add(30, 'days').format('YYYY-MM-DD');
                $('#filterToDate').val(defaultTo);
            }
        },

        // Load task dropdown
        loadTaskDropdown: function() {
            $.ajax({
                type: "POST",
                url: "/Guests/GetTaskName",
                contentType: 'application/json',
                success: function(data) {
                    var $taskSelect = $('#filterTaskId');
                    $taskSelect.empty();
                    $taskSelect.append('<option value="0">All Tasks</option>');
                    if (data != null && data.length > 0) {
                        $.each(data, function(i, task) {
                            $taskSelect.append('<option value="' + task.id + '">' + task.taskName + '</option>');
                        });
                    }
                },
                error: function(error) {
                    console.error('Failed to load tasks:', error);
                }
            });
        },

        // Setup filter change handlers
        setupFilterChangeHandlers: function() {
            var self = this;
            
            // When task changes, load employees and resources
            $('#filterTaskId').on('change', function() {
                var taskId = $(this).val();
                if (taskId && taskId !== '0') {
                    self.loadEmployeesByTask(taskId);
                    self.loadResourcesByTask(taskId);
                } else {
                    $('#filterEmployeeId').html('<option value="0">All Healers</option>');
                    $('#filterResourceId').html('<option value="0">All Treatment Rooms</option>');
                }
            });
        },

        // Load employees by task
        loadEmployeesByTask: function(taskId) {
            var inputDTO = { "Id": parseInt(taskId) };
            $.ajax({
                type: "POST",
                url: "/Guests/GetEmployeeByTaskId",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function(data) {
                    var $employeeSelect = $('#filterEmployeeId');
                    $employeeSelect.empty();
                    $employeeSelect.append('<option value="0">All Healers</option>');
                    if (data != null && data.length > 0) {
                        $.each(data, function(i, emp) {
                            $employeeSelect.append('<option value="' + emp.employeeId + '">' + emp.employeeName + '</option>');
                        });
                    }
                },
                error: function(error) {
                    console.error('Failed to load employees:', error);
                }
            });
        },

        // Load resources by task
        loadResourcesByTask: function(taskId) {
            var inputDTO = { "Id": parseInt(taskId) };
            $.ajax({
                type: "POST",
                url: "/Guests/GetResourcesByTaskId",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function(data) {
                    var $resourceSelect = $('#filterResourceId');
                    $resourceSelect.empty();
                    $resourceSelect.append('<option value="0">All Treatment Rooms</option>');
                    if (data != null && data.length > 0) {
                        $.each(data, function(i, resource) {
                            $resourceSelect.append('<option value="' + resource.id + '">' + resource.resourceName + '</option>');
                        });
                    }
                },
                error: function(error) {
                    console.error('Failed to load resources:', error);
                }
            });
        },

        // Get current filter values
        getFilters: function() {
            return {
                fromDate: $('#filterFromDate').val() || null,
                toDate: $('#filterToDate').val() || null,
                taskId: $('#filterTaskId').val() || '0',
                employeeId: $('#filterEmployeeId').val() || '0',
                resourceId: $('#filterResourceId').val() || '0'
            };
        },

        // Apply filters to events array
        applyFilters: function(events) {
            var filters = this.getFilters();
            var filtered = events;

            // Filter by date range
            if (filters.fromDate) {
                var fromMoment = moment(filters.fromDate).startOf('day');
                filtered = filtered.filter(function(event) {
                    return moment(event.start).isSameOrAfter(fromMoment);
                });
            }

            if (filters.toDate) {
                var toMoment = moment(filters.toDate).endOf('day');
                filtered = filtered.filter(function(event) {
                    return moment(event.start).isSameOrBefore(toMoment);
                });
            }

            // Filter by task (if event has taskId property)
            if (filters.taskId !== '0') {
                filtered = filtered.filter(function(event) {
                    return event.extendedProps && event.extendedProps.taskId == filters.taskId;
                });
            }

            // Filter by employee (if event has employeeId property)
            if (filters.employeeId !== '0') {
                filtered = filtered.filter(function(event) {
                    return (event.extendedProps && 
                           (event.extendedProps.employeeId1 == filters.employeeId || 
                            event.extendedProps.employeeId2 == filters.employeeId));
                });
            }

            // Filter by resource (if event has resourceId property)
            if (filters.resourceId !== '0') {
                filtered = filtered.filter(function(event) {
                    return event.extendedProps && event.extendedProps.resourceId == filters.resourceId;
                });
            }

            return filtered;
        },

        // Build query string for print page
        buildQueryString: function() {
            var filters = this.getFilters();
            var params = [];
            
            if (filters.fromDate) params.push('from=' + encodeURIComponent(filters.fromDate));
            if (filters.toDate) params.push('to=' + encodeURIComponent(filters.toDate));
            if (filters.taskId !== '0') params.push('taskId=' + encodeURIComponent(filters.taskId));
            if (filters.employeeId !== '0') params.push('employeeId=' + encodeURIComponent(filters.employeeId));
            if (filters.resourceId !== '0') params.push('resourceId=' + encodeURIComponent(filters.resourceId));
            
            return params.length > 0 ? '?' + params.join('&') : '';
        }
    };

    // Initialize when DOM is ready
    $(document).ready(function() {
        SchedulesFilters.init();
    });

    // Expose to global scope for use by other scripts
    window.SchedulesFilters = SchedulesFilters;

})();

