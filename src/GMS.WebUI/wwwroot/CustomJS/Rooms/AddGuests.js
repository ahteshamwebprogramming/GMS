function AddMoreGuestsInRooms(RNumber, GroupId, PaxSno) {
    $("#HiddenFields").find("[name='RoomNumber']").val(RNumber);
    $("#closeModalGuestsDetails").click();
    AddGuestsPartialView(GroupId, parseInt(PaxSno) + 1);
}
function AddGuestInRoomsEmpty(RNumber) {
    $("#HiddenFields").find("[name='RoomNumber']").val(RNumber);
    AddGuestsPartialView();
}

function RoutetoAddReservation(groupId, paxSno) {
    let roomNumber = $("#HiddenFields").find("[name='RoomNumber']").val()
    let obj = { GroupId: groupId, PAXSno: paxSno, PageSource: "RoomAvailability", RoomNumber: roomNumber };
    let json = JSON.stringify(obj);
    let encoded = encodeURIComponent(json);

    //window.location.href = `/Reservation/GuestReservation/${encoded}`;
    window.open(`/Reservation/GuestReservation/${encoded}`, '_blank');
}

function AddGuestsPartialView(GroupId = '', PAXSno = 1) {

    RoutetoAddReservation(GroupId, PAXSno);

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
            //initValidateRoomsAvailability();

            $("[name='MemberDetail.CountryId']").change();
            $("[name='MemberDetail.CatId']").change();

            $("select.form-control").chosen({
                width: '100%'
            });
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
        FetchDepartureDate($('[name="MemberDetail.DateOfArrival"]').val(), "NN");
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
        if ($(this).val() == "false")
            $('#divUploadPhoto').hide();
        else if ($(this).val() == "true")
            $('#divUploadPhoto').show();
        else if ($(this).val() == "false")
            $('#divUploadPhoto').hide();
    });

    $('[name="MemberDetail.Idproof"]').on("propertychange change keyup paste input", function () {
        if ($('[name="MemberDetail.Idproof"]').val().length > 0)
            $('#divUploadIdProofDetails').show();
        else
            $('#divUploadIdProofDetails').hide();
    });
    $('[name="MemberDetail.PassportNo"]').on("propertychange change keyup paste input", function () {
        if ($('[name="MemberDetail.PassportNo"]').val().length > 0)
            $('#divUploadPassportDetails').show();
        else
            $('#divUploadPassportDetails').hide();
    });
    $('[name="MemberDetail.VisaDetails"]').on("propertychange change keyup paste input", function () {
        if ($('[name="MemberDetail.VisaDetails"]').val().length > 0)
            $('#divUploadVisaDetails').show();
        else
            $('#divUploadVisaDetails').hide();
    });

    $("#AddGuestForm").find(".IssueDate").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#AddGuestForm").find(".ExpiryDate").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });

    $('[name="MemberDetail.NationalityId"]').change(function () {
        let nationality = $("[name='MemberDetail.NationalityId'] option:selected").text();

        if (nationality == "Indian") {
            $("[name='MemberDetail.Idproof']").parent().find('label').addClass('required');
            $("[name='MemberDetail.Idproof']").addClass('requiredInput');
            $("[name='MemberDetail.IdProofIssueDate']").parent().find('label').addClass('required');
            $("[name='MemberDetail.IdProofIssueDate']").addClass('requiredInput');
            $("[name='MemberDetail.IdProofExpiryDate']").parent().find('label').addClass('required');
            $("[name='MemberDetail.IdProofExpiryDate']").addClass('requiredInput');


            $("[name='MemberDetail.PassportNo']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.PassportNo']").removeClass('requiredInput');
            $("[name='MemberDetail.PassportIssueDate']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.PassportIssueDate']").removeClass('requiredInput');
            $("[name='MemberDetail.PassportExpiryDate']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.PassportExpiryDate']").removeClass('requiredInput');

            $("[name='MemberDetail.VisaDetails']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.VisaDetails']").removeClass('requiredInput');
            $("[name='MemberDetail.VisaIssueDate']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.VisaIssueDate']").removeClass('requiredInput');
            $("[name='MemberDetail.VisaExpiryDate']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.VisaExpiryDate']").removeClass('requiredInput');
        }
        else {
            $("[name='MemberDetail.Idproof']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.Idproof']").removeClass('requiredInput');
            $("[name='MemberDetail.IdProofIssueDate']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.IdProofIssueDate']").removeClass('requiredInput');
            $("[name='MemberDetail.IdProofExpiryDate']").parent().find('label').removeClass('required');
            $("[name='MemberDetail.IdProofExpiryDate']").removeClass('requiredInput');

            $("[name='MemberDetail.PassportNo']").parent().find('label').addClass('required');
            $("[name='MemberDetail.PassportNo']").addClass('requiredInput');
            $("[name='MemberDetail.PassportIssueDate']").parent().find('label').addClass('required');
            $("[name='MemberDetail.PassportIssueDate']").addClass('requiredInput');
            $("[name='MemberDetail.PassportExpiryDate']").parent().find('label').addClass('required');
            $("[name='MemberDetail.PassportExpiryDate']").addClass('requiredInput');

            $("[name='MemberDetail.VisaDetails']").parent().find('label').addClass('required');
            $("[name='MemberDetail.VisaDetails']").addClass('requiredInput');
            $("[name='MemberDetail.VisaIssueDate']").parent().find('label').addClass('required');
            $("[name='MemberDetail.VisaIssueDate']").addClass('requiredInput');
            $("[name='MemberDetail.VisaExpiryDate']").parent().find('label').addClass('required');
            $("[name='MemberDetail.VisaExpiryDate']").addClass('requiredInput');
        }

    });

    $('#ddlWantToSharePhoto').change();
    $('[name="MemberDetail.Idproof"]').change();
    $('[name="MemberDetail.PassportNo"]').change();
    $('[name="MemberDetail.VisaDetails"]').change();
    $('[name="MemberDetail.NationalityId"]').change();
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

    //GetServices();
    GetServices_New();

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
function GetServices_New(serviceId) {
    let minNight = $("[name='MemberDetail.CatId'] option:selected").attr('minNight');
    let maxNight = $("[name='MemberDetail.CatId'] option:selected").attr('maxNight');
    let price = $("[name='MemberDetail.CatId'] option:selected").attr('price');

    let $noOfNights = $("[name='MemberDetail.ServiceId']");
    $noOfNights.empty();
    if (minNight != null && minNight !== undefined && maxNight != null && maxNight !== undefined) {
        minNight = parseInt(minNight);
        maxNight = parseInt(maxNight);
        for (var i = minNight; i <= maxNight; i++) {
            if (serviceId == i) {
                $noOfNights.append('<option selected="selected" value="' + i + '">' + i + ' Nights </option>');
            }
            else {
                $noOfNights.append('<option value="' + i + '">' + i + ' Nights </option>');
            }
        }
    }
    $noOfNights.trigger("chosen:updated");
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

function FetchDepartureDate1(ArrivalDate, Source) {
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
        $erroralert("Package not found!", 'Please select select package first!');
        //alert("Please select select services first", "error");
        $('[name="MemberDetail.ServiceId"]').focus();
        $('[name="MemberDetail.DateOfArrival"]').val('');
        return false;
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

function FetchDepartureDate(ArrivalDate, Source) {
    if (ArrivalDate == "") {
        return;
    }
    var AdditionalNights = 0;
    if (Source == "AD") {
        if (ArrivalDate == "" || ArrivalDate == null || ArrivalDate == undefined || ArrivalDate == "0") {
            $erroralert("Invalid Date!", 'Please select valid date!');
            //alert("Please select valid date", "error");
            $('[name="MemberDetail.DateOfArrival"]').val('');
            return false;
        }
    }
    if ($('[name="MemberDetail.ServiceId"]').val() == "" || $('[name="MemberDetail.ServiceId"]').val() == null || $('[name="MemberDetail.ServiceId"]').val() == undefined || $('[name="MemberDetail.ServiceId"]').val() == "0") {
        $erroralert("Package not found!", 'Please select select package first!');
        //alert("Please select select services first", "error");
        $('[name="MemberDetail.ServiceId"]').focus();
        $('[name="MemberDetail.DateOfArrival"]').val('');
        return false;
    }
    if ($('[name="MemberDetail.AdditionalNights"]').val() == "" || $('[name="MemberDetail.AdditionalNights"]').val() == null || $('[name="MemberDetail.AdditionalNights"]').val() == undefined || $('[name="MemberDetail.AdditionalNights"]').val() == "0") {
        AdditionalNights = 0;
    }
    else
        AdditionalNights = $('[name="MemberDetail.AdditionalNights"]').val();

    let NoOfNights = $('[name="MemberDetail.ServiceId"]').val();

    if (NoOfNights == null || NoOfNights === undefined || isNaN(NoOfNights)) {
        $erroralert("Error!", 'Please select number of nights');
    }

    NoOfNights = parseInt(NoOfNights);

    let NoOfTotalNights = NoOfNights + parseInt(AdditionalNights);

    let departureDate = moment(ArrivalDate, "DD/MM/YYYY").add(NoOfTotalNights, 'days');
    $('[name="MemberDetail.DateOfDepartment"]').val(departureDate.format("DD-MM-YYYY"));
    $('[name="TimeOfDepartment"]').val("11:00")
    //var inputDTO = {};

    //inputDTO["DateOfArrival"] = moment(ArrivalDate, "DD/MM/YYYY").format("YYYY-MM-DD");
    ////inputDTO["DateOfDepartment"] = "";
    //inputDTO["CatId"] = $('[name="MemberDetail.ServiceId"]').val();
    //inputDTO["AdditionalNights"] = AdditionalNights;



    //PostRequest("MemberRegistration.aspx/FetchDepartmentDate", JSON.stringify({ formData }), ManageFetchDepartureDateServerResponse, "POST");

    //$.ajax({
    //    type: "POST",
    //    url: "/Guests/FetchDepartmentDate",
    //    contentType: 'application/json',
    //    data: JSON.stringify(inputDTO),
    //    success: function (data) {
    //        /*$successalert("", "Transaction Successful!");*/
    //        UnblockUI();
    //        let dapartDate = moment(data, "YYYY-MM-DD").format("DD-MM-YYYY");
    //        $('[name="MemberDetail.DateOfDepartment"]').val(dapartDate)
    //        $('[name="TimeOfDepartment"]').val("11:00")
    //        /*$('#departure-time').val('11:00');*/
    //    },
    //    error: function (error) {
    //        /* $erroralert("Transaction Failed!", error.responseText + '!');*/
    //        UnblockUI();
    //    }
    //});

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
    $("[name='MemberDetail.PaymentStatus']").trigger('chosen:updated');
}

function _isValidateForm(formId) {

    $('.requiredInputCstmFile').on('click change paste keyup', function () {
        var element = $(this);
        element.removeClass('error');
        element.parent().find('.error-mandatory').remove();
    });


    let res = isValidateForm(formId);

    var photoFromInput = jQuery("#attachmentPhoto")[0].files;
    var photoFromDB = $("#attachmentPhoto").parent().find("[name^=attachmentTag_]");
    if ($("#ddlWantToSharePhoto").val() == "true" && !(photoFromInput != null && photoFromInput != undefined && photoFromInput.length != 0) && (photoFromDB.length == 0)) {
        let element = jQuery("#attachmentPhoto");
        element.removeClass('error');
        element.parent().find('.error-mandatory').remove();
        element.addClass('error');
        element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
        res = false;
    }

    let nationality = $("[name='MemberDetail.NationalityId'] option:selected").text();
    var idProofFromInput = jQuery("#attachmentIdProof")[0].files;
    var idProofFromDB = $("#attachmentIdProof").parent().find("[name^=attachmentTag_]");
    if (nationality == "Indian" && !(idProofFromInput != null && idProofFromInput != undefined && idProofFromInput.length != 0) && (idProofFromDB.length == 0)) {
        let element = jQuery("#attachmentIdProof");
        element.removeClass('error');
        element.parent().find('.error-mandatory').remove();
        element.addClass('error');
        element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
        res = false;
    }


    var passportFromInput = jQuery("#attachmentPassport")[0].files;
    var passportFromDB = $("#attachmentPassport").parent().find("[name^=attachmentTag_]");
    if (nationality != "Indian" && !(passportFromInput != null && passportFromInput != undefined && passportFromInput.length != 0) && (passportFromDB.length == 0)) {
        let element = jQuery("#attachmentPassport");
        element.removeClass('error');
        element.parent().find('.error-mandatory').remove();
        element.addClass('error');
        element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
        res = false;
    }


    var visaFromInput = jQuery("#attachmentVisaDetails")[0].files;
    var visaFromDB = $("#attachmentVisaDetails").parent().find("[name^=attachmentTag_]");
    if (nationality != "Indian" && !(visaFromInput != null && visaFromInput != undefined && visaFromInput.length != 0) && (visaFromDB.length == 0)) {
        let element = jQuery("#attachmentVisaDetails");
        element.removeClass('error');
        element.parent().find('.error-mandatory').remove();
        element.addClass('error');
        element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
        res = false;
    }



    return res;
}
function AddGuests() {

    if (!_isValidateForm("AddGuestForm")) {
        return;
    }

    ValidateRoomsAvailability().then((d) => {
        let GuestForm = $("#AddGuestForm").find("[dbCol]");

        let dataVM = new FormData();

        var photoAttachment = jQuery("#attachmentPhoto")[0].files;
        var idProofAttachment = jQuery("#attachmentIdProof")[0].files;
        var passportAttachment = jQuery("#attachmentPassport")[0].files;
        var visaAttachment = jQuery("#attachmentVisaDetails")[0].files;
        for (var i = 0; i < photoAttachment.length; i++) {
            dataVM.append("PhotoAttachment", photoAttachment[i]);
        }
        for (var i = 0; i < idProofAttachment.length; i++) {
            dataVM.append("IdProofAttachment", idProofAttachment[i]);
        }
        for (var i = 0; i < passportAttachment.length; i++) {
            dataVM.append("PassportAttachment", passportAttachment[i]);
        }
        for (var i = 0; i < visaAttachment.length; i++) {
            dataVM.append("VisaAttachment", visaAttachment[i]);
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
    }).catch((d) => {
        $erroralert("Validation Failed!", d + '!');
    });
}

function initValidateRoomsAvailability() {
    //CheckInputs();

    $("#AddGuestForm").find('[name="MemberDetail.DateOfArrival"]').change(function () {
        ValidateRoomsAvailability().then((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestForm").find('[name="MemberDetail.DateOfDepartment"]').change(function () {
        ValidateRoomsAvailability().then((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestForm").find('[name="TimeOfDepartment"]').change(function () {
        ValidateRoomsAvailability().then((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestForm").find('[name="MemberDetail.RoomType"]').change(function () {
        ValidateRoomsAvailability().then((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestForm").find('[name="MemberDetail.Pax"]').change(function () {
        ValidateRoomsAvailability().then((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestForm").find('[name="MemberDetail.NoOfRooms"]').change(function () {
        ValidateRoomsAvailability().then((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestForm").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
}
function ValidateRoomsAvailability() {
    return new Promise((resolve, reject) => {
        let sourceDateFormat = "YYYY-MM-DDTHH:mm";
        let DateOfArrivalObj = $("#AddGuestForm").find('[name="MemberDetail.DateOfArrival"]');
        let DateOfArrival = $("#AddGuestForm").find('[name="MemberDetail.DateOfArrival"]').val();
        if (DateOfArrivalObj.hasClass('datetimeonly') && DateOfArrival != "") {
            try {
                let dateformat = DateOfArrivalObj.attr("dateformat");
                let currentDateFormat = "DD-MM-YYYY HH:mm";
                //sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD HH:mm";
                DateOfArrival = moment(DateOfArrival, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                DateOfArrival = "";
            }
        }
        let DateOfDepartmentObj = $("#AddGuestForm").find('[name="MemberDetail.DateOfDepartment"]');
        let DateOfDepartment = $("#AddGuestForm").find('[name="MemberDetail.DateOfDepartment"]').val();
        if (DateOfDepartmentObj.hasClass('dateonly') && DateOfDepartment != "") {
            try {
                let dateformat = DateOfDepartmentObj.attr("dateformat");
                let currentDateFormat = "DD-MM-YYYY";
                //sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD";
                sourceDateFormat = "YYYY-MM-DD";
                DateOfDepartment = moment(DateOfDepartment, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                DateOfDepartment = "";
            }
        }



        let TimeOfDepartment = $("#AddGuestForm").find('[name="TimeOfDepartment"]').val();
        let RoomType = $("#AddGuestForm").find('[name="MemberDetail.RoomType"]').val();
        let Pax = $("#AddGuestForm").find('[name="MemberDetail.Pax"]').val();
        let NoOfRooms = $("#AddGuestForm").find('[name="MemberDetail.NoOfRooms"]').val();

        if (DateOfArrival == null || DateOfArrival == undefined || DateOfArrival == "") {
            return reject("Date Of Arrival is empty or invalid");
        }
        if (DateOfDepartment == null || DateOfDepartment == undefined || DateOfDepartment == "") {
            return reject("Date Of Department is empty or invalid");
        }
        if (TimeOfDepartment == null || TimeOfDepartment == undefined || TimeOfDepartment == "") {
            return reject("Time Of Department is empty or invalid");
        }
        if (RoomType == null || RoomType == undefined || RoomType == "" || RoomType == 0) {
            return reject("Room Type is empty or invalid");
        }
        if (Pax == null || Pax == undefined || Pax == "" || Pax == 0) {
            return reject("Pax is empty or invalid");
        }
        if (NoOfRooms == null || NoOfRooms == undefined || NoOfRooms == "" || NoOfRooms == 0) {
            return reject("No Of Rooms is empty or invalid");
        }

        let inputDTO = {};

        inputDTO["DateOfArrival"] = DateOfArrival;
        inputDTO["DateOfDepartment"] = DateOfDepartment + "T" + TimeOfDepartment;
        inputDTO["RoomType"] = RoomType;
        inputDTO["Pax"] = Pax;
        inputDTO["NoOfRooms"] = NoOfRooms;
        $.ajax({
            type: "POST",
            url: "/Guests/ValidateRoomsAvailability",
            contentType: 'application/json',
            data: JSON.stringify(inputDTO),
            success: function (data) {
                resolve(data);
            },
            error: function (error) {
                reject(error.responseText);
            }
        });
    })
}



function fetchState(CountryId, StateId) {
    let inputDTO = {};
    inputDTO.Id = CountryId;
    $.ajax({
        type: "POST",
        url: "/Guests/FetchState",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $("[name='MemberDetail.StateId']").empty();
            $("[name='MemberDetail.StateId']").append('<option selected="selected" value="0">Select State</option>');
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    if (StateId == data[i].id) {
                        $("[name='MemberDetail.StateId']").append('<option selected="selected" value="' + data[i].id + '">' + data[i].name + '</option>');
                    }
                    else {
                        $("[name='MemberDetail.StateId']").append('<option  value="' + data[i].id + '">' + data[i].name + '</option>');
                    }
                }
            }
            $("[name='MemberDetail.StateId']").trigger('chosen:updated');
            $("[name='MemberDetail.NationalityId']").val(CountryId).trigger("chosen:updated");
            $("[name='MemberDetail.NationalityId']").change();
            $("[name='MemberDetail.StateId']").change();
        },
        error: function (error) {

        }
    });
}
function fetchCity(stateId, cityId) {
    let inputDTO = {};
    inputDTO.Id = stateId;
    $.ajax({
        type: "POST",
        url: "/Guests/FetchCity",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $("[name='MemberDetail.CityId']").empty();
            $("[name='MemberDetail.CityId']").append('<option selected="selected" value="0"></option>');
            if (data != null) {
                for (var i = 0; i < data.length; i++) {
                    if (cityId == data[i].id) {
                        $("[name='MemberDetail.CityId']").append('<option selected="selected" value="' + data[i].id + '">' + data[i].name + '</option>');
                    }
                    else {
                        $("[name='MemberDetail.CityId']").append('<option  value="' + data[i].id + '">' + data[i].name + '</option>');
                    }
                }
            }
            $("[name='MemberDetail.CityId']").trigger('chosen:updated');
        },
        error: function (error) {

        }
    });
}


function deleteUploadedFile(Id) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "Id": Id
            };
            $.ajax({
                type: "POST",
                url: "/Guests/DeleteUploadedFile",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Deleted Successful!");
                    $("[name='attachmentTag_" + Id + "']").remove();
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
function downloadAttachment(fileId, filename) {
    // AJAX request to download the file
    $.ajax({
        url: '/Guests/DownloadFile/' + fileId,
        method: 'GET',
        xhrFields: {
            responseType: 'blob' // Set the response type to blob
        },
        success: function (data) {
            // Create a temporary anchor element to trigger the download
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = filename; // Set the filename for download
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            //$successalert("Success!", "Downloaded succesfully.");
        },
        error: function (xhr, status, error) {
            $erroralert("Oops!", 'Error downloading file:' + xhr.responseText);
        }
    });
}