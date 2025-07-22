$(document).ready(function () {



});

function initBillingAttributes() {
    CalculateAmountBilling();
    $("#tblBillingAttributes").find("tbody").find(".discount").find("input").on("propertychange change keyup paste input", function () {
        CalculateAmountBilling();
    });
    $("#tblBillingAttributes").find("tbody").find(".finaldiscount").find("input").on("propertychange change keyup paste input", function () {
        CalculateAmountBilling();
    });

    $("#tblBillingAttributes").find("tbody").find("tr.cal td select.selectDB").chosen({
        width: '100%'
    });
    $("#tblBillingAttributes").find("tbody").find("tr.cal").last().find("select").chosen({
        width: '100%'
    });
    $("#tblBillingAttributes").find("tbody").find("tr.cal").find("td.services select.selectAdded").change(function () {
        let $this = $(this);
        let price = $this.find("option:selected").attr('price');
        $this.closest("tr").find("td.price input").val(price);
        CalculateAmountBilling();
    });
    $("#tblBillingAttributes").find("tbody").find("tr.cal").last().find("td.services select.selectAdded").change(function () {
        let $this = $(this);
        let price = $this.find("option:selected").attr('price');
        $this.closest("tr").find("td.price input").val(price);
        CalculateAmountBilling();
    });

    $("#tblBillingAttributes").find("tbody").find("tr.cal").find("select.selectSystem").change(function () {
        let $this = $(this);
        let price = $this.find("option:selected").attr('price');
        $this.closest("tr").find("td.price").text(price);
        CalculateAmountBilling();
    });
    $("#tblBillingAttributes").find("tbody").find("tr.cal").last().find("select.selectSystem").change(function () {
        let $this = $(this);
        let price = $this.find("option:selected").attr('price');
        $this.closest("tr").find("td.price").text(price);
        CalculateAmountBilling();
    });

    $("#tblBillingAttributes").find("tbody").find("tr.cal td.guest select.selectDB").change(function () {
        let $this = $(this);
        $this.closest("tr").attr("guestid", $this.val());
    });
    $("#tblBillingAttributes").find("tbody").find("tr.cal").last().find("td.guest").find("select").change(function () {
        let $this = $(this);
        $this.closest("tr").attr("guestid", $this.val());
    });

    //$("#tblBillingAttributes").find("tbody").find(".price input").on("propertychange change keyup paste input", function () {
    //    CalculateAmountBilling();
    //});
    $("#tblBillingAttributes tbody").on("input", ".price input", function () {
        CalculateAmountBilling();
    });
    $("#tblBillingAttributes").find("tbody").find(".count input").on("propertychange change keyup paste input", function () {
        CalculateAmountBilling();
    });
}

function initPaymentAttributes() {
    $("#tblPaymentAttributes").find("tbody").find("tr.cal").last().find("td.mode select").chosen({
        width: '100%'
    });
    $("#tblPaymentAttributes").find("tbody").find("tr.cal").last().find("td.date input").datetimepicker({
        format: 'd-M-Y H:i',
        timepicker: true,
        step: 5
    });
}

function CalculateAmountBilling() {
    let grossAmount = 0;
    let CGST = 0;
    let SGST = 0;
    let IGST = 0;
    $("#tblBillingAttributes").find("tbody").find("tr.cal").each(function () {
        let currObj = $(this);

        let priceText = currObj.find(".price").text().trim();
        let priceInput = currObj.find(".price input").val();
        let price = parseFloat(priceInput || priceText) || 0;
        //let price = parseFloat(currObj.find(".price").text()) || 0;

        let countText = currObj.find(".count").text().trim();
        let countInput = currObj.find(".count input").val();
        let count = parseFloat(countInput || countText) || 0;
        //let count = parseFloat(currObj.find(".count").text()) || 0;

        let discountText = currObj.find(".discount").text().trim();
        let discountInput = currObj.find(".discount input").val();
        let discount = parseFloat(discountInput || discountText) || 0;
        //let discount = parseFloat(currObj.find(".discount").find("input").val()) || 0;

        let amount = (price - discount) * count;
        currObj.find(".amount").text(amount.toFixed(2));
        grossAmount = grossAmount + amount;
    });

    $("#tblBillingAttributes").find("tbody").find("tr.trroomcharges").each(function () {
        let currObj = $(this);
        let priceText = currObj.find(".price").text().trim();
        let priceInput = currObj.find(".price input").val();
        let price = parseFloat(priceInput || priceText) || 0;

        let countText = currObj.find(".count").text().trim();
        let countInput = currObj.find(".count input").val();
        let count = parseFloat(countInput || countText) || 0;

        let discountText = currObj.find(".discount").text().trim();
        let discountInput = currObj.find(".discount input").val();
        let discount = parseFloat(discountInput || discountText) || 0;

        let stateAttr = currObj.attr("state");
        let state = (stateAttr ? stateAttr.trim().toLowerCase() : "");
        let roomRate = (price - discount);
        let roomCharges = (price - discount) * count;

        if (roomRate >= 5000) {
            if (state == "haryana") {
                SGST = SGST + (parseFloat(roomCharges) * (2.5 / 100));
                CGST = CGST + (parseFloat(roomCharges) * (2.5 / 100));
            }
            else {
                IGST = IGST + (parseFloat(roomCharges) * (5 / 100));
            }
        }
    });



    $("#tblBillingAttributes").find("tbody").find(".grossamount").text(grossAmount.toFixed(2));
    let finaldiscount = parseFloat($("#tblBillingAttributes").find("tbody").find(".finaldiscount").find("input").val()) || 0;

    $("#tblBillingAttributes").find("tbody").find(".taxes_SGST").text(SGST.toFixed(2));
    $("#tblBillingAttributes").find("tbody").find(".taxes_CGST").text(CGST.toFixed(2));
    $("#tblBillingAttributes").find("tbody").find(".taxes_IGST").text(IGST.toFixed(2));

    let taxes = (SGST + CGST + IGST) || 0;
    let finaltotal = (grossAmount - finaldiscount) + taxes;
    $("#tblBillingAttributes").find("tbody").find(".finaltotal").text(finaltotal.toFixed(2));
    $("#tblBillingAttributes").find("tbody").find(".inwords").text('Indian Rupee ' + numberToWords(finaltotal.toFixed(0)) + ' Only');

    //let totalAmount = 0;
    //let totalTax = 0;
    //let totalDiscount = 0;
    //$(".billing-amount").each(function () {
    //    let amount = parseFloat($(this).val()) || 0;
    //    totalAmount += amount;
    //});
    //$(".billing-tax").each(function () {
    //    let tax = parseFloat($(this).val()) || 0;
    //    totalTax += tax;
    //});
    //$(".billing-discount").each(function () {
    //    let discount = parseFloat($(this).val()) || 0;
    //    totalDiscount += discount;
    //});
    //$("#totalAmount").text(totalAmount.toFixed(2));
    //$("#totalTax").text(totalTax.toFixed(2));
    //$("#totalDiscount").text(totalDiscount.toFixed(2));
}

function AddServiceForBilling() {
    let servicehtml = '<tr class="cal"><td class="services" >Customized Packages</td><td class="price">0</td><td class="count">1</td><td class="discount fc-parent"><input type="number" class="form-control-c" value="0" /></td><td class="amount"></td><td class="action" style="text-align:end"><i style="color:green" class="bi bi-check-lg"></i> &nbsp;<i style="color:red" class="bi bi-x-lg"></i></td></tr>';

    $("#tblBillingAttributes").find("tbody").find("tr.cal").last().after(servicehtml);
}
function AddServiceForBilling1() {
    let servicehtml = '<tr class="cal"><td class="services" >Customized Packages</td><td class="price">0</td><td class="count">1</td><td class="discount fc-parent"><input type="number" class="form-control-c" value="0" /></td><td class="amount"></td><td class="action" style="text-align:end"><i style="color:green" class="bi bi-check-lg"></i> &nbsp;<i style="color:red" class="bi bi-x-lg"></i></td></tr>';

    $("#tblBillingAttributes").find("tbody").find("tr.cal").last().after(servicehtml);
}

function GetPackagesForBilling(Id) {
    BlockUI();
    var inputDTO = {
        "Id": Id
    };
    $.ajax({
        type: "POST",
        url: "/Guests/GetPackagesForBilling",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (result) {
            let data = result.services;
            let packagesdrpdown = "<select class='selectAdded'>";
            packagesdrpdown += "<option value='0'>Select Package</option>";
            for (var i = 0; i < data.length; i++) {
                packagesdrpdown += "<option price='" + data[i].price + "' value='" + data[i].id + "'>" + data[i].service + "</option>";
            }
            packagesdrpdown += "</select>";

            let guestdrpdown = buildGuestDropdown(result.memberDetailsWithChildren);

            let packageshtml = '<tr guestid="0" id="pkg" class="cal bdb trpackages"><td class="services" >' + packagesdrpdown + '</td><td class="guest">' + guestdrpdown + '</td><td class="price text-right"><input type="number" class="form-control-c text-right" value="0" /></td><td class="count text-right"><input type="number" class="form-control-c text-right" value="1" /></td><td class="discount fc-parent text-right"><input type="number" class="form-control-c text-right" value="0" /></td><td class="amount text-right"></td><td class="action" style="text-align:end"><i style="color:green" class="bi bi-check-lg" onclick="ConfirmBillingRecord(this)"></i><i style="color:red" class="bi bi-x-lg" onclick="RemoveBillingRecord(this)"></i></td></tr>';
            $("#tblBillingAttributes").find("tbody").find("tr.cal").last().after(packageshtml);


            initBillingAttributes();
            UnblockUI();
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function buildGuestDropdown(data) {
    let drpdown = "<select class='selectAdded'>";
    drpdown += "<option value='0'>Select Guest</option>";
    for (var i = 0; i < data.length; i++) {
        drpdown += "<option value='" + data[i].id + "'>" + data[i].customerName + "</option>";
    }
    drpdown += "</select>";
    return drpdown;
}
function GetServicesForBilling(Id) {
    BlockUI();
    var inputDTO = {
        "Id": Id
    };
    $.ajax({
        type: "POST",
        url: "/Guests/GetServicesForBilling",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (result) {
            let data = result.tasks;
            let servicesdrpdown = "<select  class='selectAdded'>";
            servicesdrpdown += "<option value='0'>Select Service</option>";
            for (var i = 0; i < data.length; i++) {
                servicesdrpdown += "<option price='" + data[i].rate + "' value='" + data[i].id + "'>" + data[i].taskName + "</option>";
            }
            servicesdrpdown += "</select>";
            let guestdrpdown = buildGuestDropdown(result.memberDetailsWithChildren);
            let servicehtml = '<tr  guestid="0"  class="cal bdb trservices"><td class="services" >' + servicesdrpdown + '</td><td class="guest">' + guestdrpdown + '</td><td class="price text-right"><input type="number" class="form-control-c text-right" value="0" /></td><td class="count text-right"><input type="number" class="form-control-c text-right" value="1" /></td><td class="discount fc-parent text-right"><input type="number" class="form-control-c text-right" value="0" /></td><td class="amount text-right"></td><td class="action" style="text-align:end"><i style="color:green" class="bi bi-check-lg" onclick="ConfirmBillingRecord(this)"></i><i style="color:red" class="bi bi-x-lg"  onclick="RemoveBillingRecord(this)"></i></td></tr>';
            $("#tblBillingAttributes").find("tbody").find("tr.cal").last().after(servicehtml);

            initBillingAttributes();
            UnblockUI();
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function ValidateDiscountOnBilling() {
    var $table = $("#tblBillingAttributes");
    var $discountInput = $table.find('.finaldiscount input');
    var $discountCodeInput = $table.find('.finaldiscountcode input');
    var discountValue = parseFloat($discountInput.val()) || 0;
    var discountCode = $discountCodeInput.val().trim();

    // Remove existing error if any (within the table)
    $table.find('.finaldiscountcode .error-message').remove();

    // Bind input event to remove error when typing starts
    $discountCodeInput.off('input.validateDiscount').on('input.validateDiscount', function () {
        $table.find('.finaldiscountcode .error-message').remove();
    });

    if (discountValue > 0 && discountCode === '') {
        $table.find('.finaldiscountcode').append('<div class="error-message">Please enter a discount code.</div>');
        $discountCodeInput.focus();
        return false;
    }

    return true;
}



function SaveBillingData() {
    return new Promise((resolve, reject) => {
        let gGuestId = $("#BillingModal").find("[name='GuestId']").val()
        let billingList = [];
        $("#tblBillingAttributes tbody tr.bdb").each(function () {
            let $row = $(this);
            let serviceId
            if ($row.hasClass("trroomcharges")) {
                serviceId = 0;
                serviceType = 'RoomCharges';
            } else if ($row.hasClass("trservices")) {
                serviceId = parseInt($row.find(".selectAdded").val()) || null;
                serviceType = 'Service';
            } else if ($row.hasClass("trpackagesystem")) {
                serviceId = parseInt($row.find(".selectSystem").val()) || null;
                serviceType = 'PackageSystem';
            } else if ($row.hasClass("trpackages")) {
                serviceId = parseInt($row.find(".selectAdded").val()) || null;
                serviceType = 'Package';
            }

            let priceText = $row.find(".price").text().trim();
            let priceInput = $row.find(".price input").val();
            let price = parseFloat(priceInput || priceText) || 0;

            let countText = $row.find(".count").text().trim();
            let countInput = $row.find(".count input").val();
            let count = parseFloat(countInput || countText) || 0;

            let discountText = $row.find(".discount").text().trim();
            let discountInput = $row.find(".discount input").val();
            let discount = parseFloat(discountInput || discountText) || 0;

            let amountText = $row.find(".amount").text().trim();
            let amountInput = $row.find(".amount input").val();
            let amount = parseFloat(amountInput || amountText) || 0;

            let guestid = $row.attr("guestid") || 0;
            let recordid = $row.attr("recordid") || 0;
            // You can set GuestId dynamically from your current context
            billingList.push({
                Id: recordid,
                //GuestId: $("#BillingModal").find("[name='GuestId']").val(), // Replace this dynamically
                GuestId: guestid,
                ServiceId: serviceId,
                serviceType: serviceType,
                Price: price,
                Count: count,
                Discount: discount,
                TotalAmount: amount
            });
        });

        billingList.push({
            Id: 0,
            GuestId: $("#BillingModal").find("[name='GuestIdPaxSN1']").val(), // Replace this dynamically
            ServiceId: 0,
            serviceType: "GrossAmount",
            Price: $("#tblBillingAttributes").find("tbody").find(".grossamount").text().trim() || 0,
            Count: 0,
            Discount: parseFloat($("#tblBillingAttributes").find("tbody").find(".finaldiscount").find("input").val()) || 0,
            DiscountCode: $("#tblBillingAttributes").find("tbody").find(".finaldiscountcode").find("input").val(),
            IGST: parseFloat($("#tblBillingAttributes").find("tbody").find(".taxes_IGST").text().trim()) || 0,
            CGST: parseFloat($("#tblBillingAttributes").find("tbody").find(".taxes_CGST").text().trim()) || 0,
            SGST: parseFloat($("#tblBillingAttributes").find("tbody").find(".taxes_SGST").text().trim()) || 0,
            TotalAmount: $("#tblBillingAttributes").find("tbody").find(".finaltotal").text().trim() || 0
        });

        if (ValidateDiscountOnBilling() == false) {
            return false;
        }

        const inputDTO = {
            BillingDTOs: billingList
        };
        BlockUI();
        $.ajax({
            type: "POST",
            url: "/Guests/SaveBillingData",
            contentType: 'application/json',
            data: JSON.stringify(inputDTO),
            success: function (data) {
                UnblockUI();
                $successalert("", "Saved Successfully!");
                $("#BillingModal").find(".btn-close").click();
                BillingPartialView(gGuestId);
                resolve(gGuestId);
            },
            error: function (error) {
                $erroralert("Transaction Failed!", error.responseText + '!');
                UnblockUI();
                reject(gGuestId);
            }
        });
    });

}


function AddPaymentRowInTable() {

    BlockUI();
    var inputDTO = {
        "Id": 0
    };
    $.ajax({
        type: "POST",
        url: "/Guests/GetPaymentModesForBilling",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            let paymentModesDrpdown = "<select>";
            paymentModesDrpdown += "<option value='0'>Select Method</option>";
            for (var i = 0; i < data.length; i++) {
                paymentModesDrpdown += "<option value='" + data[i].id + "'>" + data[i].code + "</option>";
            }
            paymentModesDrpdown += "</select>";

            let paymentModesHtml = '<tr class="cal"><td class="mode">' + paymentModesDrpdown + '</td><td class="amount fc-parent"><input type="number" class="form-control-c" value="0" /></td><td class="date paymentdate fc-parent"><input type="text" class="form-control-c" /></td><td class="referencenumber fc-parent"><input type="text" class="form-control-c" /></td><td class="remarks fc-parent"><input type="text" class="form-control-c" /></td></tr>';
            $("#tblPaymentAttributes").find("tbody").find("tr.cal").last().after(paymentModesHtml);


            initBillingAttributes();
            UnblockUI();
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });


    //var lastRow = $('#tblPaymentAttributes tbody tr:last');
    //var newRow = lastRow.clone();

    //// Optional: clear input values
    //newRow.find('input').val('');

    //// Append new row
    //$('#tblPaymentAttributes tbody').append(newRow);
    //initBillingAttributes();
}

function SavePaymentInformation(currObj) {

    let $tr = currObj.closest("tr");

    let paymentMode = $tr.find("td select.mode").val() || null;
    let amount = $tr.find("td input.amount").val() || null;
    let date = $tr.find("td input.date").val() || null;
    let referencenumber = $tr.find("td input.referencenumber").val() || null;
    let remarks = $tr.find("td input.remarks").val() || null;


    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Guests/SaveBillingData",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $successalert("", "Saved Successfully!");
            $("#BillingModal").find(".btn-close").click();
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });

}


function SavePaymentInformation(currObj) {

    let $row = $(currObj).closest("tr");

    // Clear previous validation messages
    $row.find(".validation-msg").remove();

    let dto = PaymentInformationDTO($row);
    if (!dto) return; // validation failed

    Swal.fire({ title: 'Are you sure?', text: "This will not be modified!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, save it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = dto;
            inputDTO.GuestId = $("#BillingModal").find("[name='GuestId']").val() || 0;
            $.ajax({
                type: "POST",
                url: "/Guests/SavePaymentInformation",
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
function PaymentInformationDTO($row) {
    let isValid = true;

    let mode = $row.find('td.mode select').val();
    let amount = $row.find('td.amount input').val();
    let paymentDateRaw = $row.find('td.paymentdate input').val();
    let referenceNumber = $row.find('td.referencenumber input').val();
    let remarks = $row.find('td.remarks input').val();

    let paymentDate = null;

    // Format date using moment.js
    if (paymentDateRaw) {
        let parsed = moment(paymentDateRaw, "DD-MMM-YYYY HH:mm");
        if (parsed.isValid()) {
            paymentDate = parsed.format("YYYY-MM-DDTHH:mm:ss"); // Standard DB format
        } else {
            showValidationMsg($row.find('td.paymentdate'), "Invalid date/time format");
            isValid = false;
        }
    } else {
        showValidationMsg($row.find('td.paymentdate'), "Please enter payment date & time");
        isValid = false;
    }

    if (!mode || mode === "0") {
        showValidationMsg($row.find('td.mode'), "Please select a payment method");
        isValid = false;
    }

    if (!amount || isNaN(amount) || parseFloat(amount) <= 0) {
        showValidationMsg($row.find('td.amount'), "Enter a valid amount");
        isValid = false;
    }

    if (!isValid) return null;

    return {
        PaymentMode: parseInt(mode),
        Amount: parseFloat(amount),
        DateOfPayment: paymentDate,
        ReferenceNumber: referenceNumber,
        Comments: remarks
    };


}

function showValidationMsg($cell, message) {
    $cell.append(`<div class="validation-msg text-danger" style="font-size: 12px;">${message}</div>`);
}

function ConfirmBillingRecord(currObj) {
    let gGuestId = $("#BillingModal").find("[name='GuestId']").val()
    Swal.fire({ title: 'Are you sure?', text: "You won't be able to modify this!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, confirm this!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {





            let $row = $(currObj).closest('tr');
            let serviceId = 0;
            let serviceType = "";
            if ($row.hasClass("trroomcharges")) {
                serviceId = 0;
                serviceType = 'RoomCharges';
            } else if ($row.hasClass("trservices")) {
                serviceId = parseInt($row.find(".selectAdded").val()) || null;
                serviceType = 'Service';
            } else if ($row.hasClass("trpackagesystem")) {
                serviceId = parseInt($row.find(".selectSystem").val()) || null;
                serviceType = 'PackageSystem';
            } else if ($row.hasClass("trpackages")) {
                serviceId = parseInt($row.find(".selectAdded").val()) || null;
                serviceType = 'Package';
            }

            let priceText = $row.find(".price").text().trim();
            let priceInput = $row.find(".price input").val();
            let price = parseFloat(priceInput || priceText) || 0;

            let countText = $row.find(".count").text().trim();
            let countInput = $row.find(".count input").val();
            let count = parseFloat(countInput || countText) || 0;

            let discountText = $row.find(".discount").text().trim();
            let discountInput = $row.find(".discount input").val();
            let discount = parseFloat(discountInput || discountText) || 0;

            let amountText = $row.find(".amount").text().trim();
            let amountInput = $row.find(".amount input").val();
            let amount = parseFloat(amountInput || amountText) || 0;
            //let GuestId = $("#BillingModal").find("[name='GuestId']").val();
            let GuestId = $row.attr("guestid");
            let recordid = $row.attr("recordid") || 0;
            // You can set GuestId dynamically from your current context
            let inputDTO = {
                Id: recordid,
                GuestId: GuestId,//$("#BillingModal").find("[name='GuestId']").val(), // Replace this dynamically
                ServiceId: serviceId,
                serviceType: serviceType,
                Price: price,
                Count: count,
                Discount: discount,
                TotalAmount: amount
            };
            BlockUI();
            $.ajax({
                type: "POST",
                url: "/Guests/ConfirmAndSaveBillingData",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    $successalert("", "Saved Successfully!");
                    $row.find('td.action').empty();
                    $("#BillingModal").find(".btn-close").click();
                    BillingPartialView(gGuestId);
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}

function RemoveBillingRecord(currObj) {
    let gGuestId = $("#BillingModal").find("[name='GuestId']").val()
    Swal.fire({ title: 'Are you sure?', text: "You won't be able to modify this!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, confirm this!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            let $row = $(currObj).closest('tr');
            let recordid = $row.attr("recordid") || 0;

            if (parseInt(recordid) === 0) {
                $row.remove(); // Just remove the row from the DOM
            }
            else {
                let inputDTO = {
                    Id: recordid
                };
                BlockUI();
                $.ajax({
                    type: "POST",
                    url: "/Guests/RemoveRecordFromBillingData",
                    contentType: 'application/json',
                    data: JSON.stringify(inputDTO),
                    success: function (data) {
                        UnblockUI();
                        $successalert("", "Saved Successfully!");
                        $row.find('td.action').empty();
                        $("#BillingModal").find(".btn-close").click();
                        BillingPartialView(gGuestId);
                    },
                    error: function (error) {
                        $erroralert("Transaction Failed!", error.responseText + '!');
                        UnblockUI();
                    }
                });
            }
        }
    });
}


function printInvoice(guestId) {
    window.open("/Guests/PrintInvoice/" + guestId + "", "_blank");
}