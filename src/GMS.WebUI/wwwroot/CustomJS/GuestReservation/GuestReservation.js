function AddGuestsPartialView(GroupId = '', PAXSno = 1) {
    $('#div_GuestReservationForm_PartialView').empty();
    //let inputDTO = {};
    //inputDTO.GroupId = GroupId;
    //inputDTO.PAXSno = PAXSno;



    let MemberDetail = {};
    MemberDetail.GroupId = GroupId;
    MemberDetail.PAXSno = PAXSno;

    let GuestReservationRouteValues = {};
    GuestReservationRouteValues.PageSource = $("#AddGuestForm").find("[name='GuestReservationRouteValues.PageSource']").val();
    GuestReservationRouteValues.RoomNumber = $("#AddGuestForm").find("[name='GuestReservationRouteValues.RoomNumber']").val();
    //GuestReservationRouteValues.RoomType = $("#AddGuestForm").find("[name='GuestReservationRouteValues.RoomType']").val();
    GuestReservationRouteValues.RoomType = Number($("#AddGuestForm").find("[name='GuestReservationRouteValues.RoomType']").val()) || 0;

    let inputData = {};
    inputData.MemberDetail = MemberDetail;
    inputData.GuestReservationRouteValues = GuestReservationRouteValues;

    BlockUI();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Reservation/AddGuestsPartialView',
        //data: JSON.stringify(inputDTO),
        data: JSON.stringify(inputData),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_GuestReservationForm_PartialView').html(data);

            const formId = "AddGuestForm";

            initTabs()

            $("[name='MemberDetail.PAXSno']").val(PAXSno);
            $("[name='spnPAXSno']").text(PAXSno);

            initDatesByFormId(formId);

            initValidateRoomsAvailabilityByFormId(formId)

            GetRoomRatesForEnquiryByFormId(formId);

            //GetServicesByFormId(formId);



            getOnLoadAddGuests(formId);



            $("select").chosen({
                width: '100%'
            });

            $form = $("#" + formId);

            if ($form.find("[name='GuestReservationRouteValues.PageSource']").val() == "RoomAvailability") {
                $form.find("[name='MemberDetail.RoomType']").closest(".form-group").hide();
                $form.find("[name='MemberDetail.Pax']").closest(".form-group").hide();
                $form.find("[name='MemberDetail.NoOfRooms']").closest(".form-group").hide();

                $form.find("[name='MemberDetail.Pax']").val(1);
                $form.find("[name='MemberDetail.NoOfRooms']").val(1);
            }


        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}


function initTabs() {
    //$('#guestFormTabs a').on('click', function (e) {
    //    e.preventDefault();
    //    $(this).tab('show');
    //});
    $('#guestFormTabs a').on('click', function (e) {
        const currentTab = $('.nav-link.active');
        const targetTab = $(this);

        // Only validate when moving forward (not backward)
        if (targetTab.parent().index() > currentTab.parent().index()) {
            if (!_isValidateForm("GuestInformationTab")) {
                e.preventDefault(); // Stop tab switch if validation fails
                return false;
            }
        }

        // Proceed with normal tab switch if validation passes or moving back
        $(this).tab('show');
    });
}




function initDatesByFormId(formId) {
    InitialiseMobileFields();
    $("#" + formId).find("[name='MemberDetail.CatId']").change();
    $("#" + formId).find("[name='MemberDetail.Dob']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#" + formId).find("[name='MemberDetail.DateOfArrival']").datetimepicker({
        format: 'd-m-Y H:i',
        timepicker: true,
        defaultTime: '14:00'
    });
    //$("#" + formId).find("[name='MemberDetail.DateOfDepartment']").datetimepicker({
    //    format: 'd-m-Y',
    //    step: 30
    //});
    $("#" + formId).find("[name='TimeOfDepartment']").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 30 // Optional: Define minute intervals
    });
    $("#" + formId).find("[name='MemberDetail.FlightArrivalDateAndDateTime']").datetimepicker({
        format: 'd-m-Y H:i',
        //timepicker: false,
    });
    $("#" + formId).find("[name='MemberDetail.FlightDepartureDateAndDateTime']").datetimepicker({
        format: 'd-m-Y H:i',
        //timepicker: false,
    });
    $("#" + formId).find("[name='MemberDetail.HoldTillDate']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#" + formId).find("[name='MemberDetail.PaymentDate']").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });

    $("#" + formId).find("[name='MemberDetail.Dob']").change(function () {
        CalculateAge(formId);
    });
    $("#" + formId).find('[name="MemberDetail.Pax"]').on("propertychange change keyup paste input", function () {
        $("#" + formId).find('[name="MemberDetail.NoOfRooms"]').val($("#" + formId).find('[name="MemberDetail.Pax"]').val());
        GetRoomRatesForEnquiryByFormId(formId);
    });
    $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').on("propertychange change keyup paste input", function () {
        FetchDepartureDateByFormId($("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val(), "AN", formId);
    });
    $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').change(function () {
        //if (!$("#" + formId).find("[name='GuestReservationRouteValues.PageSource']").val("RoomAvailability")) {
        //    FetchDepartureDateByFormId($(this).val(), "AD", formId);
        //}
        FetchDepartureDateByFormId($(this).val(), "AD", formId);
        GetRoomRatesForEnquiryByFormId(formId);
    });
    $("#" + formId).find('[name="MemberDetail.ServiceId"]').change(function () {
        FetchDepartureDateByFormId($("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val(), "NN", formId);
        GetRoomRatesForEnquiryByFormId(formId);
    });
    $("#" + formId).find('[name="MemberDetail.RoomType"]').change(function () {
        GetRoomRatesForEnquiryByFormId(formId);
    });
    $("#" + formId).find('[name="MemberDetail.CatId"]').change(function () {
        FetchDepartureDateByFormId($("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val(), "NN", formId);

        GetRoomRatesForEnquiryByFormId(formId);
    });


    $("#" + formId).find(".IssueDate").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });
    $("#" + formId).find(".ExpiryDate").datetimepicker({
        format: 'd-m-Y',
        timepicker: false,
    });

    $("#" + formId).find('[name="MemberDetail.PickUpDrop"]').change(function () {
        if ($(this).val() == 1) {
            $("#" + formId).find('#divddlPickUp').show();
            $("#" + formId).find('#divddlCarType').hide();
            $("#" + formId).find('#divtxtPickUpLocation').hide();
            $("#" + formId).find('#divFlightArrivalDateAndTime').hide();
            $("#" + formId).find('#divFlightDepartureDateAndTime').hide();
        }
        else {
            $("#" + formId).find('#divddlPickUp').hide();
            $("#" + formId).find('#divddlCarType').hide();
            $("#" + formId).find('#divtxtPickUpLocation').hide();
            $("#" + formId).find('#divFlightArrivalDateAndTime').hide();
            $("#" + formId).find('#divFlightDepartureDateAndTime').hide();
        }
    });
    $("#" + formId).find('[name="MemberDetail.PickUpType"]').change(function () {
        if ($(this).val() == 2) {
            $("#" + formId).find('#divddlCarType').show();
            $("#" + formId).find('#divtxtPickUpLocation').show();
            $("#" + formId).find('#divFlightArrivalDateAndTime').hide();
            $("#" + formId).find('#divFlightDepartureDateAndTime').hide();
        }
        else if ($(this).val() == 1) {
            $("#" + formId).find('#divddlCarType').show();
            $("#" + formId).find('#divtxtPickUpLocation').hide();
            $("#" + formId).find('#divFlightArrivalDateAndTime').show();
            $("#" + formId).find('#divFlightDepartureDateAndTime').show();
        }
        else {
            $("#" + formId).find('#divddlCarType').hide();
            $("#" + formId).find('#divtxtPickUpLocation').hide();
            $("#" + formId).find('#divFlightArrivalDateAndTime').hide();
            $("#" + formId).find('#divFlightDepartureDateAndTime').hide();
        }
    });
    $("#" + formId).find('[name="MemberDetail.GuarenteeCode"]').change(function () {
        FetchPaymentStatus($(this).val());
    });

    $("#" + formId).find('[name="MemberDetail.PaymentStatus"]').change(function () {
        if ($(this).val() == 1) {
            $("#" + formId).find('#divcalHoldTillDate').hide();
            $("#" + formId).find('#divcalPaymentDate').show();
        }
        else if ($(this).val() == 2) {
            $("#" + formId).find('#divcalHoldTillDate').show();
            $("#" + formId).find('#divcalPaymentDate').hide();
        }
        else if ($(this).val() == 3) {
            $("#" + formId).find('#divcalHoldTillDate').hide();
            $("#" + formId).find('#divcalPaymentDate').show();
        }
        else if ($(this).val() == 4) {
            $("#" + formId).find('#divcalHoldTillDate').hide();
            $("#" + formId).find('#divcalPaymentDate').hide();
        }
        else {
            $("#" + formId).find('#divcalHoldTillDate').hide();
            $("#" + formId).find('#divcalPaymentDate').hide();
        }
    });
    $("#" + formId).find('#ddlWantToSharePhoto').change(function () {
        if ($(this).val() == "false")
            $("#" + formId).find('#divUploadPhoto').hide();
        else if ($(this).val() == "true")
            $("#" + formId).find('#divUploadPhoto').show();
        else if ($(this).val() == "false")
            $("#" + formId).find('#divUploadPhoto').hide();
    });

    $("#" + formId).find('[name="MemberDetail.Idproof"]').on("propertychange change keyup paste input", function () {
        if ($("#" + formId).find('[name="MemberDetail.Idproof"]').val().length > 0)
            $("#" + formId).find('#divUploadIdProofDetails').show();
        else
            $("#" + formId).find('#divUploadIdProofDetails').hide();
    });
    $("#" + formId).find('[name="MemberDetail.PassportNo"]').on("propertychange change keyup paste input", function () {
        if ($("#" + formId).find('[name="MemberDetail.PassportNo"]').val().length > 0)
            $("#" + formId).find('#divUploadPassportDetails').show();
        else
            $("#" + formId).find('#divUploadPassportDetails').hide();
    });
    $("#" + formId).find('[name="MemberDetail.VisaDetails"]').on("propertychange change keyup paste input", function () {
        if ($("#" + formId).find('[name="MemberDetail.VisaDetails"]').val().length > 0)
            $("#" + formId).find('#divUploadVisaDetails').show();
        else
            $("#" + formId).find('#divUploadVisaDetails').hide();
    });
    $('[name="MemberDetail.NationalityId"]').change(function () {
        let nationality = $("#" + formId).find("[name='MemberDetail.NationalityId'] option:selected").text();

        if (nationality == "Indian") {
            $("#" + formId).find("[name='MemberDetail.Idproof']").parent().find('label').addClass('required');
            $("#" + formId).find("[name='MemberDetail.Idproof']").addClass('requiredInput');
            //$("[name='MemberDetail.IdProofIssueDate']").parent().find('label').addClass('required');
            //$("[name='MemberDetail.IdProofIssueDate']").addClass('requiredInput');
            //$("[name='MemberDetail.IdProofExpiryDate']").parent().find('label').addClass('required');
            //$("[name='MemberDetail.IdProofExpiryDate']").addClass('requiredInput');


            $("#" + formId).find("[name='MemberDetail.PassportNo']").parent().find('label').removeClass('required');
            $("#" + formId).find("[name='MemberDetail.PassportNo']").removeClass('requiredInput');
            $("#" + formId).find("[name='MemberDetail.PassportIssueDate']").parent().find('label').removeClass('required');
            $("#" + formId).find("[name='MemberDetail.PassportIssueDate']").removeClass('requiredInput');
            $("#" + formId).find("[name='MemberDetail.PassportExpiryDate']").parent().find('label').removeClass('required');
            $("#" + formId).find("[name='MemberDetail.PassportExpiryDate']").removeClass('requiredInput');

            $("#" + formId).find("[name='MemberDetail.VisaDetails']").parent().find('label').removeClass('required');
            $("#" + formId).find("[name='MemberDetail.VisaDetails']").removeClass('requiredInput');
            $("#" + formId).find("[name='MemberDetail.VisaIssueDate']").parent().find('label').removeClass('required');
            $("#" + formId).find("[name='MemberDetail.VisaIssueDate']").removeClass('requiredInput');
            $("#" + formId).find("[name='MemberDetail.VisaExpiryDate']").parent().find('label').removeClass('required');
            $("#" + formId).find("[name='MemberDetail.VisaExpiryDate']").removeClass('requiredInput');
        }
        else {
            $("#" + formId).find("[name='MemberDetail.Idproof']").parent().find('label').removeClass('required');
            $("#" + formId).find("[name='MemberDetail.Idproof']").removeClass('requiredInput');
            //$("[name='MemberDetail.IdProofIssueDate']").parent().find('label').removeClass('required');
            //$("[name='MemberDetail.IdProofIssueDate']").removeClass('requiredInput');
            //$("[name='MemberDetail.IdProofExpiryDate']").parent().find('label').removeClass('required');
            //$("[name='MemberDetail.IdProofExpiryDate']").removeClass('requiredInput');

            $("#" + formId).find("[name='MemberDetail.PassportNo']").parent().find('label').addClass('required');
            $("#" + formId).find("[name='MemberDetail.PassportNo']").addClass('requiredInput');
            $("#" + formId).find("[name='MemberDetail.PassportIssueDate']").parent().find('label').addClass('required');
            $("#" + formId).find("[name='MemberDetail.PassportIssueDate']").addClass('requiredInput');
            $("#" + formId).find("[name='MemberDetail.PassportExpiryDate']").parent().find('label').addClass('required');
            $("#" + formId).find("[name='MemberDetail.PassportExpiryDate']").addClass('requiredInput');

            $("#" + formId).find("[name='MemberDetail.VisaDetails']").parent().find('label').addClass('required');
            $("#" + formId).find("[name='MemberDetail.VisaDetails']").addClass('requiredInput');
            $("#" + formId).find("[name='MemberDetail.VisaIssueDate']").parent().find('label').addClass('required');
            $("#" + formId).find("[name='MemberDetail.VisaIssueDate']").addClass('requiredInput');
            $("#" + formId).find("[name='MemberDetail.VisaExpiryDate']").parent().find('label').addClass('required');
            $("#" + formId).find("[name='MemberDetail.VisaExpiryDate']").addClass('requiredInput');
        }

    });

    $("#" + formId).find('#ddlWantToSharePhoto').change();
    $("#" + formId).find('[name="MemberDetail.Idproof"]').change();
    $("#" + formId).find('[name="MemberDetail.PassportNo"]').change();
    $("#" + formId).find('[name="MemberDetail.VisaDetails"]').change();
    $("#" + formId).find('[name="MemberDetail.NationalityId"]').change();


}

function getOnLoadAddGuests(formId) {
    CalculateAge(formId);
    $("#" + formId).find('[name="MemberDetail.PickUpDrop"]').change();
    $("#" + formId).find('[name="MemberDetail.PickUpLoaction"]').change();
    $("#" + formId).find('[name="MemberDetail.GuarenteeCode"]').change();
    $("#" + formId).find('[name="MemberDetail.PaymentStatus"]').change();
    $("#" + formId).find("[name='MemberDetail.CountryId']").change();


    //$("#" + formId).find("[name='MemberDetail.CatId']").change();

    //if (!$("#" + formId).find("[name='GuestReservationRouteValues.PageSource']").val("RoomAvailability")) {
    //    $("#" + formId).find("[name='MemberDetail.CatId']").change();
    //}

    GetServicesByFormId(0, formId);

    $("#" + formId).find("[name='MemberDetail.CatId']").change();

    //if (!($("#" + formId).find("[name='GuestReservationRouteValues.PageSource']").val() == "RoomAvailability")) {
    //    $("#" + formId).find("[name='MemberDetail.CatId']").change();
    //}

}

function GetServicesByFormId(serviceId, formId) {
    let minNight = $("#" + formId).find("[name='MemberDetail.CatId'] option:selected").attr('minNight');
    let maxNight = $("#" + formId).find("[name='MemberDetail.CatId'] option:selected").attr('maxNight');
    let price = $("#" + formId).find("[name='MemberDetail.CatId'] option:selected").attr('price');

    let $noOfNights = $("#" + formId).find("[name='MemberDetail.ServiceId']");
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

    $("#" + formId).find("[name='MemberDetail.ServiceId']").change();
}


function FetchDepartureDateByFormId1(ArrivalDate, Source, formId) {
    if (ArrivalDate == "") {
        return;
    }
    var AdditionalNights = 0;
    if (Source == "AD") {
        if (ArrivalDate == "" || ArrivalDate == null || ArrivalDate == undefined || ArrivalDate == "0") {
            $erroralert("Invalid Date!", 'Please select valid date!');
            //alert("Please select valid date", "error");
            $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val('');
            return false;
        }
    }
    if ($("#" + formId).find('[name="MemberDetail.ServiceId"]').val() == "" || $("#" + formId).find('[name="MemberDetail.ServiceId"]').val() == null || $("#" + formId).find('[name="MemberDetail.ServiceId"]').val() == undefined || $("#" + formId).find('[name="MemberDetail.ServiceId"]').val() == "0") {
        $erroralert("Package not found!", 'Please select select package first!');

        $("#" + formId).find('[name="MemberDetail.ServiceId"]').focus();
        $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val('');
        return false;
    }
    if ($("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val() == "" || $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val() == null || $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val() == undefined || $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val() == "0") {
        AdditionalNights = 0;
    }
    else
        AdditionalNights = $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val();

    let NoOfNights = $("#" + formId).find('[name="MemberDetail.ServiceId"]').val();

    if (NoOfNights == null || NoOfNights === undefined || isNaN(NoOfNights)) {
        $erroralert("Error!", 'Please select number of nights');
    }

    NoOfNights = parseInt(NoOfNights);

    let NoOfTotalNights = NoOfNights + parseInt(AdditionalNights);

    let departureDate = moment(ArrivalDate, "DD/MM/YYYY").add(NoOfTotalNights, 'days');
    $("#" + formId).find('[name="MemberDetail.DateOfDepartment"]').val(departureDate.format("DD-MM-YYYY"));
    $("#" + formId).find('[name="TimeOfDepartment"]').val("11:00")

}


function FetchDepartureDateByFormId(ArrivalDate, Source, formId) {

    let ArrivalDate1 = $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val();
    let AdditionalNights1 = $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val();
    let NoOfNights1 = $("#" + formId).find('[name="MemberDetail.ServiceId"]').val();

    if (ArrivalDate1 == "" || ArrivalDate1 == null || ArrivalDate1 == undefined || ArrivalDate1 == "0") {
        return false;
    }
    if (NoOfNights1 == null || NoOfNights1 === undefined || isNaN(NoOfNights1)) {
        return false;
    }
    if (AdditionalNights1 == null || AdditionalNights1 === undefined || isNaN(AdditionalNights1) || $.trim(AdditionalNights1) == "") {
        AdditionalNights1 = 0;
    }

    let NoOfTotalNights1 = parseInt(NoOfNights1) + parseInt(AdditionalNights1);

    let departureDate1 = moment(ArrivalDate1, "DD/MM/YYYY").add(NoOfTotalNights1, 'days');
    $("#" + formId).find('[name="MemberDetail.DateOfDepartment"]').val(departureDate1.format("DD-MM-YYYY"));
    $("#" + formId).find('[name="TimeOfDepartment"]').val("11:00")


    return;


    if (ArrivalDate == "") {
        return;
    }
    var AdditionalNights = 0;
    if (Source == "AD") {
        if (ArrivalDate == "" || ArrivalDate == null || ArrivalDate == undefined || ArrivalDate == "0") {
            $erroralert("Invalid Date!", 'Please select valid date!');
            //alert("Please select valid date", "error");
            $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val('');
            return false;
        }
    }
    if ($("#" + formId).find('[name="MemberDetail.ServiceId"]').val() == "" || $("#" + formId).find('[name="MemberDetail.ServiceId"]').val() == null || $("#" + formId).find('[name="MemberDetail.ServiceId"]').val() == undefined || $("#" + formId).find('[name="MemberDetail.ServiceId"]').val() == "0") {
        $erroralert("Package not found!", 'Please select select package first!');

        $("#" + formId).find('[name="MemberDetail.ServiceId"]').focus();
        $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val('');
        return false;
    }
    if ($("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val() == "" || $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val() == null || $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val() == undefined || $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val() == "0") {
        AdditionalNights = 0;
    }
    else
        AdditionalNights = $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').val();

    let NoOfNights = $("#" + formId).find('[name="MemberDetail.ServiceId"]').val();

    if (NoOfNights == null || NoOfNights === undefined || isNaN(NoOfNights)) {
        $erroralert("Error!", 'Please select number of nights');
    }

    NoOfNights = parseInt(NoOfNights);

    let NoOfTotalNights = NoOfNights + parseInt(AdditionalNights);

    let departureDate = moment(ArrivalDate, "DD/MM/YYYY").add(NoOfTotalNights, 'days');
    $("#" + formId).find('[name="MemberDetail.DateOfDepartment"]').val(departureDate.format("DD-MM-YYYY"));
    $("#" + formId).find('[name="TimeOfDepartment"]').val("11:00")

}


function initValidateRoomsAvailabilityByFormId(formId) {
    //CheckInputs();

    $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').change(function () {
        ValidateRoomsAvailabilityByFormId(formId).then((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#" + formId).find('[name="MemberDetail.DateOfDepartment"]').change(function () {
        ValidateRoomsAvailabilityByFormId(formId).then((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#" + formId).find('[name="TimeOfDepartment"]').change(function () {
        ValidateRoomsAvailabilityByFormId(formId).then((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#" + formId).find('[name="MemberDetail.RoomType"]').change(function () {
        ValidateRoomsAvailabilityByFormId(formId).then((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#" + formId).find('[name="MemberDetail.Pax"]').change(function () {
        ValidateRoomsAvailabilityByFormId(formId).then((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
    $("#" + formId).find('[name="MemberDetail.NoOfRooms"]').change(function () {

        GetRoomRatesForEnquiryByFormId(formId);

        ValidateRoomsAvailabilityByFormId(formId).then((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "green");

        }).catch((d) => {
            $("#" + formId).find('[name="RoomsAvailabilityInformation"]').text(d).css("color", "red");
        });
    });
}

function ValidateRoomsAvailabilityByFormId(formId) {
    return new Promise((resolve, reject) => {
        let sourceDateFormat = "YYYY-MM-DDTHH:mm";
        let DateOfArrivalObj = $("#" + formId).find('[name="MemberDetail.DateOfArrival"]');
        let DateOfArrival = $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val();
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
        let DateOfDepartmentObj = $("#" + formId).find('[name="MemberDetail.DateOfDepartment"]');
        let DateOfDepartment = $("#" + formId).find('[name="MemberDetail.DateOfDepartment"]').val();
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



        let TimeOfDepartment = $("#" + formId).find('[name="TimeOfDepartment"]').val();
        let RoomType = $("#" + formId).find('[name="MemberDetail.RoomType"]').val();
        let Pax = $("#" + formId).find('[name="MemberDetail.Pax"]').val();
        let NoOfRooms = $("#" + formId).find('[name="MemberDetail.NoOfRooms"]').val();
        let GuestId = $("#" + formId).find("[name='MemberDetail.Id']").val();
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



function GetRoomRatesForEnquiryByFormId(formId) {
    var inputDTO = {};

    let _dateOfArrival = $("#" + formId).find("[name='MemberDetail.DateOfArrival']");
    let _dateOfArrivalValue = $("#" + formId).find("[name='MemberDetail.DateOfArrival']").val() || null;
    if (_dateOfArrival.hasClass('datetimeonly') && _dateOfArrivalValue != "" && _dateOfArrivalValue != null) {
        try {
            let dateformat = _dateOfArrival.attr("dateformat");
            let currentDateFormat = "DD-MM-YYYY";
            let sourceDateFormat = "YYYY-MM-DD";
            _dateOfArrivalValue = moment(_dateOfArrivalValue, currentDateFormat).format(sourceDateFormat);
        }
        catch {
            _dateOfArrivalValue = null;
        }
    }

    inputDTO.PlanId = $("#" + formId).find("[name='MemberDetail.CatId']").val() || 0;
    inputDTO.NoOfNights = $("#" + formId).find("[name='MemberDetail.ServiceId']").val() || 0;
    inputDTO.RateDate = _dateOfArrivalValue;
    inputDTO.RoomTypeId = $("#" + formId).find("[name='MemberDetail.RoomType']").val() || 0;
    inputDTO.NoOfRooms = $("#" + formId).find("[name='MemberDetail.NoOfRooms']").val() || 0;

    if (!(inputDTO.PlanId != 0 && inputDTO.NoOfNights != 0 && inputDTO.RateDate != null && inputDTO.RoomTypeId != 0 && inputDTO.NoOfRooms != 0)) {
        return;
    }

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/CRM/RoomRatesForEnquiry',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#" + formId).find("[name='div_RatePlanPartial']").html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}


function InitialiseMobileFields() {
    var txtmobile = document.querySelector("#MemberDetail_MobileNo");
    var initxtmobile = window.intlTelInput(txtmobile, {
        autoHideDialCode: false,
        nationalMode: false,
        initialCountry: "in",
        utilsScript: "../assets/CustomComponents/CountryCode/js/utils.js",
        //separateDialCode: true
    });

    //let element = document.querySelector('#MemberDetail_MobileNo');
    //if (element) {
    //    let currentPaddingLeft = window.getComputedStyle(element).getPropertyValue('padding-left');
    //    element.style.setProperty('padding-left', '100px ', '!important');
    //}
}

function CalculateAge(formId) {
    if ($("#" + formId).find("[name='MemberDetail.Dob']").val() != '') {
        var date1 = $("#" + formId).find("[name='MemberDetail.Dob']").val().split('-');
        date1 = new Date(date1[2], date1[1] - 1, date1[0]);
        var date2 = new Date();
        var timediff = Math.abs(date2.getTime() - date1.getTime());

        var ageDate = new Date(timediff); // miliseconds from epoch
        ageDate = Math.abs(ageDate.getUTCFullYear() - 1970);

        var daydiff = timediff / (1000 * 3600 * 24);

        daydiff = Math.round(daydiff);
        $("#" + formId).find("[name='MemberDetail.Age']").val(ageDate);
    }
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