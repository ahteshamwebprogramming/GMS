let debounceTimer;
$(document).on("input paste", "[name='MemberDetail.MobileNo']", function () {
    let phoneNumber = $(this).val().replace(/\D/g, ''); // Keep only numbers

    if (phoneNumber.length >= 3) {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => {
            SearchGuestDetailsByPhoneNumber(phoneNumber);
        }, 300);
    }
});

//$(document).ready(function () {
//    let debounceTimer;
//    $("[name='MemberDetail.MobileNo']").on("input paste change blur", function () {
//        let phoneNumber = $(this).val().replace(/\D/g, ''); // Keep only numbers

//        if (phoneNumber.length >= 3) {
//            clearTimeout(debounceTimer);
//            debounceTimer = setTimeout(() => {
//                SearchGuestDetailsByPhoneNumber(phoneNumber);
//            }, 300);
//        }
//    });
//});
function SearchGuestDetailsByPhoneNumber(phoneNumber) {
    var inputDTO = {};
    inputDTO.PhNo = phoneNumber;
    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Guests/SearchGuestDetailsByPhoneNumber',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            //      UnblockUI();
            $('#div_GuestSearchSection_PartialView').html(data);
        },
        error: function (result) {
            //    UnblockUI();
            /*  $erroralert("Transaction Failed!", result.responseText);*/
        }
    });
}

//function SelectGuestForForm(Id) {
//    var inputDTO = {};
//    inputDTO.Id = Id;
//    inputDTO.GroupId = $("#AddGuestForm").find("[name='MemberDetail.GroupId']").val();
//    BlockUI();
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        url: '/Guests/GuestDetailsByIdForGuestFormDetails',
//        data: JSON.stringify(inputDTO),
//        cache: false,
//        dataType: "html",
//        success: function (data, textStatus, jqXHR) {
//            UnblockUI();
//            $('#div_GuestsFormDetailsPartialView').html(data);
//            initDates();
//            getOnLoadAddGuests();
//            initValidateRoomsAvailability();
//            $("[name='MemberDetail.Id']").val(0);
//            //$("[name='MemberDetail.GroupId']").val(0);

//            $("select").chosen({
//                width: '100%'
//            });
//        },
//        error: function (result) {
//            UnblockUI();
//            /*  $erroralert("Transaction Failed!", result.responseText);*/
//        }
//    });
//}

function SelectGuestForForm(Id) {
    var inputDTO = {};
    inputDTO.Id = Id;
    inputDTO.GroupId = $("#AddGuestForm").find("[name='MemberDetail.GroupId']").val();
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Reservation/GuestDetailsByIdForGuestFormDetails',
        data: JSON.stringify(inputDTO),
        //cache: false,
        //dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            const formId = "AddGuestForm";
            $("#div_PersonalInformation_PartialView").html(data.personalInformation);
            $("#div_ContactInformation_PartialView").html(data.contactInformation);
            $("#div_Documents_PartialView").html(data.documents);
            //$("#div_HiddenValues_PartialView").html(data.hiddenValues);



            initDatesByFormId(formId);
            initValidateRoomsAvailabilityByFormId(formId)
            getOnLoadAddGuests(formId);

            //$("[name='MemberDetail.Id']").val(0);

            $("#div_HiddenValues_PartialView").find("[name='MemberDetail.UHID']").val(data.uhid);

            $("select").chosen({
                width: '100%'
            });
        },
        error: function (result) {
            UnblockUI();
            /*  $erroralert("Transaction Failed!", result.responseText);*/
        }
    });
}