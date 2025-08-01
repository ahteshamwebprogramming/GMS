﻿$(document).ready(function () {

    //$successalert('title', 'message');

    initDates();
    MasterScheduleListPartialView();
    //GuestsListPartialView();
});
function initDates() {
    //InitialiseMobileFields();
    //$("#AddSchedule").find("[name='MemberDetail.Dob']").datetimepicker({
    //    format: 'd-m-Y',
    //    timepicker: false,
    //});
    //$("#AddSchedule").find("[name='MemberDetail.DateOfArrival']").datetimepicker({
    //    format: 'd-m-Y H:i',
    //    timepicker: true,
    //});
    ////$("#AddGuestForm").find("[name='MemberDetail.DateOfDepartment']").datetimepicker({
    ////    format: 'd-m-Y',
    ////    step: 30
    ////});
    $("#AddSchedule").find("[name='StartTime']").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 10 // Optional: Define minute intervals
    });
    $("#AddSchedule").find("[name='EndTime']").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 10 // Optional: Define minute intervals
    });
    $("#AddSchedule").find("[name='Duration']").datetimepicker({
        datepicker: false,
        format: 'H:i',
        step: 5, // Optional: Define minute intervals
        minTime: '00:05'
    });
    //$("#AddGuestForm").find("[name='MemberDetail.FlightArrivalDateAndDateTime']").datetimepicker({
    //    format: 'd-m-Y H:i',
    //    //timepicker: false,
    //});
    //$("#AddGuestForm").find("[name='MemberDetail.FlightDepartureDateAndDateTime']").datetimepicker({
    //    format: 'd-m-Y H:i',
    //    //timepicker: false,
    //});
    //$("#AddGuestForm").find("[name='MemberDetail.HoldTillDate']").datetimepicker({
    //    format: 'd-m-Y',
    //    timepicker: false,
    //});
    //$("#AddGuestForm").find("[name='MemberDetail.PaymentDate']").datetimepicker({
    //    format: 'd-m-Y',
    //    timepicker: false,
    //});

    //$("#AddGuestForm").find("[name='MemberDetail.Dob']").change(function () {
    //    CalculateAge();
    //});
    //$('[name="MemberDetail.Pax"]').on("propertychange change keyup paste input", function () {
    //    $('[name="MemberDetail.NoOfRooms"]').val($('[name="MemberDetail.Pax"]').val());
    //});
    //$('[name="MemberDetail.AdditionalNights"]').on("propertychange change keyup paste input", function () {
    //    FetchDepartureDate($('[name="MemberDetail.DateOfArrival"]').val(), "AN");
    //});
    //$('[name="MemberDetail.DateOfArrival"]').change(function () {
    //    FetchDepartureDate($(this).val(), "AD");
    //});
    //$('[name="MemberDetail.PickUpDrop"]').change(function () {
    //    if ($(this).val() == 1) {
    //        $('#divddlPickUp').show();
    //        $('#divddlCarType').hide();
    //        $('#divtxtPickUpLocation').hide();
    //        $('#divFlightArrivalDateAndTime').hide();
    //        $('#divFlightDepartureDateAndTime').hide();
    //    }
    //    else {
    //        $('#divddlPickUp').hide();
    //        $('#divddlCarType').hide();
    //        $('#divtxtPickUpLocation').hide();
    //        $('#divFlightArrivalDateAndTime').hide();
    //        $('#divFlightDepartureDateAndTime').hide();
    //    }
    //});
    //$('[name="MemberDetail.PickUpType"]').change(function () {
    //    if ($(this).val() == 2) {
    //        $('#divddlCarType').show();
    //        $('#divtxtPickUpLocation').show();
    //        $('#divFlightArrivalDateAndTime').hide();
    //        $('#divFlightDepartureDateAndTime').hide();
    //    }
    //    else if ($(this).val() == 1) {
    //        $('#divddlCarType').show();
    //        $('#divtxtPickUpLocation').hide();
    //        $('#divFlightArrivalDateAndTime').show();
    //        $('#divFlightDepartureDateAndTime').show();
    //    }
    //    else {
    //        $('#divddlCarType').hide();
    //        $('#divtxtPickUpLocation').hide();
    //        $('#divFlightArrivalDateAndTime').hide();
    //        $('#divFlightDepartureDateAndTime').hide();
    //    }
    //});
    //$('[name="MemberDetail.GuarenteeCode"]').change(function () {
    //    FetchPaymentStatus($(this).val());
    //});
    //$('[name="MemberDetail.PaymentStatus"]').change(function () {
    //    if ($(this).val() == 1) {
    //        $('#divcalHoldTillDate').hide();
    //        $('#divcalPaymentDate').show();
    //    }
    //    else if ($(this).val() == 2) {
    //        $('#divcalHoldTillDate').show();
    //        $('#divcalPaymentDate').hide();
    //    }
    //    else if ($(this).val() == 3) {
    //        $('#divcalHoldTillDate').hide();
    //        $('#divcalPaymentDate').show();
    //    }
    //    else if ($(this).val() == 4) {
    //        $('#divcalHoldTillDate').hide();
    //        $('#divcalPaymentDate').hide();
    //    }
    //    else {
    //        $('#divcalHoldTillDate').hide();
    //        $('#divcalPaymentDate').hide();
    //    }
    //});
    //$('#ddlWantToSharePhoto').change(function () {
    //    if ($(this).val() == 0)
    //        $('#divUploadPhoto').hide();
    //    else if ($(this).val() == 1)
    //        $('#divUploadPhoto').show();
    //    else if ($(this).val() == 2)
    //        $('#divUploadPhoto').hide();
    //});

    //$('[name="MemberDetail.Idproof"]').on("propertychange change keyup paste input", function () {
    //    if ($('[name="MemberDetail.Idproof"]').val().length > 3)
    //        $('#divUploadProof').show();
    //    else
    //        $('#divUploadProof').hide();
    //});
    //$('[name="MemberDetail.PassportNo"]').on("propertychange change keyup paste input", function () {
    //    if ($('[name="MemberDetail.PassportNo"]').val().length > 3)
    //        $('#divUploadVisa').show();
    //    else
    //        $('#divUploadVisa').hide();
    //});


    $("#AddSchedule").find("[name='StartTime']").change(function () {
        CalculateEndTime();
    });
    $("#AddSchedule").find("[name='Duration']").change(function () {
        CalculateEndTime();
    });
}

function CalculateEndTime() {
    let startTime = $("#AddSchedule").find("[name='StartTime']").val();
    let duration = $("#AddSchedule").find("[name='Duration']").val();

    if (!startTime || !duration) {
        // Handle invalid input (optional)
        $("#AddSchedule").find("[name='EndTime']").val("Invalid input");
        return;
    }
    // Parse StartTime and Duration
    let startMoment = moment(startTime, "HH:mm"); // Create a Moment.js object for StartTime
    let durationParts = duration.split(":").map(Number); // Split Duration into [hours, minutes]

    // Add Duration to StartTime
    let endMoment = startMoment
        .add(durationParts[0], "hours")  // Add hours
        .add(durationParts[1], "minutes")  // Add minutes
        .subtract(1, "seconds"); // Subtract 1 second

    // Format EndTime as HH:mm and set the value
    $("#AddSchedule").find("[name='EndTime']").val(endMoment.format("HH:mm"));

    /*$("#AddSchedule").find("[name='EndTime']").val("00:10");*/
}
function getOnLoadAddGuests() {
    CalculateAge();
    $('[name="MemberDetail.PickUpDrop"]').change();
    $('[name="MemberDetail.PickUpLoaction"]').change();
    $('[name="MemberDetail.GuarenteeCode"]').change();
    $('[name="MemberDetail.PaymentStatus"]').change();

    GetServices();

}

function MasterScheduleListPartialView() {

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/ScheduleMaster/MasterScheduleListPartialView',
        //data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_MasterScheduleListPartial').html(data);
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}



function AddGuestsPartialView(Id = 0) {

    let inputDTO = {};
    inputDTO.Id = Id;

    BlockUI();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/ScheduleMaster/AddMasterSchedulePartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();

            $('#div_MasterScheduleAddPartial').html(data);
            $("#btnOpenAddMasterSchedilePopup").click();
            initDates();
            //getOnLoadAddGuests();
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
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


}




function isValidForm(formId) {

    let res = isValidateForm(formId);

    return res;
}
function AddSchedule() {

    if (!isValidForm("AddSchedule")) {
        return;
    }
    //alert("Success");
    //return;

    let ScheduleForm = $("#AddSchedule").find("[dbCol]");

    let dataVM = new FormData();

    ScheduleForm.each((i, v) => {
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
        if (currObj.hasClass('timeonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let currentDateFormat = "HH:mm";
                let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "HH:mm";
                value = moment(value, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                value = "";
            }
        }

        //if (name == "MemberDetail.DateOfDepartment") {
        //    value += " " + $("[name='TimeOfDepartment']").val();
        //}

        dataVM.append(currObj.attr("name"), value);
    });
    dataVM.append("MemberDetail.PAXSno", $("[name='MemberDetail.PAXSno']").val());
    BlockUI();
    $.ajax({
        url: '/ScheduleMaster/SaveMasterSchedule',
        data: dataVM,
        //dataType: "json",
        async: false,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (response) {
            $("#addMasterSchedulePopup").find(".btn-close").click();
            if (response != null) {
                Swal.fire({ title: '', text: "Added Successfully!", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                    if (result.isConfirmed) {
                        MasterScheduleListPartialView();
                    }
                });
            }

            UnblockUI();
        },
        error: function (xhr, ajaxOptions, error) {
            UnblockUI();
            $erroralert("Transaction Failed!", xhr.responseText + '!');

            //responseText
        }
    });

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




function DeleteList(Id) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "Id": Id
            };
            $.ajax({
                type: "POST",
                url: "/ScheduleMaster/DeleteScheduleMaster",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Deleted Successful!");
                    MasterScheduleListPartialView();
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