function RunAudit() {
    Swal.fire({ title: 'Are you sure?', text: "You want to run the audit!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, run it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {};
            $.ajax({
                type: "POST",
                url: "/RunAudit/RunNightAudit",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    UnblockUI();
                    $successalert("", "Audit Completed!");
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}