function setActive(status) {
    const $tabs = $(".guest-status-tabs .nav-link");
    if ($tabs.length) {
        $tabs.removeClass("active").attr("aria-selected", "false");
        const $targetTab = $tabs.filter(function () {
            return $(this).data("status") === status;
        });
        if ($targetTab.length) {
            $targetTab.addClass("active").attr("aria-selected", "true");
        }
    }

    $("#CreditNoteAccountStatus").val(status);
    ListPartialView();
}

function ListPartialView() {
    var status = $("#CreditNoteAccountStatus").val() || "Active";
    
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/CreditDebitNoteAccount/ListPartialView?status=' + status,
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ListPartial').html(data);
            
            // Initialize DataTable only if table exists
            var tableId = '#CreditNoteAccountTable';
            var $table = $(tableId);
            
            if ($table.length > 0) {
                // Destroy existing DataTable if it exists
                if ($.fn.DataTable.isDataTable(tableId)) {
                    $(tableId).DataTable().destroy();
                }
                
                // Check if table has data rows (not just the "No records found" row)
                var hasData = $table.find('tbody tr').length > 0 && 
                              !$table.find('tbody tr td.text-center.text-muted').length;
                
                if (hasData) {
                    $(tableId).DataTable({
                        "aaSorting": [],
                        "pageLength": 25,
                        "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                        layout: {
                            topStart: {
                                buttons: ['copy', 'csv', 'excel', 'pdf', 'print']
                            }
                        }
                    });
                }
            }
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function openUpdateValidityModal(id, code, currentValidity) {
    $('#CreditNoteId').val(id);
    $('#CreditNoteCode').val(code);
    
    // Convert date from yyyy-MM-dd to d-m-Y format for display
    var displayDate = '';
    if (currentValidity && currentValidity !== '') {
        var dateParts = currentValidity.split('-');
        if (dateParts.length === 3) {
            displayDate = dateParts[2] + '-' + dateParts[1] + '-' + dateParts[0];
        }
    }
    $('#NewCodeValidity').val(displayDate);
    
    // Initialize or reinitialize datetimepicker
    var $dateInput = $('#NewCodeValidity');
    if ($dateInput.data('xdsoft_datetimepicker')) {
        $dateInput.datetimepicker('destroy');
    }
    $dateInput.datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
        minDate: new Date() // Prevent selecting past dates
    });
    
    var modal = new bootstrap.Modal(document.getElementById('updateValidityModal'));
    modal.show();
}

function updateCodeValidity() {
    var id = $('#CreditNoteId').val();
    var $dateInput = $('#NewCodeValidity');
    var newValidity = $dateInput.val();
    
    if (!newValidity) {
        $erroralert("Validation Error", "Please select a validity date.");
        return;
    }
    
    // Convert date from d-m-Y format to yyyy-MM-dd format for API
    var apiDate = '';
    if (newValidity) {
        // Try to get the date from datetimepicker if available
        var datePicker = $dateInput.data('xdsoft_datetimepicker');
        if (datePicker && datePicker.selectedDate) {
            var selectedDate = datePicker.selectedDate;
            var year = selectedDate.getFullYear();
            var month = String(selectedDate.getMonth() + 1).padStart(2, '0');
            var day = String(selectedDate.getDate()).padStart(2, '0');
            apiDate = year + '-' + month + '-' + day;
        } else {
            // Fallback: parse the string format d-m-Y
            var dateParts = newValidity.split('-');
            if (dateParts.length === 3) {
                // Format: d-m-Y to yyyy-MM-dd
                apiDate = dateParts[2] + '-' + dateParts[1].padStart(2, '0') + '-' + dateParts[0].padStart(2, '0');
            }
        }
    }
    
    var inputDTO = {
        Id: parseInt(id),
        CodeValidity: apiDate
    };
    
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/CreditDebitNoteAccount/UpdateCodeValidity",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            if (data.success) {
                $successalert("", "Validity updated successfully!");
                $('#updateValidityModal').modal('hide');
                ListPartialView(); // Refresh the list
            } else {
                $erroralert("Error", data.message || "Failed to update validity.");
            }
        },
        error: function (error) {
            UnblockUI();
            $erroralert("Transaction Failed!", error.responseJSON?.message || error.responseText || 'An error occurred while updating validity.');
        }
    });
}

