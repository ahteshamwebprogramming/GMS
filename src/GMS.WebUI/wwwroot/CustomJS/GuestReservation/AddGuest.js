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
        let GuestId = $("#AddGuestForm").find("[name='MemberDetail.Id']").val();
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

function AddGuests() {

    const formId = "AddGuestForm";
    let $form = $("#AddGuestForm");

    if (!_isValidateForm("AddGuestForm")) {
        return;
    }
    //alert("Success");
    //return;

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

        if ($form.find("[name='GuestReservationRouteValues.PageSource']").val() == "RoomAvailability") {
            dataVM.append("RoomAllocation.Rnumber", $("[name='GuestReservationRouteValues.RoomNumber']").val());
            dataVM.append("MemberDetail.RoomType", $form.find("[name='MemberDetail.RoomType']").val());
            dataVM.append("MemberDetail.NoOfRooms", $form.find("[name='MemberDetail.NoOfRooms']").val());

        }

        dataVM.append("Source", $form.find("[name='GuestReservationRouteValues.PageSource']").val());

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
                //$("#addGuestPopup").find(".btn-close").click();

                if ($form.find("[name='GuestReservationRouteValues.PageSource']").val() == "RoomAvailability") {
                    Swal.fire({ title: 'Transaction Successful!', text: "Guest is added", icon: 'success', confirmButtonText: 'OK' }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = "/Rooms/RoomsAvailabality";
                            return;
                        }
                    });
                }
                else {
                    if (response != null) {
                        if (response.paxSno < response.pax) {
                            //$("[name='MemberDetail.PAXSno']").val(parseInt(response?.pAXSno) + 1);
                            AddGuestsPartialView(response.groupId, parseInt(response?.paxSno) + 1);
                        }
                        else {
                            if (response.opt == "Update") {
                                window.location.href = "/Guests/GuestsList";
                            }
                            else {
                                window.location.href = "/Guests/GuestRegistrationSuccessfull/" + response.groupId;
                            }
                        }
                    }
                }


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

function _isValidateForm(formId) {

    $('.requiredInputCstmFile').on('click change paste keyup', function () {
        var element = $(this);
        element.removeClass('error');
        element.parent().find('.error-mandatory').remove();
    });


    let res = isValidateForm(formId);

    var photoFromInput = jQuery("#attachmentPhoto")[0].files;
    var photoFromDB = $("#attachmentPhoto").parent().parent().find("[name^=attachmentTag_]");
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
    var idProofFromDB = $("#attachmentIdProof").parent().parent().find("[name^=attachmentTag_]");
    if (nationality == "Indian" && !(idProofFromInput != null && idProofFromInput != undefined && idProofFromInput.length != 0) && (idProofFromDB.length == 0)) {
        let element = jQuery("#attachmentIdProof");
        element.removeClass('error');
        element.parent().find('.error-mandatory').remove();
        element.addClass('error');
        element.parent().append('<p class="error-mandatory">This field is mandatory</p>');
        res = false;
    }


    var passportFromInput = jQuery("#attachmentPassport")[0].files;
    var passportFromDB = $("#attachmentPassport").parent().parent().find("[name^=attachmentTag_]");
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