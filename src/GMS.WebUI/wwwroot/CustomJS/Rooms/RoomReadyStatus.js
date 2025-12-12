let selectedRooms = [];
let activeChecklistRooms = [];

function GetRoomList(RoomId = 0) {
    let inputDTO = {}
    inputDTO.Id = RoomId;
    inputDTO.EnableSelection = true;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/RoomStatusRecordPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ListPartial').html(data);
            initRoomSelectionGrid();
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function OpenCheckList(Id, roomNumber = "", roomsCollection = null) {
    let inputDTO = {}
    let collection = [];
    if (Array.isArray(roomsCollection) && roomsCollection.length > 0) {
        collection = roomsCollection;
    }
    else {
        const displayNumber = roomNumber && roomNumber !== "" ? roomNumber : Id;
        collection = [{ id: Id, number: displayNumber }];
    }
    activeChecklistRooms = collection;
    inputDTO.Id = collection[0]?.id ?? Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/RoomCleanCheckList',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_RoomCleanCheckListPartial').html(data);
            $("#btnCheckInListModal").click();
            $("#GrvdCheckIn thead input[type='checkbox']").on("change", function () {
                // Get the checked state of the header checkbox
                var isChecked = $(this).is(":checked");
                // Set the checked state for all checkboxes in tbody
                $("#GrvdCheckIn tbody input[type='checkbox']").prop("checked", isChecked);
            });
            updateSelectedRoomsLabel();
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}
function SubmitRoomCheckList() {
    let inputDTO = {};
    if ($('#GrvdCheckIn tbody tr td input').is(':checked')) {
        var ids = [];
        $('#GrvdCheckIn tbody tr td input[type="checkbox"]:checked').each(function () {
            ids.push(this.id.split('_')[1]);
        });
        inputDTO["RChkLstID"] = ids.toString(',');
        inputDTO["RID"] = $("#CheckInListModal").find("[name='RID']").val();
        inputDTO["Reason"] = $("#CheckInListModal").find("[name='Reason']").val();
        inputDTO["Comments"] = $("#CheckInListModal").find("[name='Comments']").val();
    }
    else {
        $erroralert("Action Missing!", 'Please review the action items!');
        return;
    }
    const roomsToClean = activeChecklistRooms.length
        ? activeChecklistRooms.map(room => room.id)
        : [];
    if (!roomsToClean.length) {
        const fallbackRoomId = parseInt($("#CheckInListModal").find("[name='RID']").val());
        if (fallbackRoomId) {
            roomsToClean.push(fallbackRoomId);
        }
    }
    if (!roomsToClean.length) {
        $erroralert("Action Missing!", 'Please select room(s) to clean!');
        return;
    }
    inputDTO["RoomIds"] = roomsToClean;
    inputDTO["RID"] = roomsToClean[0];

    BlockUI();
    $.ajax({
        type: "POST",
        url: "/Rooms/CleanRoomCheck",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            $successalert("", "Room Cleaned Successfully!");
            $("#CheckInListModal").find(".btn-close").click();
            const cleanedRoomIds = roomsToClean;
            selectedRooms = selectedRooms.filter(room => !cleanedRoomIds.includes(room.id));
            activeChecklistRooms = [];
            updateSelectedRoomsLabel();
            updateBulkCleanControls();
            GetRoomList();
        },
        error: function (error) {
            UnblockUI();
            if (error.status == 430) {
                $erroralert("Action Missing!", 'Please review the action items!');
            }
            else {
                $erroralert("Error!", error.responseText + '!');
            }
        }
    });
}

function AddPartialView() {

    //let dates = {}
    //dates.StartDate = moment($("#flatpickr-startdate").val(), "DD-MMM-YYYY").format("YYYY-MM-DD"); //'2025-01-01';//$("#flatpickr-startdate").val();
    //dates.EndDate = moment($("#flatpickr-enddate").val(), "DD-MMM-YYYY").format("YYYY-MM-DD"); // '2025-01-11';//$("#flatpickr-enddate").val();

    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/RoomLockingAddPartialView',
        //data: JSON.stringify(dates),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            //UnblockUI();
            $('#div_AddPartial').html(data);
            initDates();
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function ListPartialView() {
    //let dates = {}
    //dates.StartDate = moment($("#flatpickr-startdate").val(), "DD-MMM-YYYY").format("YYYY-MM-DD"); //'2025-01-01';//$("#flatpickr-startdate").val();
    //dates.EndDate = moment($("#flatpickr-enddate").val(), "DD-MMM-YYYY").format("YYYY-MM-DD"); // '2025-01-11';//$("#flatpickr-enddate").val();

    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/RoomDetailsListPartialView',
        //data: JSON.stringify(dates),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            //UnblockUI();
            $('#div_ListPartial').html(data);
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function initDates() {
    flatpickr("#flatpickr-enddate", {
        enableTime: false,
        noCalendar: false,
        dateFormat: "d-M-y",
        onChange: function (selectedDates, dateStr) {
            // $("#selectedDuration").val(dateStr);
            // CalculateEndTime();
        }
    });
    flatpickr("#flatpickr-startdate", {
        enableTime: false,
        noCalendar: false,
        dateFormat: "d-M-y",
        onChange: function (selectedDates, dateStr) {
            // $("#selectedDuration").val(dateStr);
            // CalculateEndTime();
        }
    });
}

function Submitdata() {
    //if (!isValidateForm("AddGuestForm")) {
    //    return;
    //}

    let formInputs = $("#RoomLockForm").find("[dbCol]");

    let inputDTO = {};

    formInputs.each((i, v) => {
        let currObj = $(v);
        let value = currObj.val();
        let id = currObj.attr("id");
        let name = currObj.attr("name");
        if (value != null && value != undefined && value !== "") {
            //value = value.toUpperCase();
        }
        if (currObj.hasClass('dateonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let dtoDateFormat = "YYYY-MM-DD";
                //let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD";
                //value = moment(moment(value).format("DD-MM-YYYY")).format(dateformat);
                value = moment(value, dateformat).format(dtoDateFormat);
            }
            catch {
                value = "";
            }
        }
        inputDTO[name] = value;
    });
    /*dataVM.append("MemberDetail.PAXSno", $("[name='MemberDetail.PAXSno']").val());*/
    BlockUI();
    $.ajax({
        url: '/Rooms/SaveRoomLockingDetails',
        type: "POST",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (response) {

            $("#closeModalLock").click();
            $successalert("Transaction Successfull");
            ListPartialView()
            UnblockUI();
        },
        error: function (xhr, ajaxOptions, error) {
            UnblockUI();
            alert(xhr.responseText);
        }
    });

}

function LockRoom(Id, Rnumber) {
    let inputDTO = {};
    inputDTO["RoomLockHold"] = "Lock";
    inputDTO["Id"] = Id;
    inputDTO["RoomNo"] = Rnumber;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/RoomLockingAddPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_AddPartial').html(data);
            $("#btnModalLock").click();
            initDates();
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}
function HoldRoom(Id, Rnumber) {
    let inputDTO = {};
    inputDTO["RoomLockHold"] = "Hold";
    inputDTO["Id"] = Id;
    inputDTO["RoomNo"] = Rnumber;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/RoomLockingAddPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_AddPartial').html(data);
            $("#btnModalLock").click();
            initDates();
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function initRoomSelectionGrid() {
    const $tableWrapper = $('#div_ListPartial');
    const $checkboxes = $tableWrapper.find('.room-select');
    if (!$checkboxes.length) {
        selectedRooms = [];
        updateBulkCleanControls();
        return;
    }

    const currentIds = $checkboxes.map(function () {
        return parseInt($(this).data('room-id'));
    }).get().filter(id => !isNaN(id));
    selectedRooms = selectedRooms.filter(room => currentIds.includes(room.id));

    $checkboxes.each(function () {
        const roomId = parseInt($(this).data('room-id'));
        const isSelected = selectedRooms.some(room => room.id === roomId);
        $(this).prop('checked', isSelected);
    });

    $checkboxes.off('change').on('change', function () {
        const roomId = parseInt($(this).data('room-id'));
        const roomNumber = $(this).data('room-number');
        if (isNaN(roomId)) {
            return;
        }
        const displayNumber = roomNumber && roomNumber !== "" ? roomNumber : roomId;
        if ($(this).is(':checked')) {
            if (!selectedRooms.some(room => room.id === roomId)) {
                selectedRooms.push({ id: roomId, number: displayNumber });
            }
        } else {
            selectedRooms = selectedRooms.filter(room => room.id !== roomId);
        }
        syncSelectAllState();
        updateBulkCleanControls();
    });

    const $selectAll = $('#selectAllRooms');
    if ($selectAll.length) {
        $selectAll.prop('checked', false).prop('indeterminate', false);
        $selectAll.off('change').on('change', function () {
            const isChecked = $(this).is(':checked');
            $checkboxes.each(function () {
                $(this).prop('checked', isChecked).trigger('change');
            });
        });
    }
    syncSelectAllState();
    updateBulkCleanControls();
}

function syncSelectAllState() {
    const $selectAll = $('#selectAllRooms');
    const $checkboxes = $('#div_ListPartial').find('.room-select');
    if (!$selectAll.length) {
        return;
    }
    if (!$checkboxes.length) {
        $selectAll.prop('checked', false).prop('indeterminate', false);
        return;
    }
    const checkedCount = $checkboxes.filter(':checked').length;
    const totalCount = $checkboxes.length;
    $selectAll.prop('checked', checkedCount === totalCount && totalCount > 0);
    $selectAll.prop('indeterminate', checkedCount > 0 && checkedCount < totalCount);
}

function updateBulkCleanControls() {
    const hasSelection = selectedRooms.length > 0;
    const selectedText = hasSelection ? selectedRooms.map(room => room.number).join(', ') : 'None';
    const $button = $('#btnCleanSelectedRooms');
    const $summary = $('#selectedRoomsSummary');

    if ($button.length) {
        $button.prop('disabled', !hasSelection);
    }
    if ($summary.length) {
        $summary.text(selectedText);
    }
}

function updateSelectedRoomsLabel() {
    const $label = $("#CheckInListModal").find("#selectedRoomsLabel");
    if (!$label.length) {
        return;
    }
    const rooms = activeChecklistRooms.length ? activeChecklistRooms.map(room => room.number).join(', ') : "--";
    $label.text(rooms || "--");
}

$(document).on('click', '#btnCleanSelectedRooms', function () {
    if (!selectedRooms.length) {
        $erroralert("Action Missing!", "Please select room(s) to clean.");
        return;
    }
    const roomSnapshot = selectedRooms.map(room => ({ id: room.id, number: room.number }));
    OpenCheckList(roomSnapshot[0].id, roomSnapshot[0].number, roomSnapshot);
});

$(document).on('hidden.bs.modal', '#CheckInListModal', function () {
    activeChecklistRooms = [];
    updateSelectedRoomsLabel();
});

function UnlockRoom(Id, Rnumber) {
    let inputDTO = {};
    inputDTO["RoomLockHold"] = "Lock";
    inputDTO["Id"] = Id;
    inputDTO["RoomNo"] = Rnumber;
    BlockUI();
    $.ajax({
        url: '/Rooms/UnHold',
        type: "POST",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (response) {

            //$("#closeModalLock").click();
            $successalert("Transaction Successfull");
            ListPartialView()
            UnblockUI();
        },
        error: function (xhr, ajaxOptions, error) {
            UnblockUI();
            alert(xhr.responseText);
        }
    });
}
function UnholdRoom(Id, Rnumber) {
    let inputDTO = {};
    inputDTO["RoomLockHold"] = "Hold";
    inputDTO["Id"] = Id;
    inputDTO["RoomNo"] = Rnumber;
    BlockUI();
    $.ajax({
        url: '/Rooms/UnHold',
        type: "POST",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (response) {

            //$("#closeModalLock").click();
            $successalert("Transaction Successfull");
            ListPartialView()
            UnblockUI();
        },
        error: function (xhr, ajaxOptions, error) {
            UnblockUI();
            alert(xhr.responseText);
        }
    });
}