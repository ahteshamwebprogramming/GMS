$(document).ready(function () {
    PopulateMonthYearDropdown();
    ListPartialView();

    $('#month, #year').on('change', function () {
        ListPartialView();
    });
});

function PopulateMonthYearDropdown() {
    const monthNames_D = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    const currentDate = new Date();
    const currentMonth_D = currentDate.getMonth() + 1;
    const currentYear_D = currentDate.getFullYear();

    // Populate month dropdown
    monthNames_D.forEach((month, index) => {
        const monthValue_D = index + 1;
        $('#month').append(
            `<option value="${monthValue_D}" ${monthValue_D === currentMonth_D ? 'selected' : ''}>${month}</option>`
        );
    });

    // Populate year dropdown (current year and previous 4 years)
    for (let i = 0; i < 5; i++) {
        const year_D = currentYear_D - i;
        $('#year').append(
            `<option value="${year_D}" ${year_D === currentYear_D ? 'selected' : ''}>${year_D}</option>`
        );
    }
}

function ListPartialView() {
    const selectedMonth = parseInt($('#month').val());
    const selectedYear = parseInt($('#year').val());
    let inputDTO = {};
    inputDTO.Month = selectedMonth;
    inputDTO.Year = selectedYear;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Payment/ListPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ListPartial').html(data);
            initPaymentAttributes();
            initCalculateDifference();
            //$("table").find("tbody").find("td select.selectchosen").chosen({
            //    width: '100%'
            //});

            $("table").DataTable({
                "aaSorting": [],
                layout: {
                    topStart: {
                        buttons: ['copy', 'csv', 'excel', 'pdf', 'print']
                    }
                }
            });
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}
function initPaymentAttributes() {
    $("#ListOfPayment").find("tbody").find(".amountreceived").find("input").on("propertychange change keyup paste input", function () {
        initCalculateDifference();
    });
}
function initCalculateDifference() {
    $("#ListOfPayment").find("tbody").find("tr.pending").each(function () {
        let currObj = $(this);

        let amountText = currObj.find(".amount").text().trim();
        let amountInput = currObj.find(".amount input").val();
        let amount = parseFloat(amountInput || amountText) || 0;

        let amountreceivedText = currObj.find(".amountreceived").text().trim();
        let amountreceivedInput = currObj.find(".amountreceived input").val();
        let amountreceived = parseFloat(amountreceivedInput || amountreceivedText) || 0;


        let differences = parseFloat(amount - amountreceived) || 0;

        currObj.find(".difference").text(differences.toFixed(2));

    });
}
function Approve(Id, currObj) {
    Swal.fire({ title: 'Are you sure?', text: "You won't be able to modify this!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, confirm this!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            let $row = $(currObj).closest('tr');

            let amountreceivedText = $row.find(".amountreceived").text().trim();
            let amountreceivedInput = $row.find(".amountreceived input").val();
            let amountreceived = parseFloat(amountreceivedInput || amountreceivedText) || 0;

            let differenceText = $row.find(".difference").text().trim();
            let differenceInput = $row.find(".difference input").val();
            let difference = parseFloat(differenceInput || differenceText) || 0;

            //let approvalcommentsText = $row.find(".approvalcomments").text().trim();
            let approvalcommentsInput = $row.find(".approvalcomments input").val();
            //let approvalcomments = (approvalcommentsInput || approvalcommentsText) || "";
            let approvalcomments = approvalcommentsInput;

            let inputDTO = {
                Id: Id,
                AmountReceived: amountreceived,
                Differences: difference,
                ApprovalComment: approvalcomments
            };
            BlockUI();
            $.ajax({
                type: "POST",
                url: "/Payment/ApprovePayment",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    $successalert("", "Saved Successfully!");
                    ListPartialView();
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}