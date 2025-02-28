function AddNewRoomPartialView(Id = 0) {
    let inputDTO = {}
    inputDTO.Id = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/AddNewRoomPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_AddNewRoomPartial').html(data);
            $("#btnModalAddNewRoom").click();
            $('.numeric').forceNumeric();

            $('#amenitiesDropdown').select2({
                dropdownParent: $('#ModalAddNewRoom'),
                placeholder: "Select Amenities",
                allowClear: true,
                width: "100%",
                minimumResultsForSearch: 0
            });
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function AddNewRoomPartialView1(Id = 0) {
    let inputDTO = {}
    inputDTO.Id = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/AddNewRoomPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_AddNewRoomPartial').html(data);
            $("#btnModalAddNewRoom").click();
            $('.numeric').forceNumeric();

            $('#amenitiesDropdown').select2({
                dropdownParent: $('#ModalAddNewRoom'),
                placeholder: "Select Amenities",
                allowClear: true,
                width: "100%",
                minimumResultsForSearch: 0
            });
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function AddRoom() {

    if (!isValidateForm("FormAddNewRoom")) {
        return;
    }
    //alert("Success");
    //return;

    let formElements = $("#FormAddNewRoom").find("[dbCol]");

    let dataVM = new FormData();

    var roompictures = jQuery("#roompictures")[0].files;
    for (var i = 0; i < roompictures.length; i++) {
        dataVM.append("Attachments", roompictures[i]);
    }

    formElements.each((i, v) => {
        let currObj = $(v);
        let value = currObj.val();
        let id = currObj.attr("id");
        let name = currObj.attr("name");
        if (value != null && value != undefined && value !== "") {
            //value = value.toUpperCase();
        }

        if (currObj.hasClass('contactno') && value != "") {
            value = value.replace(/\s/g, "");
        }
        if (currObj.hasClass('dateonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let currentDateFormat = "DD-MM-YYYY";
                let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD";
                //value = moment(moment(value).format("DD-MM-YYYY")).format(dateformat);
                value = moment(value, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                value = "";
            }
        }
        if (currObj.hasClass('datetimeonly') && value != "") {
            try {
                let dateformat = currObj.attr("dateformat");
                let currentDateFormat = "DD-MM-YYYY HH:mm";
                let sourceDateFormat = (dateformat != undefined || dateformat != null || dateformat != "") ? dateformat : "YYYY-MM-DD HH:mm";
                value = moment(value, currentDateFormat).format(sourceDateFormat);
            }
            catch {
                value = "";
            }
        }
        if (Array.isArray(value)) {
            value.forEach(x => {
                dataVM.append(currObj.attr("name"), x);
            });
        }
        else {
            dataVM.append(currObj.attr("name"), value);
        }

    });

    //dataVM.append("AmenityIds", 1);
    //dataVM.append("AmenityIds", 2);
    //dataVM.append("AmenityIds", 3);

    BlockUI();
    $.ajax({
        url: '/Rooms/SaveRoom',
        data: dataVM,
        //dataType: "json",
        async: false,
        type: 'POST',
        processData: false,
        contentType: false,
        success: function (response) {
            $("#ModalAddNewRoom").find(".btn-close").click();
            $successalert("Success!", "Room Added Sucessfully");
            ListPartialView();
            UnblockUI();
        },
        error: function (xhr, ajaxOptions, error) {
            UnblockUI();
            alert(xhr.responseText);
            //responseText
        }
    });

}

function EditRoom(RoomId) {
    AddNewRoomPartialView(RoomId);
}


function ViewRoomAmenetiesPartialView(RoomNumber) {
    let inputDTO = {}
    inputDTO.Rnumber = RoomNumber;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/ViewRoomAmenetiesPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewRoomAmenetiesPartial').html(data);
            $("#btnModalViewRoomAmeneties").click();
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}

function ViewRoomImagesPartialView(Id) {
    let inputDTO = {}
    inputDTO.Id = Id;
    BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Rooms/ViewRoomImagesPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            UnblockUI();
            $('#div_ViewRoomImagesPartial').html(data);
            $("#btnModalViewRoomImages").click();
        },
        error: function (result) {
            UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}


function DeleteRoom(Id) {
    Swal.fire({ title: 'Are you sure?', text: "This will get deleted permanently!", icon: 'warning', showCancelButton: true, confirmButtonText: 'Yes, delete it!', customClass: { confirmButton: 'btn btn-primary me-3', cancelButton: 'btn btn-label-secondary' }, buttonsStyling: false }).then(function (result) {
        if (result.value) {
            BlockUI();
            var inputDTO = {
                "Id": Id
            };
            $.ajax({
                type: "POST",
                url: "/Rooms/DeleteRoom",
                contentType: 'application/json',
                data: JSON.stringify(inputDTO),
                success: function (data) {
                    $successalert("", "Deleted Successful!");
                    ListPartialView();
                    UnblockUI();
                },
                error: function (error) {
                    $erroralert("Transaction Failed!", error.responseText + '!');
                    UnblockUI();
                }
            });
        }
    });
}