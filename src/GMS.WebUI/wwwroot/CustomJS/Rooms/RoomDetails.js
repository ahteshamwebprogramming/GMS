function GetRoomList(RoomId) {
    let inputDTO = {}
    inputDTO.Id = RoomId;
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
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function OpenCheckList(Id) {
    let inputDTO = {}
    inputDTO.Id = Id;
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
            GuestsListPartialView();
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