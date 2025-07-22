function CopyRates() {
    if (!validateCopyRatesInputs()) {
        return; // If validation fails, exit early
    }

    let PlanIdTo = $("[name='SelectPlanDiv']").find("select[name='ChannelId']").val();
    let FromDate = $("#fromDate").val();
    let ToDate = $("#toDate").val();
    let PlanIdFrom = $("[name='CopyFromDiv']").find("select[name='CopyFrom']").val();


    let inputDTO = {};
    inputDTO.PlanIdTo = PlanIdTo;
    inputDTO.FromDate = FromDate;
    inputDTO.ToDate = ToDate;
    inputDTO.PlanIdFrom = PlanIdFrom;
    $.ajax({
        type: "POST",
        url: "/Rooms/CopyRoomRateBulk",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $successalert("", "Copied Successfully!");
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
        }
    });


    //alert(SelectedPlanId + ", " + fromDate + ", " + toDate + "," + CopyFromPlanId);

}


function validateCopyRatesInputs() {
    let SelectedPlanId = $("[name='SelectPlanDiv']").find("select[name='ChannelId']").val();
    let fromDate = $("#fromDate").val();
    let toDate = $("#toDate").val();
    let CopyFromPlanId = $("[name='CopyFromDiv']").find("select[name='CopyFrom']").val();

    if (!SelectedPlanId || SelectedPlanId === "0") {        
        $erroralert("", "Please select a target plan.");
        return false;
    }

    if (!fromDate) {        
        $erroralert("", "Please select the 'From Date'.");
        return false;
    }

    if (!toDate) {        
        $erroralert("", "Please select the 'To Date");
        return false;
    }

    if (new Date(fromDate) > new Date(toDate)) {        
        $erroralert("", "'From Date' cannot be after 'To Date'");
        return false;
    }

    if (!CopyFromPlanId || CopyFromPlanId === "0") {        
        $erroralert("", "Please select a source plan to copy from.");
        return false;
    }

    if (SelectedPlanId === CopyFromPlanId) {        
        $erroralert("", "Cannot copy rates from the same plan.");
        return false;
    }

    return true; // All checks passed
}