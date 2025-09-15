function ChangeInOutDatePartialView(Id) {

    return new Promise((resolve, reject) => {

        let inputDTO = {}
        inputDTO.Id = Id;
        BlockUI();
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: '/Guests/ChangeInOutDatePartialView',
            data: JSON.stringify(inputDTO),
            cache: false,
            dataType: "html",
            success: function (data, textStatus, jqXHR) {
                UnblockUI();
                $('#div_ChangeInOutDatePartial').html(data);
                $("#btnChangeInOutDateModal").click();


                $("#InOutDateSection").find("[name='RoomAllocation.CheckInDate']").datetimepicker({
                    format: 'd-M-Y H:i',
                    timepicker: true,
                    defaultTime: '14:00'
                });

                $("#InOutDateSection").find("[name='RoomAllocation.CheckOutDate']").datetimepicker({
                    format: 'd-M-Y H:i',
                    timepicker: true,
                    defaultTime: '14:00'
                });


                resolve(Id);
            },
            error: function (result) {
                //UnblockUI();
                $erroralert("Transaction Failed!", result.responseText);
                reject(Id);
            }
        });
    });
}

function SaveChangeInOutDetails() {

    let inputDTO = {};

    inputDTO.CheckInDate = moment($("#InOutDateSection").find("[name='RoomAllocation.CheckInDate']").val(), "DD-MMM-YYYY HH:mm").format("YYYY-MM-DDTHH:mm:ss");
    inputDTO.CheckOutDate = moment($("#InOutDateSection").find("[name='RoomAllocation.CheckOutDate']").val(), "DD-MMM-YYYY HH:mm").format("YYYY-MM-DDTHH:mm:ss");
    inputDTO.GuestId = $("#ChangeInOutDateModal").find("[name='hdnCheckinoutGuestId']").val();


    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Guests/SaveCheckInOutDates",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $successalert("", "Saved Successfully!");
            $("#ChangeInOutDateModal").find(".btn-close").click();
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });

}