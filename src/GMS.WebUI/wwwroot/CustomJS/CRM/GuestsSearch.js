let debounceTimer;
$(document).on("input paste", "[name='MemberDetail.MobileNo']", function () {
    let phoneNumber = $(this).val().replace(/\D/g, ''); // Keep only numbers

    if (phoneNumber.length >= 3) {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => {
            SearchGuestDetailsByPhoneNumberCRM(phoneNumber);
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
function SearchGuestDetailsByPhoneNumberCRM(phoneNumber) {
    var inputDTO = {};
    inputDTO.PhNo = phoneNumber;
    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/CRM/SearchGuestDetailsByPhoneNumber',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            //      UnblockUI();
            $('#div_GuestsSearchPartial').html(data);
        },
        error: function (result) {
            //    UnblockUI();
            /*  $erroralert("Transaction Failed!", result.responseText);*/
        }
    });
}

function SelectGuestForFormCRM(Id) {
    var inputDTO = {};
    inputDTO.Id = Id;
    inputDTO.GroupId = $("#AddGuestForm").find("[name='MemberDetail.GroupId']").val();
    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/CRM/GuestDetailsByIdForGuestFormDetails',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            //      UnblockUI();
            $('#div_GuestsFormDetailsPartialView').html(data);
            initDatesCRM();
            getOnLoadAddGuestsCRM();
            initValidateRoomsAvailabilityCRM();
            $("[name='MemberDetail.Id']").val(0);
            //$("[name='MemberDetail.GroupId']").val(0);

            $("select").chosen({
                width: '100%'
            });
        },
        error: function (result) {
            //    UnblockUI();
            /*  $erroralert("Transaction Failed!", result.responseText);*/
        }
    });
}