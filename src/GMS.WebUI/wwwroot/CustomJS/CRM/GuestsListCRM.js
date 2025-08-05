$(document).ready(function () {
    GuestsListPartialViewCRM();
});
function initDatesCRM() {
    InitialiseMobileFieldsCRM();
    $("#AddGuestFormCRM").find("[name='MemberDetail.Dob']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#AddGuestFormCRM").find("[name='MemberDetail.DateOfArrival']").datetimepicker({
        format: 'd-m-Y H:i',
        timepicker: true,
        defaultTime: '14:00'
    });
    $("#AddGuestFormCRM").find("[name='TimeOfDepartment']").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30 // Optional: Define minute intervals
    });
    $("#AddGuestFormCRM").find("[name='MemberDetail.FlightArrivalDateAndDateTime']").datetimepicker({
        format: 'd-m-Y H:i',
        //timepicker: false,
    });
    $("#AddGuestFormCRM").find("[name='MemberDetail.FlightDepartureDateAndDateTime']").datetimepicker({
        format: 'd-m-Y H:i',
        //timepicker: false,
    });
    $("#AddGuestFormCRM").find("[name='MemberDetail.HoldTillDate']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#AddGuestFormCRM").find("[name='MemberDetail.PaymentDate']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });

    $("#AddGuestFormCRM").find("[name='MemberDetail.Dob']").change(function () {
        CalculateAgeCRM();
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.Pax"]').on("propertychange change keyup paste input", function () {
        $("#AddGuestFormCRM").find('[name="MemberDetail.NoOfRooms"]').val($("#AddGuestFormCRM").find('[name="MemberDetail.Pax"]').val());
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.AdditionalNights"]').on("propertychange change keyup paste input", function () {
        FetchDepartureDateCRM($("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]').val(), "AN");
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]').change(function () {
        FetchDepartureDateCRM($(this).val(), "AD");
        GetRoomRatesForEnquiryCRM();
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.ServiceId"]').change(function () {
        FetchDepartureDateCRM($("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]').val(), "NN");
        GetRoomRatesForEnquiryCRM();
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.RoomType"]').change(function () {
        GetRoomRatesForEnquiryCRM();
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.CatId"]').change(function () {
        FetchDepartureDateCRM($("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]').val(), "NN");
        GetRoomRatesForEnquiryCRM();
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.PickUpDrop"]').change(function () {
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
    $("#AddGuestFormCRM").find('[name="MemberDetail.PickUpType"]').change(function () {
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
    $("#AddGuestFormCRM").find('[name="MemberDetail.GuarenteeCode"]').change(function () {
        FetchPaymentStatusCRM($(this).val());
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').change(function () {
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

    $("#AddGuestFormCRM").find('[name="MemberDetail.Idproof"]').on("propertychange change keyup paste input", function () {
        if ($("#AddGuestFormCRM").find('[name="MemberDetail.Idproof"]').val().length > 0)
            $('#divUploadIdProofDetails').show();
        else
            $('#divUploadIdProofDetails').hide();
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.PassportNo"]').on("propertychange change keyup paste input", function () {
        if ($("#AddGuestFormCRM").find('[name="MemberDetail.PassportNo"]').val().length > 0)
            $('#divUploadPassportDetails').show();
        else
            $('#divUploadPassportDetails').hide();
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.VisaDetails"]').on("propertychange change keyup paste input", function () {
        if ($("#AddGuestFormCRM").find('[name="MemberDetail.VisaDetails"]').val().length > 0)
            $('#divUploadVisaDetails').show();
        else
            $('#divUploadVisaDetails').hide();
    });

    $("#AddGuestFormCRM").find(".IssueDate").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#AddGuestFormCRM").find(".ExpiryDate").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });

    $("#AddGuestFormCRM").find('[name="MemberDetail.NationalityId1"]').change(function () {
        let nationality = $("[name='MemberDetail.NationalityId'] option:selected").text();

        if (nationality == "Indian") {
            $("[name='MemberDetail.Idproof']").parent().find('label').addClass('required');
            $("[name='MemberDetail.Idproof']").addClass('requiredInput');
            //$("[name='MemberDetail.IdProofIssueDate']").parent().find('label').addClass('required');
            //$("[name='MemberDetail.IdProofIssueDate']").addClass('requiredInput');
            //$("[name='MemberDetail.IdProofExpiryDate']").parent().find('label').addClass('required');
            //$("[name='MemberDetail.IdProofExpiryDate']").addClass('requiredInput');


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
            //$("[name='MemberDetail.IdProofIssueDate']").parent().find('label').removeClass('required');
            //$("[name='MemberDetail.IdProofIssueDate']").removeClass('requiredInput');
            //$("[name='MemberDetail.IdProofExpiryDate']").parent().find('label').removeClass('required');
            //$("[name='MemberDetail.IdProofExpiryDate']").removeClass('requiredInput');

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
    $("#AddGuestFormCRM").find('[name="MemberDetail.Idproof"]').change();
    $("#AddGuestFormCRM").find('[name="MemberDetail.PassportNo"]').change();
    $("#AddGuestFormCRM").find('[name="MemberDetail.VisaDetails"]').change();
    $("#AddGuestFormCRM").find('[name="MemberDetail.NationalityId"]').change();
}

function getOnLoadAddGuestsCRM() {
    //CalculateAge();
    $("#AddGuestFormCRM").find('[name="MemberDetail.PickUpDrop"]').change();
    $("#AddGuestFormCRM").find('[name="MemberDetail.PickUpLoaction"]').change();
    $("#AddGuestFormCRM").find('[name="MemberDetail.GuarenteeCode"]').change();
    $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').change();

    //GetServices();
    GetServices_NewCRM();

}

function GuestsListPartialViewCRM(PageNumber = 1) {
    let GuestsListType = $("#GuestsListType").val();
    if (GuestsListType == "") {
        GuestsListType = "Current";
    }
    let SearchKeyword = $("#GuestsSearchKeyword").val();
    var inputDTO = {};
    inputDTO.GuestsListType = GuestsListType;
    inputDTO.PageSize = 10;
    inputDTO.PageNumber = PageNumber;
    inputDTO.SearchKeyword = SearchKeyword;
    //inputDTO.Source = $("[name='Source']").val();
    BlockUI();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Guests/GuestsGridViewPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewGuestsListPartial').html(data);
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function setActiveCRM(type) {
    $("#GuestsListType").val(type);
    GuestsListPartialViewCRM();
}

function AddGuestsPartialViewCRM(GroupId = '', PAXSno = 1) {

    let inputDTO = {};
    inputDTO.GroupId = GroupId;
    inputDTO.PAXSno = PAXSno;

    BlockUI();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/CRM/AddGuestsPartialViewCRM',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();

            $('#div_AddGuestsListPartialCRM').html(data);
            //$("#btnModelAddGuests").click();
            $("#AddGuestFormCRM").find("[name='MemberDetail.PAXSno']").val(PAXSno);
            $("#AddGuestFormCRM").find("[name='spnPAXSno']").text(PAXSno);
            initDatesCRM();
            getOnLoadAddGuestsCRM();
            initValidateRoomsAvailabilityCRM();
            $("[name='MemberDetail.CountryId']").change();
            $("[name='MemberDetail.CatId']").change();

            $("select").chosen({
                width: '100%'
            });
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}


function InitialiseMobileFieldsCRM() {
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

function GetServices_NewCRM(serviceId) {
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

function GetServicesCRM() {
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

function FetchDepartureDateCRM(ArrivalDate, Source) {
    if (ArrivalDate == "") {
        return;
    }
    var AdditionalNights = 0;
    if (Source == "AD") {
        if (ArrivalDate == "" || ArrivalDate == null || ArrivalDate == undefined || ArrivalDate == "0") {
            $erroralert("Invalid Date!", 'Please select valid date!');
            //alert("Please select valid date", "error");
            $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]').val('');
            return false;
        }
    }
    if ($("#AddGuestFormCRM").find('[name="MemberDetail.ServiceId"]').val() == "" || $("#AddGuestFormCRM").find('[name="MemberDetail.ServiceId"]').val() == null || $("#AddGuestFormCRM").find('[name="MemberDetail.ServiceId"]').val() == undefined || $("#AddGuestFormCRM").find('[name="MemberDetail.ServiceId"]').val() == "0") {
        $erroralert("Package not found!", 'Please select select package first!');
        //alert("Please select select services first", "error");
        $("#AddGuestFormCRM").find('[name="MemberDetail.ServiceId"]').focus();
        $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]').val('');
        return false;
    }
    if ($("#AddGuestFormCRM").find('[name="MemberDetail.AdditionalNights"]').val() == "" || $("#AddGuestFormCRM").find('[name="MemberDetail.AdditionalNights"]').val() == null || $("#AddGuestFormCRM").find('[name="MemberDetail.AdditionalNights"]').val() == undefined || $("#AddGuestFormCRM").find('[name="MemberDetail.AdditionalNights"]').val() == "0") {
        AdditionalNights = 0;
    }
    else
        AdditionalNights = $("#AddGuestFormCRM").find('[name="MemberDetail.AdditionalNights"]').val();

    let NoOfNights = $("#AddGuestFormCRM").find('[name="MemberDetail.ServiceId"]').val();

    if (NoOfNights == null || NoOfNights === undefined || isNaN(NoOfNights)) {
        $erroralert("Error!", 'Please select number of nights');
    }

    NoOfNights = parseInt(NoOfNights);

    let NoOfTotalNights = NoOfNights + parseInt(AdditionalNights);

    let departureDate = moment(ArrivalDate, "DD/MM/YYYY").add(NoOfTotalNights, 'days');
    $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfDepartment"]').val(departureDate.format("DD-MM-YYYY"));
    $('[name="TimeOfDepartment"]').val("11:00")
    //var inputDTO = {};

    //inputDTO["DateOfArrival"] = moment(ArrivalDate, "DD/MM/YYYY").format("YYYY-MM-DD");
    ////inputDTO["DateOfDepartment"] = "";
    //inputDTO["CatId"] = $("#AddGuestFormCRM").find('[name="MemberDetail.ServiceId"]').val();
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
    //        $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfDepartment"]').val(dapartDate)
    //        $('[name="TimeOfDepartment"]').val("11:00")
    //        /*$('#departure-time').val('11:00');*/
    //    },
    //    error: function (error) {
    //        /* $erroralert("Transaction Failed!", error.responseText + '!');*/
    //        UnblockUI();
    //    }
    //});

}

function FetchPaymentStatusCRM(Id) {
    $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').empty();
    if (Id == 8 || Id == 2 || Id == 5 || Id == 6 || Id == 1) {
        $('#divddlPaymentStatus').show();
        $('#divcalHoldTillDate').hide();
        $('#divcalPaymentDate').hide();
        $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').append('<option value="0" >--Payment Status *--</option>');
        $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').append('<option value="2">Later</option>');
        $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').append('<option value="3">Paid</option>');
        $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').append('<option value="4">Pending</option>');
    }
    else if (Id == 7) {
        $('#divddlPaymentStatus').show();
        $('#divcalHoldTillDate').hide();
        $('#divcalPaymentDate').hide();
        $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').append('<option value="0">--Payment Status *--</option>');
        $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').append('<option value="1">Cash</option>');
    }
    else if (Id == 0 || Id == 9) {
        $('#divddlPaymentStatus').hide();
        $('#divcalHoldTillDate').hide();
        $('#divcalPaymentDate').hide();
    }

    let paymentStatus = $("#hfddlPaymentStatus").val();
    if (paymentStatus > 0) {
        $("#AddGuestFormCRM").find('[name="MemberDetail.PaymentStatus"]').val(paymentStatus);
    }

    $("[name='MemberDetail.PaymentStatus']").trigger('chosen:updated');

}

function _isValidateFormCRM(formId) {

    $('.requiredInputCstmFile').on('click change paste keyup', function () {
        var element = $(this);
        element.removeClass('error');
        element.parent().find('.error-mandatory').remove();
    });


    let res = isValidateForm(formId);

    //var photoFromInput = jQuery("#attachmentPhoto")[0].files;
    //var photoFromDB = $("#attachmentPhoto").parent().parent().find("[name^=attachmentTag_]");
    //if ($("#ddlWantToSharePhoto").val() == "true" && !(photoFromInput != null && photoFromInput != undefined && photoFromInput.length != 0) && (photoFromDB.length == 0)) {
    //    let element = jQuery("#attachmentPhoto");
    //    element.removeClass('error');
    //    element.parent().find('.error-mandatory').remove();
    //    element.addClass('error');
    //    element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
    //    res = false;
    //}

    //let nationality = $("[name='MemberDetail.NationalityId'] option:selected").text();
    //var idProofFromInput = jQuery("#attachmentIdProof")[0].files;
    //var idProofFromDB = $("#attachmentIdProof").parent().parent().find("[name^=attachmentTag_]");
    //if (nationality == "Indian" && !(idProofFromInput != null && idProofFromInput != undefined && idProofFromInput.length != 0) && (idProofFromDB.length == 0)) {
    //    let element = jQuery("#attachmentIdProof");
    //    element.removeClass('error');
    //    element.parent().find('.error-mandatory').remove();
    //    element.addClass('error');
    //    element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
    //    res = false;
    //}


    //var passportFromInput = jQuery("#attachmentPassport")[0].files;
    //var passportFromDB = $("#attachmentPassport").parent().parent().find("[name^=attachmentTag_]");
    //if (nationality != "Indian" && !(passportFromInput != null && passportFromInput != undefined && passportFromInput.length != 0) && (passportFromDB.length == 0)) {
    //    let element = jQuery("#attachmentPassport");
    //    element.removeClass('error');
    //    element.parent().find('.error-mandatory').remove();
    //    element.addClass('error');
    //    element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
    //    res = false;
    //}


    //var visaFromInput = jQuery("#attachmentVisaDetails")[0].files;
    //var visaFromDB = $("#attachmentVisaDetails").parent().find("[name^=attachmentTag_]");
    //if (nationality != "Indian" && !(visaFromInput != null && visaFromInput != undefined && visaFromInput.length != 0) && (visaFromDB.length == 0)) {
    //    let element = jQuery("#attachmentVisaDetails");
    //    element.removeClass('error');
    //    element.parent().find('.error-mandatory').remove();
    //    element.addClass('error');
    //    element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
    //    res = false;
    //}



    return res;
}

function initValidateRoomsAvailabilityCRM() {
    //CheckInputs();

    $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]').change(function () {
        ValidateRoomsAvailabilityCRM().then((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfDepartment"]').change(function () {
        ValidateRoomsAvailabilityCRM().then((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestFormCRM").find('[name="TimeOfDepartment"]').change(function () {
        ValidateRoomsAvailabilityCRM().then((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.RoomType"]').change(function () {
        ValidateRoomsAvailabilityCRM().then((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.Pax"]').change(function () {
        ValidateRoomsAvailabilityCRM().then((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#AddGuestFormCRM").find('[name="MemberDetail.NoOfRooms"]').change(function () {
        ValidateRoomsAvailabilityCRM().then((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#AddGuestFormCRM").find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
}

//function CheckInputs() {
//    let inputDTO = {};

//    inputDTO["DateOfArrival"] = "2025-01-01T12:00:00";
//    inputDTO["DateOfDepartment"] = null;
//    inputDTO["RoomType"] = "1";
//    inputDTO["Pax"] = "1";
//    inputDTO["NoOfRooms"] = "1";
//    $.ajax({
//        type: "POST",
//        url: "/Guests/ValidateRoomsAvailability",
//        contentType: 'application/json',
//        data: JSON.stringify(inputDTO),
//        success: function (data) {

//        },
//        error: function (error) {

//        }
//    });
//}

function ValidateRoomsAvailabilityCRM() {
    return new Promise((resolve, reject) => {
        let sourceDateFormat = "YYYY-MM-DDTHH:mm";
        let DateOfArrivalObj = $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]');
        let DateOfArrival = $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfArrival"]').val();
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
        let DateOfDepartmentObj = $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfDepartment"]');
        let DateOfDepartment = $("#AddGuestFormCRM").find('[name="MemberDetail.DateOfDepartment"]').val();
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



        let TimeOfDepartment = $("#AddGuestFormCRM").find('[name="TimeOfDepartment"]').val();
        let RoomType = $("#AddGuestFormCRM").find('[name="MemberDetail.RoomType"]').val();
        let Pax = $("#AddGuestFormCRM").find('[name="MemberDetail.Pax"]').val();
        let NoOfRooms = $("#AddGuestFormCRM").find('[name="MemberDetail.NoOfRooms"]').val();
        let GuestId = $("#AddGuestFormCRM").find("[name='MemberDetail.Id']").val();
        GuestId = (GuestId == "" || GuestId == null || GuestId == undefined) ? 0 : GuestId;
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
        inputDTO["Id"] = GuestId;
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

function AddGuestsCRM() {

    if (!_isValidateFormCRM("AddGuestFormCRM")) {
        return;
    }
    //alert("Success");
    //return;

    ValidateRoomsAvailabilityCRM().then((d) => {
        let GuestForm = $("#AddGuestFormCRM").find("[dbCol]");

        let dataVM = new FormData();

        //var photoAttachment = jQuery("#attachmentPhoto")[0].files;
        //var idProofAttachment = jQuery("#attachmentIdProof")[0].files;
        //var passportAttachment = jQuery("#attachmentPassport")[0].files;
        //var visaAttachment = jQuery("#attachmentVisaDetails")[0].files;
        //for (var i = 0; i < photoAttachment.length; i++) {
        //    dataVM.append("PhotoAttachment", photoAttachment[i]);
        //}
        //for (var i = 0; i < idProofAttachment.length; i++) {
        //    dataVM.append("IdProofAttachment", idProofAttachment[i]);
        //}
        //for (var i = 0; i < passportAttachment.length; i++) {
        //    dataVM.append("PassportAttachment", passportAttachment[i]);
        //}
        //for (var i = 0; i < visaAttachment.length; i++) {
        //    dataVM.append("VisaAttachment", visaAttachment[i]);
        //}

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
        dataVM.append("MemberDetail.PAXSno", $("#AddGuestFormCRM").find("[name='MemberDetail.PAXSno']").val());
        BlockUI();
        $.ajax({
            url: '/CRM/SaveMemberDetailsCRM',
            data: dataVM,
            //dataType: "json",
            async: false,
            type: 'POST',
            processData: false,
            contentType: false,
            success: function (response) {
                RoutetoAddReservation(response.groupId, 1);

                return;

                AddGuestsPartialView(response.groupId, 1);

                let obj = { GroupId: response.groupId, PAXSno: 1, PageSource: "NewEnquiry" };
                let json = JSON.stringify(obj);
                let encoded = encodeURIComponent(json);

                window.location.href = `/Reservation/GuestReservation/${encoded}`;

                //$("#addGuestPopup").find(".btn-close").click();
                //if (response != null) {
                //    if (response.paxSno < response.pax) {
                //        //$("[name='MemberDetail.PAXSno']").val(parseInt(response?.pAXSno) + 1);
                //        AddGuestsPartialViewCRM(response.groupId, parseInt(response?.paxSno) + 1);
                //    }
                //    else {
                //        if (response.opt == "Update") {
                //            GuestsListPartialViewCRM(1);
                //        }
                //        else {
                //            window.location.href = "/Guests/GuestRegistrationSuccessfull/" + response.groupId;
                //        }
                //    }
                //}

                UnblockUI();
            },
            error: function (xhr, ajaxOptions, error) {
                UnblockUI();
                $erroralert("Data Submission Error!", xhr.responseText + '!');
                //alert(xhr.responseText);
                //responseText
            }
        });

    }).catch((d) => {
        $erroralert("Validation Failed!", d + '!');
    });



}

function RoutetoAddReservation(groupId, paxSno) {
    let obj = { GroupId: groupId, PAXSno: paxSno, PageSource: "NewEnquiry" };
    let json = JSON.stringify(obj);
    let encoded = encodeURIComponent(json);

    window.location.href = `/Reservation/GuestReservation/${encoded}`;
}


function AddGuestsEnquiryConfirmation() {
    Swal.fire({
        title: '',
        text: 'May I know your level of interest in proceeding with this offer',
        icon: 'question',
        showCancelButton: true,
        showDenyButton: true,
        confirmButtonText: 'Interested',
        denyButtonText: 'Call Later',
        cancelButtonText: 'Not Interested',
        customClass: {
            confirmButton: 'btn btn-success mx-1',
            denyButton: 'btn btn-warning mx-1',
            cancelButton: 'btn btn-danger mx-1'
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed) {
            AddGuestsEnquiry(3);
            // User is Interested
        } else if (result.isDenied) {
            AddGuestsEnquiry(2);
            // User wants to Call Later
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            AddGuestsEnquiry(1);
            // User is Not Interested
        }
    });
}
function AddGuestsEnquiry(ScheduleType) {

    if (!_isValidateFormCRM("AddGuestFormCRM")) {
        return;
    }

    let GuestForm = $("#AddGuestFormCRM").find("[dbCol]");
    let dataVM = new FormData();
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
    dataVM.append("MemberDetail.Remarks", $("[name='MembersDetails.Remarks']").text());

    dataVM.append("MemberDetail.ScheduleType", ScheduleType);

    BlockUI();
    $.ajax({
        url: '/CRM/SaveMemberEnquiryDetails',
        data: dataVM,
        //dataType: "json",
        async: false,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (response) {

            $sadConfirmation("Success", 'Saved Successfully!', 'Ok', 'smile', function () {
                window.location.reload();
            });


            UnblockUI();
        },
        error: function (xhr, ajaxOptions, error) {
            UnblockUI();
            $erroralert("Data Submission Error!", xhr.responseText + '!');
            //alert(xhr.responseText);
            //responseText
        }
    });




}

//function CalculateAge1() {
//    if ($("#AddGuestFormCRM").find("[name='MemberDetail.Dob']").val() != '') {
//        var date1 = $("#AddGuestFormCRM").find("[name='MemberDetail.Dob']").val().split('-');
//        date1 = new Date(date1[2], date1[1] - 1, date1[0]);
//        var date2 = new Date();
//        var timediff = Math.abs(date2.getTime() - date1.getTime());

//        var ageDate = new Date(timediff); // miliseconds from epoch
//        ageDate = Math.abs(ageDate.getUTCFullYear() - 1970);

//        var daydiff = timediff / (1000 * 3600 * 24);

//        daydiff = Math.round(daydiff);
//        $("#AddGuestFormCRM").find("[name='MemberDetail.Age']").val(ageDate);
//    }
//}

function fetchStateCRM(CountryId, StateId) {
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
function fetchCityCRM(stateId, cityId) {
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


function deleteUploadedFileCRM(Id) {
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
function downloadAttachmentCRM(fileId, filename) {
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



