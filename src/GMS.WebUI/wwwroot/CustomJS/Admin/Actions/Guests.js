function SearchGuests() {
    let searchField = $("#SearchSection").find("[name='searchField']").val();

    var inputDTO = {};

    inputDTO.SearchField = searchField;

    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AdminActions/SearchGuests',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#div_SearchResultGuestsPartialView").html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}


function SearchGuestById(Id) {
    GetMembers(Id);
    GetRoomAllocation(Id);
    GetBilling(Id);
    GetPayment(Id);
    GetSettlement(Id);
}

function GetMembers(Id) {
    var inputDTO = {};
    inputDTO.GuestId = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AdminActions/GetGuestDetailsByID',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#div_SearchResultGuestsPartialView").html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}
function GetRoomAllocation(Id) {
    var inputDTO = {};
    inputDTO.GuestId = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AdminActions/SearchGuests',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#div_SearchResultGuestsPartialView").html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}
function GetBilling(Id) {
    var inputDTO = {};
    inputDTO.GuestId = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AdminActions/SearchGuests',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#div_SearchResultGuestsPartialView").html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}
function GetPayment(Id) {
    var inputDTO = {};
    inputDTO.GuestId = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AdminActions/SearchGuests',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#div_SearchResultGuestsPartialView").html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}
function GetSettlement(Id) {
    var inputDTO = {};
    inputDTO.GuestId = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/AdminActions/SearchGuests',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $("#div_SearchResultGuestsPartialView").html(data);
        },
        error: function (result) {
            UnblockUI();
        }
    });
}