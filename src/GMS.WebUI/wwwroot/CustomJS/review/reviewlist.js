$(document).ready(function () {
    ReviewListPartialView();
});

function ReviewListPartialView(PageNumber = 1) {
    let ReviewListType = $("#ReviewListType").val();
    if (ReviewListType == "") {
        ReviewListType = "Current";
    }
    let SearchKeyword = $("#ReviewSearchKeyword").val();
    var inputDTO = {};
    inputDTO.GuestsListType = ReviewListType;
    inputDTO.PageSize = 10;
    inputDTO.PageNumber = PageNumber;
    inputDTO.SearchKeyword = SearchKeyword;
    BlockUI();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Review/ReviewListGridViewPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewReviewListPartial').html(data);
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function setActive(type) {
    $("#ReviewListType").val(type);
    ReviewListPartialView();
}

