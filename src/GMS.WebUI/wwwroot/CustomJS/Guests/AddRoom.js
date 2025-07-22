$(document).ready(function () {



});

function initOnRoomAdd() {
    //$("#exampleModal").find("[name='ddlAvailableRoomsSharedStatus']").change(function () {
    //    let SharedStatus = $(this).val();
    //    BlockUI();
    //    let inputDTO = {};
    //    inputDTO.Shared = SharedStatus;
    //    inputDTO.GuestId = $("#exampleModal").find("[name='GuestId']").val();
    //    $.ajax({
    //        type: "POST",
    //        url: "/Guests/GetAvailableRoomsForGuestAllocation",
    //        contentType: 'application/json',
    //        data: JSON.stringify(inputDTO),
    //        success: function (data) {
    //            UnblockUI();
    //            let ctrl = $("#exampleModal").find("[name='ddlAvailableRoomsShared']");
    //            ctrl.empty();
    //            ctrl.append('<option value="0">Select Room</option>');
    //            if (data != null) {
    //                for (var i = 0; i < data.length; i++) {
    //                    ctrl.append('<option value="' + data[i].roomNo + '">Room No - ' + data[i].roomNo + (data[i].shared == true ? " Shared With(" + data[i].sharedWith + ")" : "") + '</option>');
    //                }
    //            }
    //        },
    //        error: function (error) {
    //            /* $erroralert("Transaction Failed!", error.responseText + '!');*/
    //            UnblockUI();
    //        }
    //    });

    //});
    $("#exampleModal").find("[name='ddlAvailableRoomsSharedStatus']").change(function () {
        let SharedStatus = $(this).val();
        BlockUI();
        let inputDTO = {};
        inputDTO.Shared = SharedStatus;
        inputDTO.GuestId = $("#exampleModal").find("[name='GuestId']").val();
        $.ajax({
            type: "POST",
            url: "/Guests/GetRoomsAvailableForGuestsSharedNew",
            contentType: 'application/json',
            data: JSON.stringify(inputDTO),
            success: function (data) {
                UnblockUI();
                let ctrl = $("#exampleModal").find("[name='ddlAvailableRoomsShared']");
                ctrl.empty();
                ctrl.append('<option value="0">Select Room</option>');
                if (data != null) {
                    for (var i = 0; i < data.length; i++) {
                        ctrl.append('<option value="' + data[i].roomNo + '">' + data[i].roomNo + ((data[i].sharedWith == null || data[i].sharedWith == "") ? "" : "(" + data[i].sharedWith + ")") + '</option>');
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
            UnblockUI();
            $("#exampleModal").find(".btn-close").click();
            $successalert("Room Changed Successfully");

            try {
                GuestsListPartialView();
            }
            catch {

            }


        },
        error: function (error) {
            /* $erroralert("Transaction Failed!", error.responseText + '!');*/
            UnblockUI();
        }
    });
}

function AllocateRoom_New() {
    var inputDTO = {
        Rnumber: $("#exampleModal").find("[name='ddlAvailableRoomsShared']").val(),
        GuestId: $("#exampleModal").find("[name='GuestId']").val()
    };

    if (inputDTO.Rnumber == 0) {
        $erroralert("Error!", 'Please Select Room');
        return;
    }

    let CurrentSharingStatus = $("#CurrentSharingStatus").val();


    let SharingStatus = $("[name='ddlAvailableRoomsSharedStatus']").val();
    let RoomSharingWith = $("#RoomSharingWith").val();
    //Sharing Status Yes to Yes
    if (SharingStatus == 1 && CurrentSharingStatus == 1) {
        Swal.fire({ title: 'You and ' + RoomSharingWith + ' are sharing a room.', text: "Apply this change for " + RoomSharingWith + "?", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, apply for both', cancelButtonText: 'No, only for me', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
            if (result.value) {
                ChangeRoomForAllGroup();
            }
            else {
                Swal.fire({ title: '', text: "Would you like to update your accommodation status?", icon: 'warning', showCancelButton: true, confirmButtonText: 'Sharing – I prefer to share', cancelButtonText: 'Non-Sharing – Assign a private room', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
                    if (result.value) {
                        ChangeRoomForCurrentGuestWithSharingStatus(SharingStatus);
                    }
                    else {
                        Swal.fire({ title: '', text: "Changing to a single private room may incur additional charges. Would you like to proceed?", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, proceed – Change to a private room', cancelButtonText: 'No, keep sharing – Maintain current shared status', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
                            if (result.value) {
                                ChangeRoomForCurrentGuestWithNonSharingStatus();
                            }
                            else {
                                ChangeRoomForCurrentGuestWithSharingStatus(SharingStatus);
                            }
                        });
                    }
                });
            }
        });
    }
    //Sharing Status Yes to No
    else if (SharingStatus == 2 && CurrentSharingStatus == 1) {
        Swal.fire({ title: '', text: "Changing to a single private room may incur additional charges. Would you like to proceed?", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, proceed – Change to a private room', cancelButtonText: 'No, keep sharing – Maintain current shared status', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
            if (result.value) {
                ChangeRoomForCurrentGuestWithNonSharingStatus();
            }
            else {
                ChangeRoomForCurrentGuestWithSharingStatus(SharingStatus);
            }
        });
    }
    //Sharing Status No to No
    else if (SharingStatus == 2 && CurrentSharingStatus == 0) {
        ChangeRoomForCurrentGuestWithNonSharingStatus();
    }
    //Sharing Status No to Yes
    else if (SharingStatus == 1 && CurrentSharingStatus == 0) {
        ChangeRoomForCurrentGuestWithSharingStatus(SharingStatus);
    }

    //BlockUI();
    //$.ajax({
    //    type: "POST",
    //    url: "/Guests/AllocateRoom",
    //    contentType: 'application/json',
    //    data: JSON.stringify(inputDTO),
    //    success: function (data) {
    //        UnblockUI();
    //        $("#exampleModal").find(".btn-close").click();
    //        $successalert("Room Changed Successfully");

    //        try {
    //            GuestsListPartialView();
    //        }
    //        catch {

    //        }


    //    },
    //    error: function (error) {
    //        /* $erroralert("Transaction Failed!", error.responseText + '!');*/
    //        UnblockUI();
    //    }
    //});
}


function ChangeRoomForAllGroup() {
    var inputDTO = {
        Rnumber: $("#exampleModal").find("[name='ddlAvailableRoomsShared']").val(),
        GuestId: $("#exampleModal").find("[name='GuestId']").val()
    };
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Guests/AllocateRoomToAllGroup",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $("#exampleModal").find(".btn-close").click();
            $successalert("Room Changed Successfully");

            try {
                GuestsListPartialView();
            }
            catch {

            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function ChangeRoomForCurrentGuestWithSharingStatus(Shared) {
    var inputDTO = {
        Rnumber: $("#exampleModal").find("[name='ddlAvailableRoomsShared']").val(),
        GuestId: $("#exampleModal").find("[name='GuestId']").val(),
        Shared: Shared
    };
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Guests/ChangeRoomForCurrentGuestWithSharingStatus",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $("#exampleModal").find(".btn-close").click();
            $successalert("Room Changed Successfully");

            try {
                GuestsListPartialView();
            }
            catch {

            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}

function ChangeRoomForCurrentGuestWithNonSharingStatus() {
    var inputDTO = {
        Rnumber: $("#exampleModal").find("[name='ddlAvailableRoomsShared']").val(),
        GuestId: $("#exampleModal").find("[name='GuestId']").val()
    };
    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Guests/ChangeRoomForCurrentGuestWithNonSharingStatus",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $("#exampleModal").find(".btn-close").click();
            $successalert("Room Changed Successfully");

            try {
                GuestsListPartialView();
            }
            catch {

            }
        },
        error: function (error) {
            $erroralert("Transaction Failed!", error.responseText + '!');
            UnblockUI();
        }
    });
}