function AddMoreGuestsInRooms(RNumber, GroupId, PaxSno) {
    $("#HiddenFields").find("[name='RoomNumber']").val(RNumber);
    $("#closeModalGuestsDetails").click();
    AddGuestsPartialView(GroupId, parseInt(PaxSno) + 1);
}
function AddGuestInRoomsEmpty(RNumber) {
    $("#HiddenFields").find("[name='RoomNumber']").val(RNumber);
    AddGuestsPartialView();
}

function AddGuestsPartialView(GroupId = '', PAXSno = 1) {

    let inputDTO = {};
    inputDTO.GroupId = GroupId;
    inputDTO.PAXSno = PAXSno;

    BlockUI();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/AddGuestsPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();

            $('#div_AddGuestsListPartial').html(data);
            $("#btnModelAddGuests").click();
            $("[name='MemberDetail.PAXSno']").val(PAXSno);
            $("[name='spnPAXSno']").text(PAXSno);
            initDates();
            getOnLoadAddGuests();
            $("[name='RoomAllocation.RNumber']").val($("#HiddenFields").find("[name='RoomNumber']").val());
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}
function initDates() {
    InitialiseMobileFields();
    $("#AddGuestForm").find("[name='MemberDetail.Dob']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#AddGuestForm").find("[name='MemberDetail.DateOfArrival']").datetimepicker({
        format: 'd-m-Y H:i',
        timepicker: true,
    });
    //$("#AddGuestForm").find("[name='MemberDetail.DateOfDepartment']").datetimepicker({
    //    format: 'd-m-Y',
    //    step: 30
    //});
    $("#AddGuestForm").find("[name='TimeOfDepartment']").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30 // Optional: Define minute intervals
    });
    $("#AddGuestForm").find("[name='MemberDetail.FlightArrivalDateAndDateTime']").datetimepicker({
        format: 'd-m-Y H:i',
        //timepicker: false,
    });
    $("#AddGuestForm").find("[name='MemberDetail.FlightDepartureDateAndDateTime']").datetimepicker({
        format: 'd-m-Y H:i',
        //timepicker: false,
    });
    $("#AddGuestForm").find("[name='MemberDetail.HoldTillDate']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#AddGuestForm").find("[name='MemberDetail.PaymentDate']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });

    $("#AddGuestForm").find("[name='MemberDetail.Dob']").change(function () {
        CalculateAge();
    });
    $('[name="MemberDetail.Pax"]').on("propertychange change keyup paste input", function () {
        $('[name="MemberDetail.NoOfRooms"]').val($('[name="MemberDetail.Pax"]').val());
    });
    $('[name="MemberDetail.AdditionalNights"]').on("propertychange change keyup paste input", function () {
        FetchDepartureDate($('[name="MemberDetail.DateOfArrival"]').val(), "AN");
    });
    $('[name="MemberDetail.DateOfArrival"]').change(function () {
        FetchDepartureDate($(this).val(), "AD");
    });
    $('[name="MemberDetail.ServiceId"]').change(function () {
        FetchDepartureDate($('[name="MemberDetail.DateOfArrival"]').val(), "AD");
    });
    $('[name="MemberDetail.PickUpDrop"]').change(function () {
        if ($(this).val() == 1) {
            $('#divddlPickUp').show();
            $('#divddlCarType').hide();
            $('#divtxtPickUpLocation').hide();
            $('#divFlightArrivalDateAndTime').hide();
            $('#divFlightDepartureDateAndTime').hide();
        }
        else {
            $('#divddlPickUp').hide();
            $('#divddlCarType').hide();
            $('#divtxtPickUpLocation').hide();
            $('#divFlightArrivalDateAndTime').hide();
            $('#divFlightDepartureDateAndTime').hide();
        }
    });
    $('[name="MemberDetail.PickUpType"]').change(function () {
        if ($(this).val() == 2) {
            $('#divddlCarType').show();
            $('#divtxtPickUpLocation').show();
            $('#divFlightArrivalDateAndTime').hide();
            $('#divFlightDepartureDateAndTime').hide();
        }
        else if ($(this).val() == 1) {
            $('#divddlCarType').show();
            $('#divtxtPickUpLocation').hide();
            $('#divFlightArrivalDateAndTime').show();
            $('#divFlightDepartureDateAndTime').show();
        }
        else {
            $('#divddlCarType').hide();
            $('#divtxtPickUpLocation').hide();
            $('#divFlightArrivalDateAndTime').hide();
            $('#divFlightDepartureDateAndTime').hide();
        }
    });
    $('[name="MemberDetail.GuarenteeCode"]').change(function () {
        FetchPaymentStatus($(this).val());
    });
    $('[name="MemberDetail.PaymentStatus"]').change(function () {
        if ($(this).val() == 1) {
            $('#divcalHoldTillDate').hide();
            $('#divcalPaymentDate').show();
        }
        else if ($(this).val() == 2) {
            $('#divcalHoldTillDate').show();
            $('#divcalPaymentDate').hide();
        }
        else if ($(this).val() == 3) {
            $('#divcalHoldTillDate').hide();
            $('#divcalPaymentDate').show();
        }
        else if ($(this).val() == 4) {
            $('#divcalHoldTillDate').hide();
            $('#divcalPaymentDate').hide();
        }
        else {
            $('#divcalHoldTillDate').hide();
            $('#divcalPaymentDate').hide();
        }
    });
    $('#ddlWantToSharePhoto').change(function () {
        if ($(this).val() == 0)
            $('#divUploadPhoto').hide();
        else if ($(this).val() == 1)
            $('#divUploadPhoto').show();
        else if ($(this).val() == 2)
            $('#divUploadPhoto').hide();
    });

    $('[name="MemberDetail.Idproof"]').on("propertychange change keyup paste input", function () {
        if ($('[name="MemberDetail.Idproof"]').val().length > 3)
            $('#divUploadProof').show();
        else
            $('#divUploadProof').hide();
    });
    $('[name="MemberDetail.PassportNo"]').on("propertychange change keyup paste input", function () {
        if ($('[name="MemberDetail.PassportNo"]').val().length > 3)
            $('#divUploadVisa').show();
        else
            $('#divUploadVisa').hide();
    });
}
function InitialiseMobileFields() {
    var txtmobile = document.querySelector("#MemberDetail_MobileNo");
    var initxtmobile = window.intlTelInput(txtmobile, {
        autoHideDialCode: false,
        nationalMode: false,
        initialCountry: "in",
        utilsScript: "CustomComponents/CountryCode/js/utils.js",
        //separateDialCode: true
    });

    //let element = document.querySelector('#MemberDetail_MobileNo');
    //if (element) {
    //    let currentPaddingLeft = window.getComputedStyle(element).getPropertyValue('padding-left');
    //    element.style.setProperty('padding-left', '100px ', '!important');
    //}
}
function getOnLoadAddGuests() {
    CalculateAge();
    $('[name="MemberDetail.PickUpType"]').change();
    $('[name="MemberDetail.PickUpDrop"]').change();
    $('[name="MemberDetail.PickUpLoaction"]').change();
    $('[name="MemberDetail.GuarenteeCode"]').change();
    $('[name="MemberDetail.PaymentStatus"]').change();

    GetServices();

}
function CalculateAge() {
    if ($("#AddGuestForm").find("[name='MemberDetail.Dob']").val() != '') {
        var date1 = $("#AddGuestForm").find("[name='MemberDetail.Dob']").val().split('-');
        date1 = new Date(date1[2], date1[1] - 1, date1[0]);
        var date2 = new Date();
        var timediff = Math.abs(date2.getTime() - date1.getTime());

        var ageDate = new Date(timediff); // miliseconds from epoch
        ageDate = Math.abs(ageDate.getUTCFullYear() - 1970);

        var daydiff = timediff / (1000 * 3600 * 24);

        daydiff = Math.round(daydiff);
        $("#AddGuestForm").find("[name='MemberDetail.Age']").val(ageDate);
    }
}
function GetServices() {
    let serviceId = $("#hfddlservices").val();
    let catID = $("[name='MemberDetail.CatId']").val();
    let inputDTO = {};
    inputDTO.Id = catID;
    $.ajax({
        type: "POST",
        url: "/Guests/GetCategoriesByServiceId",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            /*$successalert("", "Transaction Successful!");*/
            UnblockUI();
            let $ctrl = $("[name='MemberDetail.ServiceId']");
            $ctrl.empty();
            $ctrl.append('<option value="0">--Select Services *--</option>');
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    if (serviceId == data[i].id) {
                        $ctrl.append('<option selected="selected" value="' + data[i].id + '">' + data[i].category + '</option>');
                    }
                    else {
                        $ctrl.append('<option value="' + data[i].id + '">' + data[i].category + '</option>');
                    }

                }
            }
        },
        error: function (error) {
            /* $erroralert("Transaction Failed!", error.responseText + '!');*/
            UnblockUI();
        }
    });
}

function FetchDepartureDate(ArrivalDate, Source) {
    if (ArrivalDate == "") {
        return;
    }
    var AdditionalNights = 0;
    if (Source == "AD") {
        if (ArrivalDate == "" || ArrivalDate == null || ArrivalDate == undefined || ArrivalDate == "0") {
            alert("Please select valid date", "error");
            $('[name="MemberDetail.DateOfArrival"]').val('');
            return false;
        }
    }
    if ($('[name="MemberDetail.ServiceId"]').val() == "" || $('[name="MemberDetail.ServiceId"]').val() == null || $('[name="MemberDetail.ServiceId"]').val() == undefined || $('[name="MemberDetail.ServiceId"]').val() == "0") {
        alert("Please select select services first", "error");
        $('[name="MemberDetail.ServiceId"]').focus();
        $('[name="MemberDetail.DateOfArrival"]').val('');
        return false;
    }
    if ($('[name="MemberDetail.AdditionalNights"]').val() == "" || $('[name="MemberDetail.AdditionalNights"]').val() == null || $('[name="MemberDetail.AdditionalNights"]').val() == undefined || $('[name="MemberDetail.AdditionalNights"]').val() == "0") {
        AdditionalNights = 0;
    }
    else
        AdditionalNights = $('[name="MemberDetail.AdditionalNights"]').val();
    var inputDTO = {};

    inputDTO["DateOfArrival"] = moment(ArrivalDate, "DD/MM/YYYY").format("YYYY-MM-DD");
    //inputDTO["DateOfDepartment"] = "";
    inputDTO["CatId"] = $('[name="MemberDetail.ServiceId"]').val();
    inputDTO["AdditionalNights"] = AdditionalNights;

    //PostRequest("MemberRegistration.aspx/FetchDepartmentDate", JSON.stringify({ formData }), ManageFetchDepartureDateServerResponse, "POST");

    $.ajax({
        type: "POST",
        url: "/Guests/FetchDepartmentDate",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            /*$successalert("", "Transaction Successful!");*/
            UnblockUI();
            let dapartDate = moment(data, "YYYY-MM-DD").format("DD-MM-YYYY");
            $('[name="MemberDetail.DateOfDepartment"]').val(dapartDate)
            $('[name="TimeOfDepartment"]').val("11:00")
            /*$('#departure-time').val('11:00');*/
        },
        error: function (error) {
            /* $erroralert("Transaction Failed!", error.responseText + '!');*/
            UnblockUI();
        }
    });

}

function FetchPaymentStatus(Id) {
    $('[name="MemberDetail.PaymentStatus"]').empty();
    if (Id == 8 || Id == 2 || Id == 5 || Id == 6 || Id == 1) {
        $('#divddlPaymentStatus').show();
        $('#divcalHoldTillDate').hide();
        $('#divcalPaymentDate').hide();
        $('[name="MemberDetail.PaymentStatus"]').append('<option value="0" >--Payment Status *--</option>');
        $('[name="MemberDetail.PaymentStatus"]').append('<option value="2">Later</option>');
        $('[name="MemberDetail.PaymentStatus"]').append('<option value="3">Paid</option>');
        $('[name="MemberDetail.PaymentStatus"]').append('<option value="4">Pending</option>');
    }
    else if (Id == 7) {
        $('#divddlPaymentStatus').show();
        $('#divcalHoldTillDate').hide();
        $('#divcalPaymentDate').hide();
        $('[name="MemberDetail.PaymentStatus"]').append('<option value="0">--Payment Status *--</option>');
        $('[name="MemberDetail.PaymentStatus"]').append('<option value="1">Cash</option>');
    }
    else if (Id == 0 || Id == 9) {
        $('#divddlPaymentStatus').hide();
        $('#divcalHoldTillDate').hide();
        $('#divcalPaymentDate').hide();
    }

    let paymentStatus = $("#hfddlPaymentStatus").val();
    if (paymentStatus > 0) {
        $('[name="MemberDetail.PaymentStatus"]').val(paymentStatus);
    }

}

function AddGuests() {

    if (!isValidateForm("AddGuestForm")) {
        return;
    }
    //alert("Success");
    //return;

    let GuestForm = $("#AddGuestForm").find("[dbCol]");

    let dataVM = new FormData();

    var photoAttachment = jQuery("#fldphoto")[0].files;
    var idProofAttachment = jQuery("#fuIdProof")[0].files;
    var passportAttachment = jQuery("#txtpassupload")[0].files;
    for (var i = 0; i < photoAttachment.length; i++) {
        dataVM.append("PhotoAttachment", photoAttachment[i]);
    }
    for (var i = 0; i < idProofAttachment.length; i++) {
        dataVM.append("IdProofAttachment", idProofAttachment[i]);
    }
    for (var i = 0; i < passportAttachment.length; i++) {
        dataVM.append("PassportAttachment", passportAttachment[i]);
    }

    GuestForm.each((i, v) => {
        let currObj = $(v);
        let value = currObj.val();
        let id = currObj.attr("id");
        let name = currObj.attr("name");
        if (value != null && value != undefined && value !== "") {
            //value = value.toUpperCase();
        }

        if (currObj.hasClass('contactno') && value != "") {
            value = value.replace(/\s/g, "");
        }
        if (currObj.hasClass('dateonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let currentDateFormat = "DD-MM-YYYY";
                let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD";
                //value = moment(moment(value).format("DD-MM-YYYY")).format(dateformat);
                value = moment(value, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                value = "";
            }
        }
        if (currObj.hasClass('datetimeonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let currentDateFormat = "DD-MM-YYYY HH:mm";
                let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD HH:mm";
                value = moment(value, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                value = "";
            }
        }

        if (name == "MemberDetail.DateOfDepartment") {
            value += " " + $("[name='TimeOfDepartment']").val();
        }

        dataVM.append(currObj.attr("name"), value);
    });
    dataVM.append("MemberDetail.PAXSno", $("[name='MemberDetail.PAXSno']").val());
    dataVM.append("Source", "RoomAvailability");
    //dataVM.append("RoomAllocation.RNumber", "RoomAvailability");

    BlockUI();
    $.ajax({
        url: '/Guests/SaveMemberDetails',
        data: dataVM,
        //dataType: "json",
        async: false,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (response) {
            $("#addGuestPopup").find(".btn-close").click();
            if (response != null) {
                if (response.paxSno < response.pax) {
                    //$("[name='MemberDetail.PAXSno']").val(parseInt(response?.pAXSno) + 1);
                    AddGuestsPartialView(response.groupId, parseInt(response?.paxSno) + 1);
                }
                else {
                    window.location.href = "/Guests/GuestRegistrationSuccessfull/" + response.groupId;
                }
            }

            UnblockUI();
        },
        error: function (xhr, ajaxOptions, error) {
            UnblockUI();
            alert(xhr.responseText);
            //responseText
        }
    });

}