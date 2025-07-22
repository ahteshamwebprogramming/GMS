$(document).ready(function () {



});

function initSettlementAttributes() {
    CalculateSettlementBalance();

}

function CalculateSettlementBalance() {


    let invoicedamount = $("#tblSettlementAttributes").find(".invoicedamount").text().trim() || 0;
    let paymentcollected = $("#tblSettlementAttributes").find(".paymentcollected").text().trim() || 0;

    let balance = invoicedamount - paymentcollected;
    let refundValue = -balance

    refundValue = refundValue < 0 ? 0 : refundValue;

    $("#tblSettlementAttributes").find("tbody tr").find("td.balance").text(balance.toFixed(2));

    $("#tblSettlementAttributes").find("tbody tr").find("td.refund input").val(refundValue.toFixed(2));

    let refund = $("#tblSettlementAttributes").find("tbody tr").find("td.refund input").val();
    let creditamount = $("#tblSettlementAttributes").find("tbody tr").find("td.creditamount input").val();

    $("#tblSettlementAttributes").find("tbody tr").find("td.refund input").off("change").on("change input", function () {
        let currentRefund = parseFloat($(this).val()) || 0;
        if (currentRefund !== 0) {
            $("#SettlementModal").find(".inputrefund").show();
        } else {
            $("#SettlementModal").find(".inputrefund").hide();
        }
    }).trigger("change");
    $("#tblSettlementAttributes").find("tbody tr").find("td.creditamount input").off("change").on("change input", function () {
        let currentCredit = parseFloat($(this).val()) || 0;
        if (currentCredit !== 0) {
            $("#SettlementModal").find(".inputcreditamount").show();
        } else {
            $("#SettlementModal").find(".inputcreditamount").hide();
        }
    }).trigger("change");
}
function InitiateSettlementAndSave() {

    // Perform settlement validation
    let balance = parseFloat($("#tblSettlementAttributes").find(".balance").text().trim()) || 0;
    let refund = parseFloat($("#tblSettlementAttributes").find("td.refund input").val()) || 0;
    let credit = parseFloat($("#tblSettlementAttributes").find("td.creditamount input").val()) || 0;

    let settlementSum = balance + refund + credit;

    if (Math.abs(settlementSum) > 0.01) {
        Swal.fire({
            icon: 'error',
            title: 'Invalid Settlement',
            text: 'Settlement is only possible when Balance + Refund + Credit = 0.'
        });
        return;
    }

    Swal.fire({ title: 'Are you sure?', text: "This will not be modified!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, save it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            let validTill = null;
            let validTillRaw = $("#SettlementModal").find("[name='ValidTill']").val();
            if (validTillRaw) {
                let parsed = moment(validTillRaw, "DD-MMM-YYYY HH:mm");
                if (parsed.isValid()) {
                    validTill = parsed.format("YYYY-MM-DDTHH:mm:ss"); // Standard DB format
                }
            }

            BlockUI();
            var inputDTO = {};
            inputDTO.GuestId = $("#SettlementModal").find("[name='GuestId']").val() || 0;
            inputDTO.GuestIdPaxSN1 = $("#SettlementModal").find("[name='GuestIdPaxSN1']").val() || 0;
            inputDTO.InvoicedAmount = parseFloat($("#tblSettlementAttributes").find("td.invoicedamount").text().trim()) || 0;
            inputDTO.PaymentCollected = parseFloat($("#tblSettlementAttributes").find("td.paymentcollected").text().trim()) || 0;
            inputDTO.Balance = parseFloat($("#tblSettlementAttributes").find("td.balance").text().trim()) || 0;
            inputDTO.Refund = parseFloat($("#tblSettlementAttributes").find("td.refund").find("input").val()) || 0;
            inputDTO.CreditAmount = parseFloat($("#tblSettlementAttributes").find("td.creditamount").find("input").val()) || 0;
            inputDTO.RefundRemarks = $("#SettlementModal").find("[name='RefundRemarks']").val() || "";
            inputDTO.NoteNumber = $("#SettlementModal").find("[name='NoteNumber']").val() || "";
            inputDTO.ValidTill = validTill;



            $.ajax({
                type: "POST",
                url: "/Guests/SaveSettlementInformation",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Saved Successful!");
                    PaymentPartialView($("#BillingModal").find("[name='GuestId']").val() || 0, "payment")
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
