$(document).ready(function () {
    // Note: Settlement is now displayed inside BillingModal, cleanup handled in backToBilling()
});

// Helper function to parse formatted numbers (remove commas)
function parseFormattedNumber(value) {
    if (!value || value === '') return 0;
    // Remove commas and parse as float
    return parseFloat(value.toString().replace(/,/g, '')) || 0;
}

function initSettlementAttributes() {
    CalculateSettlementBalance();

}

function CalculateSettlementBalance() {

    let invoicedamountText = $("#tblSettlementAttributes").find(".invoicedamount").text().trim() || "0";
    let paymentcollectedText = $("#tblSettlementAttributes").find(".paymentcollected").text().trim() || "0";

    let invoicedamount = parseFormattedNumber(invoicedamountText);
    let paymentcollected = parseFormattedNumber(paymentcollectedText);

    let balance = invoicedamount - paymentcollected;
    let refundValue = -balance;

    refundValue = refundValue < 0 ? 0 : refundValue;

    // Format the balance with commas for display
    $("#tblSettlementAttributes").find("tbody tr").find("td.balance").text(balance.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));

    $("#tblSettlementAttributes").find("tbody tr").find("td.refund input").val(refundValue.toFixed(2));

    let refund = $("#tblSettlementAttributes").find("tbody tr").find("td.refund input").val();
    let creditamount = $("#tblSettlementAttributes").find("tbody tr").find("td.creditamount input").val();
    let debitamount = $("#tblSettlementAttributes").find("tbody tr").find("td.debitamount input").val();

    // Show/hide credit note fields based on balance
    if (balance < 0) {
        // Payment > Invoice: Show refund and credit note option, hide debit note
        $("#tblSettlementAttributes").find("tbody tr.refund-row").show();
        $("#tblSettlementAttributes").find("tbody tr").find("td.creditamount").closest("tr").show();
        $("#tblSettlementAttributes").find("tbody tr").find("td.debitamount").closest("tr").hide();
    } else if (balance > 0) {
        // Payment < Invoice: Show debit note option, hide refund and credit note
        $("#tblSettlementAttributes").find("tbody tr.refund-row").hide();
        $("#tblSettlementAttributes").find("tbody tr").find("td.creditamount").closest("tr").hide();
        $("#tblSettlementAttributes").find("tbody tr").find("td.debitamount").closest("tr").show();
        // Hide refund remarks section when refund is not applicable
        $("#settlementModalContent").find(".inputrefund").hide();
        // Clear debit amount input - user must fill it manually
        $("#tblSettlementAttributes").find("tbody tr").find("td.debitamount input").val(0);
        // Generate debit note number only if not already set from server
        let existingDebitNoteNumber = $("#settlementModalContent").find("[name='DebitNoteNumber']").val();
        if (!existingDebitNoteNumber || existingDebitNoteNumber.trim() === "") {
            generateDebitNoteNumber();
        }
    } else {
        // Balance = 0: Hide all (refund, credit note, debit note)
        $("#tblSettlementAttributes").find("tbody tr.refund-row").hide();
        $("#tblSettlementAttributes").find("tbody tr").find("td.creditamount").closest("tr").hide();
        $("#tblSettlementAttributes").find("tbody tr").find("td.debitamount").closest("tr").hide();
        // Hide refund remarks section when refund is not applicable
        $("#settlementModalContent").find(".inputrefund").hide();
    }

    // Only show refund remarks if refund is applicable (balance < 0) and refund amount > 0
    $("#tblSettlementAttributes").find("tbody tr").find("td.refund input").off("change").on("change input", function () {
        let currentRefund = parseFloat($(this).val()) || 0;
        if (balance < 0 && currentRefund > 0) {
            $("#settlementModalContent").find(".inputrefund").show();
        } else {
            $("#settlementModalContent").find(".inputrefund").hide();
        }
    }).trigger("change");
    $("#tblSettlementAttributes").find("tbody tr").find("td.creditamount input").off("change").on("change input", function () {
        let currentCredit = parseFloat($(this).val()) || 0;
        if (currentCredit !== 0) {
            $("#settlementModalContent").find(".inputcreditamount").show();
        } else {
            $("#settlementModalContent").find(".inputcreditamount").hide();
        }
    }).trigger("change");
    $("#tblSettlementAttributes").find("tbody tr").find("td.debitamount input").off("change").on("change input", function () {
        let currentDebit = parseFloat($(this).val()) || 0;
        if (currentDebit !== 0) {
            $("#settlementModalContent").find(".inputdebitamount").show();
            // Generate debit note number only if not already set from server
            let existingNumber = $("#settlementModalContent").find("[name='DebitNoteNumber']").val();
            if (!existingNumber || existingNumber.trim() === "") {
                generateDebitNoteNumber();
            }
            // Initialize date picker for estimated recovery date
            initializeDebitNoteDatePicker();
        } else {
            $("#settlementModalContent").find(".inputdebitamount").hide();
        }
    }).trigger("change");
}

function generateDebitNoteNumber() {
    // Generate debit note number in format: DN-YYYY-XXXXXX
    let year = new Date().getFullYear();
    let prefix = "DN-" + year + "-";
    
    // For now, generate a temporary number. The server will generate the actual number.
    // We'll use a timestamp-based approach for client-side preview
    let timestamp = Date.now().toString().slice(-6);
    let tempNumber = prefix + timestamp.padStart(6, '0');
    
    $("#settlementModalContent").find("[name='DebitNoteNumber']").val(tempNumber);
}

function initializeDebitNoteDatePicker() {
    var $dateInput = $("#settlementModalContent").find("[name='DebitNoteValidTill']");
    
    // Destroy existing datetimepicker if it exists
    if ($dateInput.data('xdsoft_datetimepicker')) {
        $dateInput.datetimepicker('destroy');
    }
    
    // Calculate default date: 90 days from now
    var defaultDate = new Date();
    defaultDate.setDate(defaultDate.getDate() + 90);
    
    // Format date as d-m-Y for display
    var day = String(defaultDate.getDate()).padStart(2, '0');
    var month = String(defaultDate.getMonth() + 1).padStart(2, '0');
    var year = defaultDate.getFullYear();
    var defaultDateString = day + '-' + month + '-' + year;
    
    // Set default value
    $dateInput.val(defaultDateString);
    
    // Initialize datetimepicker
    $dateInput.datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
        minDate: new Date(), // Prevent selecting past dates
        value: defaultDateString // Set default value
    });
}
function InitiateSettlementAndSave() {
    // Perform settlement validation
    let balanceText = $("#tblSettlementAttributes").find(".balance").text().trim() || "0";
    let balance = parseFormattedNumber(balanceText);
    let refund = parseFloat($("#tblSettlementAttributes").find("td.refund input").val()) || 0;
    let credit = parseFloat($("#tblSettlementAttributes").find("td.creditamount input").val()) || 0;
    let debit = parseFloat($("#tblSettlementAttributes").find("td.debitamount input").val()) || 0;

    // Validate debit note fields if debit amount > 0
    if (debit > 0) {
        let debitNoteNumber = $("#settlementModalContent").find("[name='DebitNoteNumber']").val() || "";
        let debitNoteDate = $("#settlementModalContent").find("[name='DebitNoteValidTill']").val() || "";
        
        if (!debitNoteNumber || debitNoteNumber.trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Debit Note Number is required when Debit Note Amount is greater than 0.'
            });
            return;
        }
        
        if (!debitNoteDate || debitNoteDate.trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Validation Error',
                text: 'Estimated Recovery Date is required when Debit Note Amount is greater than 0.'
            });
            return;
        }
    }

    // For debit notes: balance + refund + credit - debit = 0
    // For credit notes: balance + refund + credit = 0
    let settlementSum = balance + refund + credit - debit;

    if (Math.abs(settlementSum) > 0.01) {
        // Build a meaningful error message
        let message = 'Cannot complete settlement. The accounts are not balanced.\n\n';
        message += 'Current Status:\n';
        message += '• Balance: ' + balance.toFixed(2) + '\n';
        message += '• Refund: ' + refund.toFixed(2) + '\n';
        message += '• Credit Note: ' + credit.toFixed(2) + '\n';
        message += '• Debit Note: ' + debit.toFixed(2) + '\n';
        message += '• Total: ' + settlementSum.toFixed(2) + '\n\n';
        
        if (settlementSum > 0) {
            message += 'To complete settlement, you need to:\n';
            message += '• Add a Debit Note of ' + Math.abs(settlementSum).toFixed(2) + ', OR\n';
            message += '• Add additional payment of ' + Math.abs(settlementSum).toFixed(2);
        } else {
            message += 'To complete settlement, you need to:\n';
            message += '• Add a Refund of ' + Math.abs(settlementSum).toFixed(2) + ', OR\n';
            message += '• Adjust the Credit Note or Debit Note amount';
        }
        
        message += '\n\nNote: Settlement requires Balance + Refund + Credit - Debit = 0';
        
        Swal.fire({
            icon: 'error',
            title: 'Settlement Cannot Be Completed',
            html: message.replace(/\n/g, '<br>'),
            width: '600px'
        });
        return;
    }
    
    // If debit note is being created, show different confirmation message
    let confirmationMessage = "This will not be modified!";
    if (debit > 0) {
        confirmationMessage = "A Debit Note will be created and sent for approval. Settlement will be partially complete until the debit note is approved and recovered.";
    }

    Swal.fire({ title: 'Are you sure?', text: confirmationMessage, icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, save it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            let validTill = null;
            let validTillRaw = $("#settlementModalContent").find("[name='ValidTill']").val();
            if (validTillRaw) {
                let parsed = moment(validTillRaw, "DD-MMM-YYYY HH:mm");
                if (parsed.isValid()) {
                    validTill = parsed.format("YYYY-MM-DDTHH:mm:ss"); // Standard DB format
                }
            }

            // Get debit note estimated recovery date
            let debitNoteValidTill = null;
            let $debitNoteDateInput = $("#settlementModalContent").find("[name='DebitNoteValidTill']");
            let debitNoteDateRaw = $debitNoteDateInput.val();
            if (debitNoteDateRaw) {
                // Try to get the date from datetimepicker if available
                var datePicker = $debitNoteDateInput.data('xdsoft_datetimepicker');
                if (datePicker && datePicker.selectedDate) {
                    var selectedDate = datePicker.selectedDate;
                    var year = selectedDate.getFullYear();
                    var month = String(selectedDate.getMonth() + 1).padStart(2, '0');
                    var day = String(selectedDate.getDate()).padStart(2, '0');
                    debitNoteValidTill = year + '-' + month + '-' + day + 'T00:00:00';
                } else {
                    // Fallback: parse the string format d-m-Y
                    var dateParts = debitNoteDateRaw.split('-');
                    if (dateParts.length === 3) {
                        // Format: d-m-Y to yyyy-MM-dd
                        debitNoteValidTill = dateParts[2] + '-' + dateParts[1].padStart(2, '0') + '-' + dateParts[0].padStart(2, '0') + 'T00:00:00';
                    }
                }
            }

            BlockUI();
            var inputDTO = {};
            inputDTO.GuestId = $("#settlementModalContent").find("[name='GuestId']").val() || 0;
            inputDTO.GuestIdPaxSN1 = $("#settlementModalContent").find("[name='GuestIdPaxSN1']").val() || 0;
            inputDTO.InvoicedAmount = parseFormattedNumber($("#tblSettlementAttributes").find("td.invoicedamount").text().trim());
            inputDTO.PaymentCollected = parseFormattedNumber($("#tblSettlementAttributes").find("td.paymentcollected").text().trim());
            inputDTO.Balance = parseFormattedNumber($("#tblSettlementAttributes").find("td.balance").text().trim());
            inputDTO.Refund = parseFloat($("#tblSettlementAttributes").find("td.refund").find("input").val()) || 0;
            inputDTO.CreditAmount = parseFloat($("#tblSettlementAttributes").find("td.creditamount").find("input").val()) || 0;
            inputDTO.DebitAmount = parseFloat($("#tblSettlementAttributes").find("td.debitamount").find("input").val()) || 0;
            inputDTO.RefundRemarks = $("#settlementModalContent").find("[name='RefundRemarks']").val() || "";
            inputDTO.NoteNumber = $("#settlementModalContent").find("[name='NoteNumber']").val() || "";
            inputDTO.DebitNoteNumber = $("#settlementModalContent").find("[name='DebitNoteNumber']").val() || "";
            inputDTO.ValidTill = validTill;
            inputDTO.DebitNoteValidTill = debitNoteValidTill;



            $.ajax({
                type: "POST",
                url: "/Guests/SaveSettlementInformation",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    if (inputDTO.DebitAmount > 0) {
                        $successalert("", "Settlement saved successfully! Debit Note has been sent for approval. Settlement will be completed once the debit note is approved and recovered.");
                    } else {
                        $successalert("", "Saved Successful!");
                    }
                    
                    // Switch back to billing view
                    backToBilling();
                    
                    // Refresh payment data
                    let guestId = $("#settlementModalContent").find("[name='GuestId']").val() || 0;
                    PaymentPartialView(guestId, "payment");
                    
                    // Reload billing data after a short delay
                    setTimeout(function() {
                        BillingPartialView(guestId);
                    }, 500);
                },
                error: function (error) {
                    UnblockUI();
                    $erroralert("Transaction Failed!", error.responseText + '!');
                }
            });
        }
    });
}
