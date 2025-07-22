function GuestsListPartialView(PageNumber = 1) {
    let GuestsListType = "Current";
    let SearchKeyword = "";
    var inputDTO = {};
    inputDTO.GuestsListType = GuestsListType;
    inputDTO.PageSize = 10000;
    inputDTO.PageNumber = PageNumber;
    inputDTO.SearchKeyword = SearchKeyword;
    //inputDTO.Source = $("[name='Source']").val();
    BlockUI();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Guests/GuestsTableViewPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewGuestsListPartial').html(data);
            $("table").DataTable();
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}