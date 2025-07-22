

document.querySelectorAll('.toggle input[type="checkbox"]').forEach(toggle => {
    toggle.addEventListener('change', function () {
        const dateIndex = this.id.replace('toggle-date-', '');
        document.querySelectorAll(`input[data-date-index="${dateIndex}"]`).forEach(input => {
            input.disabled = !this.checked;
        });
    });
});

function validateRate(priceId, minRateId, maxRateId) {
    const priceInput = document.getElementById(priceId);
    const minRateInput = document.getElementById(minRateId);
    const maxRateInput = document.getElementById(maxRateId);

    // Remove commas before parsing
    const price = parseFloat(priceInput.value.replace(/,/g, '')) || 0;
    const minRate = parseFloat(minRateInput.value.replace(/,/g, '')) || 0;
    const maxRate = parseFloat(maxRateInput.value.replace(/,/g, '')) || 999999.99;

    if (price < minRate) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Price',
            text: `Price (${price}) cannot be less than Min Rate (${minRate}).`,
            confirmButtonText: 'OK'
        }).then(() => {
            priceInput.value = minRate;
            priceInput.focus();
        });
    } else if (price > maxRate) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Price',
            text: `Price (${price}) cannot be greater than Max Rate (${maxRate}).`,
            confirmButtonText: 'OK'
        }).then(() => {
            priceInput.value = maxRate;
            priceInput.focus();
        });
    }
}


$(document).on('submit', '#ratesForm', function (e) {
    e.preventDefault();


    Swal.fire({
        title: 'Please Wait',
        text: 'Saving data...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: this.action,
        type: this.method,
        data: $(this).serialize(),
        success: function (result) {
            Swal.close();
            Swal.fire({
                icon: result.success ? 'success' : 'error',
                title: result.success ? 'Success' : 'Error',
                text: result.message
            }).then(() => {
                if (result.success) {
                    updateDate(); // Refresh table on success
                }
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

// Date navigation
function navigateDate(direction) {
    var calendar = document.getElementById("calendar");
    var date = new Date(calendar.value);
    var today = new Date('@DateTime.Today.ToString("yyyy-MM-dd")');

    if (direction === "prev") {
        date.setDate(date.getDate() - 10);
        if (date < today) {
            date = today;
        }
    } else {
        date.setDate(date.getDate() + 10);
    }
    calendar.value = date.toISOString().split("T")[0];
    updateDate();
}

// Update table on date change
function updateDate() {
    var date = document.getElementById("calendar").value;
    var today = '@DateTime.Today.ToString("yyyy-MM-dd")';
    if (new Date(date) < new Date(today)) {
        date = today;
        document.getElementById("calendar").value = date;
    }
    BlockUI();
    $.ajax({
        url: '/Rooms/GetRatesTable',
        type: 'GET',
        data: { startDate: date },
        cache: false, // Prevent caching
        success: function (result) {
            UnblockUI();
            $('#ratesTableContainer').html(result);            
        },
        error: function (xhr, status, error) {
            UnblockUI();
            console.error('Error updating table:', error);
        }
    });
    
}
