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

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/CRM/RoomRatesForEnquiryCRM',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_RatePlanSection_PartialView').html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}