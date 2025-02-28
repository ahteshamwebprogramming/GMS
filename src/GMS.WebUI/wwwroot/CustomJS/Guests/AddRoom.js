$(document).ready(function () {



});

function initOnRoomAdd() {
    $("#exampleModal").find("[name='ddlAvailableRoomsSharedStatus']").change(function () {
        let SharedStatus = $(this).val();
        BlockUI();
        let inputDTO = {};
        inputDTO.Shared = SharedStatus;
        inputDTO.GuestId = $("#exampleModal").find("[name='GuestId']").val();
        $.ajax({
            type: "POST",
            url: "/Guests/GetAvailableRoomsForGuestAllocation",
            contentType: 'application/json',
            data: JSON.stringify(inputDTO),
            success: function (data) {
                UnblockUI();
                let ctrl = $("#exampleModal").find("[name='ddlAvailableRoomsShared']");
                ctrl.empty();
                ctrl.append('<option value="0">Select Room</option>');
                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        ctrl.append('<option value="' + data[i].roomNo + '">Room No - ' + data[i].roomNo + (data[i].shared == true ? " Shared With(" + data[i].sharedWith + ")" : "") + '</option>');
                    }
                }
            },
            error: function (error) {
                /* $erroralert("Transaction Failed!", error.responseText + '!');*/
                UnblockUI();
            }
        });

    });
}

function AddRoomPartialView(Id) {
    var inputDTO = {};
    inputDTO.Id = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Guests/AddRoomPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_AddRoomPartial').html(data);
            $("#btnAddRoomModal").click();
            initOnRoomAdd();
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });


}

function AllocateRoom() {
    var inputDTO = {
        Rnumber: $("#exampleModal").find("[name='ddlAvailableRoomsShared']").val(),
        GuestId: $("#exampleModal").find("[name='GuestId']").val(),
        Shared: $("#exampleModal").find("[name='ddlAvailableRoomsSharedStatus']").val()
    };
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Guests/AllocateRoom",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            $("#exampleModal").find(".btn-close").click();
            $successalert("Room Changed Successfully");
            UnblockUI();
        },
        error: function (error) {
            /* $erroralert("Transaction Failed!", error.responseText + '!');*/
            UnblockUI();
        }
    });
}