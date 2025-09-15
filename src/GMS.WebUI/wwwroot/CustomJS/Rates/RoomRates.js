//$(document).on('submit', '#ratesForm', function (e) {
//    e.preventDefault();
//    let Rates = [];
//    for (let i = 0; i <= 9; i++) {

//        const toggle = $(`#toggle-date-${i}`);

//        if (toggle.is(':checked')) {


//            let uniqueRoomTypeIds = [...new Set(
//                $('tr[roomtypeid]').map(function () {
//                    return $(this).attr('roomtypeid');
//                }).get()
//            )];
//            uniqueRoomTypeIds.forEach(rti => {


//                let rate = {
//                    RoomTypeId: parseInt(rti, 10) || 0,
//                    Id: parseInt($(`.Id_${rti}_${i}`).val(), 10) || 0,
//                    Date: $(`.Date_${rti}_${i}`).val() || "",
//                    Price: parseFloat($(`.Price_${rti}_${i}`).val().replace(/[^\d.-]/g, '')) || 0,
//                    MinRate: parseFloat($(`.MinRate_${rti}_${i}`).val().replace(/[^\d.-]/g, '')) || 0,
//                    MaxRate: parseFloat($(`.MaxRate_${rti}_${i}`).val().replace(/[^\d.-]/g, '')) || 0,
//                    PlanId: parseInt($("#RatePlanId").val(), 10) || 0
//                };

//                Rates.push(rate);
//            });
//        }
//    }

//    let planId = $("#RatePlanId").val();

//    let model = {};
//    model.Rates = Rates;
//    //formData.append("PlanId",planId);

//    Swal.fire({
//        title: 'Please Wait',
//        text: 'Saving data...',
//        allowOutsideClick: false,
//        allowEscapeKey: false,
//        didOpen: () => {
//            Swal.showLoading();
//        }
//    });

//    $.ajax({
//        url: this.action,
//        type: this.method,
//        data: JSON.stringify(model),
//        //processData: false,
//        //contentType: false,
//        contentType: 'application/json',

//        success: function (result) {
//            Swal.close();
//            Swal.fire({
//                icon: result.success ? 'success' : 'error',
//                title: result.success ? 'Success' : 'Error',
//                text: result.message
//            }).then(() => {
//                if (result.success) {
//                    updateDate();
//                }
//            });
//        },
//        error: function (xhr, status, error) {
//            Swal.close();
//            Swal.fire({
//                icon: 'error',
//                title: 'Error',
//                text: 'Failed to save rates: ' + error
//            });
//        }
//    });
//});


$(document).on('submit', '#ratesForm', function (e) {
    e.preventDefault();

    let Rates = [];

    for (let i = 0; i <= 9; i++) {
        const toggle = $(`#toggle-date-${i}`);

        if (toggle.is(':checked')) {
            let uniqueRoomTypeIds = [...new Set(
                $('tr[roomtypeid]').map(function () {
                    return $(this).attr('roomtypeid');
                }).get()
            )];

            uniqueRoomTypeIds.forEach(rti => {
                let rate = {
                    RoomTypeId: parseInt(rti, 10) || 0,
                    Id: parseInt($(`.Id_${rti}_${i}`).val(), 10) || 0,
                    Date: $(`.Date_${rti}_${i}`).val() || "",
                    Price: parseFloat($(`.Price_${rti}_${i}`).val().replace(/[^\d.-]/g, '')) || 0,
                    MinRate: parseFloat($(`.MinRate_${rti}_${i}`).val().replace(/[^\d.-]/g, '')) || 0,
                    MaxRate: parseFloat($(`.MaxRate_${rti}_${i}`).val().replace(/[^\d.-]/g, '')) || 0,
                    PlanId: parseInt($("#RatePlanId").val(), 10) || 0
                };
                Rates.push(rate);
            });
        }
    }
    let planId = $("#RatePlanId").val();
    let model = { Rates: Rates, PlanId: planId };

    Swal.fire({
        title: 'Please Wait',
        text: 'Saving data...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    $.ajax({
        url: this.action,
        type: this.method,
        data: JSON.stringify(model),
        contentType: 'application/json',
        success: function (result) {
            Swal.close();
            Swal.fire({
                icon: result.success ? 'success' : 'error',
                title: result.success ? 'Success' : 'Error',
                text: result.message
            }).then(() => {
                if (result.success) updateDate();
            });
        },
        error: function (xhr, status, error) {
            Swal.close();
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to save rates: ' + error
            });
        }
    });
});
