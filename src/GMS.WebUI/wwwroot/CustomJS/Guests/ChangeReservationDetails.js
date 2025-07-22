function ChangeReservationDetailsPartialView(Id) {
    let inputDTO = {}
    inputDTO.Id = Id;
    //inputDTO.opt = opt;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Guests/ChangeReservationDetailsPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            const formId = "ChangeGuestDetailsForm";
            UnblockUI();
            $('#div_ChangeReservationDetailsPartial').html(data);
            $("#btnChangeReservationDetailsModal").click();

            initDatesByFormId(formId);

            initValidateRoomsAvailabilityByFormId(formId)

            GetRoomRatesForEnquiryByFormId(formId);

            GetServicesByFormId(formId);

        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
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
        CalculateAge();
    });
    $("#" + formId).find('[name="MemberDetail.Pax"]').on("propertychange change keyup paste input", function () {
        $("#" + formId).find('[name="MemberDetail.NoOfRooms"]').val($("#" + formId).find('[name="MemberDetail.Pax"]').val());
        GetRoomRatesForEnquiryByFormId(formId);
    });
    $("#" + formId).find('[name="MemberDetail.AdditionalNights"]').on("propertychange change keyup paste input", function () {
        FetchDepartureDateByFormId($("#" + formId).find('[name="MemberDetail.DateOfArrival"]').val(), "AN", formId);
    });
    $("#" + formId).find('[name="MemberDetail.DateOfArrival"]').change(function () {
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
}


function FetchDepartureDateByFormId(ArrivalDate, Source, formId) {
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

function UpdateGuestReservationDetails(formId) {
    if (!_isValidateForm(formId)) {
        return;
    }

    ValidateRoomsAvailabilityByFormId(formId).then((d) => {
        let GuestForm = $("#" + formId).find("[dbCol]");

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
        //dataVM.append("MemberDetail.PAXSno", $("#AddGuestForm").find("[name='MemberDetail.PAXSno']").val());
        BlockUI();
        $.ajax({
            url: '/Guests/UpdateGuestReservationDetails',
            data: dataVM,
            //dataType: "json",
            async: false,
            type: 'POST',
            processData: false,
            contentType: false,
            success: function (response) {
                UnblockUI();
                $("#ChangeReservationDetailsModal").find(".btn-close").click();
                $successalert("", "Details Changed Successfully!");
                GuestsListPartialView();
                //if (response != null) {
                //    if (response.paxSno < response.pax) {
                //        //$("[name='MemberDetail.PAXSno']").val(parseInt(response?.pAXSno) + 1);
                //        AddGuestsPartialView(response.groupId, parseInt(response?.paxSno) + 1);
                //    }
                //    else {
                //        if (response.opt == "Update") {
                //            //GuestsListPartialView(1);
                //            window.location.href = "/Guests/GuestRegistrationSuccessfull/" + response.groupId;
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