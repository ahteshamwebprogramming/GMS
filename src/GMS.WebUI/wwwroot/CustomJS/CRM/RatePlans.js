function GetRoomRatesForEnquiry() {
    var inputDTO = {};

    let _dateOfArrival = $("#AddGuestForm").find("[name='MemberDetail.DateOfArrival']");
    let _dateOfArrivalValue = $("#AddGuestForm").find("[name='MemberDetail.DateOfArrival']").val() || null;
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

    inputDTO.PlanId = $("#AddGuestForm").find("[name='MemberDetail.CatId']").val() || 0;
    inputDTO.NoOfNights = $("#AddGuestForm").find("[name='MemberDetail.ServiceId']").val() || 0;
    inputDTO.RateDate = _dateOfArrivalValue;
    inputDTO.RoomTypeId = $("#AddGuestForm").find("[name='MemberDetail.RoomType']").val() || 0;
    inputDTO.NoOfRooms = $("#AddGuestForm").find("[name='MemberDetail.NoOfRooms']").val() || 0;

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
            $('#div_RatePlanPartial').html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}