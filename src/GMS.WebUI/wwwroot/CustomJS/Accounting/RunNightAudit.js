let auditPollingInterval = null;
let initialRunDate = null;
let auditStartTime = null;

function RunAudit() {
    Swal.fire({ 
        title: 'Are you sure?', 
        text: "You want to run the audit!", 
        icon: 'warning', 
        showCancelButton: true, 
        confirmButtonText: 'Yes, run it!', 
        customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, 
        buttonsStyling: false 
    }).then(function (result) {
        if (result.value) {
            startAudit();
        }
    });
}

function startAudit() {
    var btn = document.querySelector('.btn-runaudit');
    if (!btn) {
        $erroralert("Error", "Button not found. Please refresh the page.");
        return;
    }

    // Update button immediately - BEFORE the AJAX call
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border me-2" role="status" aria-hidden="true"></span>Running...';
    btn.classList.add('disabled', 'btn-running');
    
    // Force a reflow to ensure the button updates visually
    btn.offsetHeight;

    // Record the start time to ensure we don't trigger completion too early
    auditStartTime = new Date().getTime();
    initialRunDate = null; // Reset initial run date

    var inputDTO = {};
    $.ajax({
        type: "POST",
        url: "/RunAudit/RunNightAudit",
        contentType: 'application/json',
        dataType: 'json', // Explicitly expect JSON response
        data: JSON.stringify(inputDTO),
        success: function (response) {
            // Show "Audit started" message immediately
            $successalert("", "Audit Started Successfully!");
            
            // Parse response - handle both direct object and wrapped responses
            var responseData = response;
            if (typeof response === 'string') {
                try {
                    responseData = JSON.parse(response);
                } catch (e) {
                    responseData = null;
                }
            }
            
            // Store initial run date for comparison
            // Handle both camelCase and PascalCase property names
            if (responseData) {
                initialRunDate = responseData.initialRunDate || responseData.InitialRunDate || null;
            }
            
            // Always get initial status from status endpoint to ensure accuracy
            // This ensures we have the most up-to-date status before polling
            getInitialAuditStatus(function() {
                // Start polling after getting initial status
                // Add a delay to ensure stored procedure has started and initial status is captured
                setTimeout(function() {
                    startPolling();
                }, 3000); // Increased delay to 3 seconds to ensure stored procedure has started
            });
        },
        error: function (error) {
            resetButton();
            var errorMessage = 'Failed to start audit!';
            if (error.responseJSON && error.responseJSON.message) {
                errorMessage = error.responseJSON.message;
            } else if (error.responseText) {
                errorMessage = error.responseText;
            }
            $erroralert("Transaction Failed!", errorMessage);
        }
    });
}

function getInitialAuditStatus(callback) {
    $.ajax({
        type: "GET",
        url: "/RunAudit/GetAuditStatus",
        success: function (data) {
            // Handle both camelCase and PascalCase property names
            var runDate = data ? (data.runDate || data.RunDate) : null;
            // Only update if we don't already have a value from the response
            if (initialRunDate === null || initialRunDate === undefined) {
                initialRunDate = runDate;
            }
            if (callback) callback();
        },
        error: function (error) {
            if (initialRunDate === null || initialRunDate === undefined) {
                initialRunDate = null;
            }
            if (callback) callback();
        }
    });
}

function startPolling() {
    // Clear any existing polling
    if (auditPollingInterval) {
        clearInterval(auditPollingInterval);
    }

    // Poll every 3 seconds
    auditPollingInterval = setInterval(function () {
        checkAuditStatus();
    }, 3000);
}

function checkAuditStatus() {
    // Ensure at least 5 seconds have passed since audit started
    // This prevents false positives from immediate status checks
    var timeSinceStart = new Date().getTime() - auditStartTime;
    if (timeSinceStart < 5000) {
        return; // Too early to check, skip this poll
    }

    $.ajax({
        type: "GET",
        url: "/RunAudit/GetAuditStatus",
        success: function (data) {
            // Handle both camelCase and PascalCase property names
            var currentRunDate = data ? (data.runDate || data.RunDate) : null;
            
            // Normalize dates for comparison
            var initialDateStr = null;
            var currentDateStr = null;
            
            if (initialRunDate) {
                try {
                    initialDateStr = new Date(initialRunDate).toISOString();
                } catch (e) {
                    initialDateStr = initialRunDate.toString();
                }
            }
            
            if (currentRunDate) {
                try {
                    currentDateStr = new Date(currentRunDate).toISOString();
                } catch (e) {
                    currentDateStr = currentRunDate.toString();
                }
            }
            
            // Check if audit has completed (RunDate has changed)
            // Only trigger if:
            // 1. Current date exists
            // 2. Current date is different from initial date
            // 3. At least 5 seconds have passed since start
            if (currentDateStr && initialDateStr !== null && initialDateStr !== currentDateStr) {
                // Audit completed!
                stopPolling();
                resetButton();
                $successalert("", "Audit Completed Successfully!");
                
                // Reload the page to show updated data
                setTimeout(function() {
                    window.location.reload();
                }, 2000);
            }
            // If initialRunDate was null and currentRunDate is still null, continue polling
            // If initialRunDate was null and currentRunDate now has a value, that means audit completed
            else if (initialDateStr === null && currentDateStr !== null && timeSinceStart > 10000) {
                // This handles the case where there was no previous audit
                // Wait at least 10 seconds before considering it complete
                stopPolling();
                resetButton();
                $successalert("", "Audit Completed Successfully!");
                
                setTimeout(function() {
                    window.location.reload();
                }, 2000);
            }
        },
        error: function (error) {
            // Continue polling even on error
        }
    });
}

function stopPolling() {
    if (auditPollingInterval) {
        clearInterval(auditPollingInterval);
        auditPollingInterval = null;
    }
}

function resetButton() {
    var btn = document.querySelector('.btn-runaudit');
    if (btn) {
        btn.disabled = false;
        btn.innerHTML = 'Run Audit';
        btn.classList.remove('disabled', 'btn-running');
    }
}

// Clean up polling when page unloads
window.addEventListener('beforeunload', function() {
    stopPolling();
});