$(document).ready(function () {
    ListPartialView();
});

function ListPartialView() {
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/CreditDebitNoteAccount/ListPartialView',
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ListPartial').html(data);
            
            // Initialize DataTable
            if ($.fn.DataTable.isDataTable('#CreditDebitNoteAccountTable')) {
                $('#CreditDebitNoteAccountTable').DataTable().destroy();
            }
            
            $("table").DataTable({
                "aaSorting": [],
                "pageLength": 25,
                "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                layout: {
                    topStart: {
                        buttons: ['copy', 'csv', 'excel', 'pdf', 'print']
                    }
                }
            });
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

