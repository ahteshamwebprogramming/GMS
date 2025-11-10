
function updateDateRange(tabId) {
    $tab = $("#" + tabId);
    const fromDate = $tab.find("#fromDate").val(); //document.getElementById("fromDate").value;
    const toDate = $tab.find("#toDate").val();

    $tab.find("#inventoryFromDate").val(fromDate)
    $tab.find("#inventoryToDate").val(toDate);
    $tab.find("#ratesFromDate").val(fromDate);
    $tab.find("#ratesToDate").val(toDate);
    $tab.find("#restrictionsFromDate").val(fromDate);
    $tab.find("#restrictionsToDate").val(toDate);
    $tab.find("#incrementFromDate").val(fromDate);
    $tab.find("#incrementToDate").val(toDate);

    updateSelectedDays(tabId);

    const activeTab = document.querySelector('.nav-link.active').id;
    //console.log("Active tab on date change:", activeTab);
    loadTabData(activeTab.replace('-tab', ''));
}
function updateSelectedDays(tabId) {
    let $tab = $("#" + tabId);

    // Get all checked days in the selected tab
    let selectedDays = $tab.find('input[name="SelectedDays"]:checked')
        .map(function () { return $(this).val(); })
        .get()
        .join(',');

    // Set hidden field value in the selected tab
    $tab.find("#SelectedDaysList").val(selectedDays);

    // Handle "All" checkbox
    let allCheckbox = $tab.find('input[name="SelectedDays"][value="All"]');
    if (allCheckbox.is(':checked')) {
        $tab.find('input[name="SelectedDays"]').not('[value="All"]').prop('checked', true);
    }
}

function loadTabData(tabId) {
    $tab = $("#" + tabId);
    console.log("Loading data for tab:", tabId);
    const fromDate = $tab.find("#fromDate").val();
    const toDate = $tab.find("#toDate").val();
    const planId = $tab.find("[name='ChannelId']").val();
    const selectedDays = $tab.find("[name='SelectedDaysList']").val();

    let url, tbodyId;
    if (tabId === 'tab2') {
        url = '/Rooms/GetInventoryData';
        tbodyId = 'inventoryTableBody';
    } else if (tabId === 'tab1') {
        url = '/Rooms/GetRatesData';
        tbodyId = 'ratesTableBody';
    } else if (tabId === 'tab_PR') {
        url = '/Rooms/GetRatesData_New';
        tbodyId = 'packageRatesTableBody';
    } else if (tabId === 'tab3') {
        url = '/Rooms/GetIncrementData';
        tbodyId = 'incrementTableBody';
    } else if (tabId === 'tab4') {
        url = '/Rooms/GetRestrictionsData';
        tbodyId = 'restrictionsTableBody';
    } else {
        console.log("Skipping tab:", tabId);
        return;
    }

    console.log("AJAX URL:", url, "From:", fromDate, "To:", toDate);
    BlockUI();
    $.ajax({
        url: url,
        type: 'GET',
        data: { fromDate: fromDate, toDate: toDate, planId: planId },
        success: function (data) {
            UnblockUI();
            console.log("Data received for", tabId, ":", data);
            const tbody = document.getElementById(tbodyId);
            tbody.innerHTML = '';

            if (tabId === 'tab2') {
                data.forEach((item, index) => {
                    tbody.innerHTML += `
                                <tr>
                                    <td><input type="checkbox" name="Inventory[${index}].IsSelected" value="true" /></td>
                                    <td>${item.roomTypeName}</td>
                                    <td><input type="number" name="Inventory[${index}].RoomsAvailable" value="${item.roomsAvailable || ''}" readonly style="width: 60px; background-color: #e9ecef; border: none;" /></td>
                                    <td>
                                        <input type="number" name="Inventory[${index}].RoomsOpen" value="${item.roomsOpen || ''}" min="0" step="1" style="width: 60px;" oninput="validateRoomsOpen(this)" />
                                        <input type="hidden" name="Inventory[${index}].RoomTypeId" value="${item.roomTypeId}" />
                                    </td>
                                </tr>`;
                });
                setRoomsOpenMaxValues();
            } else if (tabId === 'tab1') {
                data.forEach((item, index) => {
                    tbody.innerHTML += `
                                <tr>
                                    <td><input type="checkbox" name="Rates[${index}].IsSelected" value="true" /></td>
                                    <td>${item.roomTypeName}</td>
                                    <td><input type="number" id="minRate-${index}" name="Rates[${index}].MinimumRate" value="${item.minimumRate || ''}" min="0" step="0.01" style="width: 80px;" /></td>
                                    <td><input type="number" id="maxRate-${index}" name="Rates[${index}].MaximumRate" value="${item.maximumRate || ''}" min="0" step="0.01" style="width: 80px;" /></td>
                                    <td>
                                        <input type="number" id="saleRate-${index}" name="Rates[${index}].SaleRate" value="${item.saleRate || ''}" min="0" step="0.01" style="width: 80px;" onblur="validateRate('saleRate-${index}', 'minRate-${index}', 'maxRate-${index}')" />
                                        <input type="hidden" name="Rates[${index}].RoomTypeId" value="${item.roomTypeId}" />
                                    </td>
                                    <td><input type="number" name="Rates[${index}].CancellationDays" value="${item.cancellationDays || ''}" min="0" step="1" style="width: 80px;" /></td>
                                    <td>
                                        <div class="toggle-container">
                                            <input type="hidden" name="Rates[${index}].StopSell" value="false" />
                                            <input type="checkbox" id="rate-stopSell-${index}" name="Rates[${index}].StopSell" value="true" ${item.stopSell === true ? 'checked' : ''} class="toggle-checkbox">
                                            <label for="rate-stopSell-${index}" class="toggle-label">
                                                <span class="toggle-circle"></span>
                                            </label>
                                        </div>
                                    </td>
                                    <td>
                                        <input type="hidden" name="Rates[${index}].CloseOnArrival" value="false" />
                                        <input type="checkbox" id="rate-closeOnArrival-${index}" name="Rates[${index}].CloseOnArrival" value="true" ${item.closeOnArrival ? 'checked' : ''}>
                                    </td>
                                    <td>
                                        <input type="hidden" name="Rates[${index}].RestrictStay" value="false" />
                                        <input type="checkbox" id="rate-restrictStay-${index}" name="Rates[${index}].RestrictStay" value="true" ${item.restrictStay ? 'checked' : ''}>
                                    </td>
                                    <td><input type="number" name="Rates[${index}].MinimumNights" value="${item.minimumNights || ''}" min="0" step="1" style="width: 80px;" /></td>
                                    <td><input type="number" name="Rates[${index}].MaximumNights" value="${item.maximumNights || ''}" min="0" step="1" style="width: 80px;" /></td>
                                </tr>`;
                });
            }
            else if (tabId === 'tab_PR') {
                data.forEach((item, index) => {
                    tbody.innerHTML += `
                                <tr>
                                    <td><input type="checkbox" name="Rates[${index}].IsSelected" value="true" /></td>
                                    <td>${item.roomTypeName}</td>
                                    <td><input type="number" id="minRate-${index}" name="Rates[${index}].MinimumRate" value="${item.minimumRate || ''}" min="0" step="0.01" style="width: 80px;" /></td>
                                    <td><input type="number" id="maxRate-${index}" name="Rates[${index}].MaximumRate" value="${item.maximumRate || ''}" min="0" step="0.01" style="width: 80px;" /></td>
                                    <td>
                                        <input type="number" id="saleRate-${index}" name="Rates[${index}].SaleRate" value="${item.saleRate || ''}" min="0" step="0.01" style="width: 80px;" onblur="validateRate('saleRate-${index}', 'minRate-${index}', 'maxRate-${index}')" />
                                        <input type="hidden" name="Rates[${index}].RoomTypeId" value="${item.roomTypeId}" />
                                    </td>
                                </tr>`;
                });
            }
            else if (tabId === 'tab3') {
                data.forEach((item, index) => {
                    const percentages = item.percentages || [-0.10, 0, 0.25, 0.50, 0.35, 0.20, 0.10, 0.75];
                    const percentageValues = percentages.map(pct => (pct * 100).toFixed(2));
                    tbody.innerHTML += `
                            <tr>
                                <td><input type="checkbox" name="Percentages[${index}].IsSelected" value="true" /></td>
                                <td>${item.roomTypeName}</td>
                                <td>${item.numberOfRooms}</td>
                                <td><input type="number" name="Percentages[${index}].Percentages[0]" value="${percentageValues[0]}" step="0.01" class="form-control" style="width: 100px;" /></td>
                                <td><input type="number" name="Percentages[${index}].Percentages[1]" value="${percentageValues[1]}" step="0.01" class="form-control" style="width: 100px;" /></td>
                                <td><input type="number" name="Percentages[${index}].Percentages[2]" value="${percentageValues[2]}" step="0.01" class="form-control" style="width: 100px;" /></td>
                                <td><input type="number" name="Percentages[${index}].Percentages[3]" value="${percentageValues[3]}" step="0.01" class="form-control" style="width: 100px;" /></td>
                                <td><input type="number" name="Percentages[${index}].Percentages[4]" value="${percentageValues[4]}" step="0.01" class="form-control" style="width: 100px;" /></td>
                                <td><input type="number" name="Percentages[${index}].Percentages[5]" value="${percentageValues[5]}" step="0.01" class="form-control" style="width: 100px;" /></td>
                                <td><input type="number" name="Percentages[${index}].Percentages[6]" value="${percentageValues[6]}" step="0.01" class="form-control" style="width: 100px;" /></td>
                                <td><input type="number" name="Percentages[${index}].Percentages[7]" value="${percentageValues[7]}" step="0.01" class="form-control" style="width: 100px;" /></td>
                                <input type="hidden" name="Percentages[${index}].RoomTypeId" value="${item.roomTypeId}" />
                            </tr>`;
                });
            }

            else if (tabId === 'tab4') {
                data.forEach((item, index) => {
                    tbody.innerHTML += `
                                <tr>
                                    <td><input type="checkbox" name="Restrictions[${index}].IsSelected" value="true" /></td>
                                    <td>${item.roomTypeName}</td>
                                    <td>
                                        <div class="toggle-container">
                                            <input type="checkbox" id="toggle-${index}" name="Restrictions[${index}].StopSell" value="true" ${item.stopSell === true ? 'checked' : ''} class="toggle-checkbox">
                                            <label for="toggle-${index}" class="toggle-label">
                                                <span class="toggle-circle"></span>
                                            </label>
                                        </div>
                                    </td>
                                    <td>

        <input type="checkbox"
                   name="Restrictions[${index}].CloseOnArrival"

                   ${item.closeOnArrival ? 'checked' : ''}>
                                    </td>
                                    <td>


                                           <input type="hidden" name="Restrictions[${index}].RestrictStay" value="false">
        <input type="checkbox"
               name="Restrictions[${index}].RestrictStay"

               value="true"
               ${item.restrictStay ? 'checked' : ''}>
                                    </td>
                                    <td><input type="number" name="Restrictions[${index}].MinimumNights" value="${item.minimumNights || ''}" min="0" step="1" style="width: 80px;" /></td>
                                    <td>
                                        <input type="number" name="Restrictions[${index}].MaximumNights" value="${item.maximumNights || ''}" min="0" step="1" style="width: 80px;" />
                                        <input type="hidden" name="Restrictions[${index}].RoomTypeId" value="${item.roomTypeId}" />
                                    </td>
                                </tr>`;
                });
            }
        },
        error: function (xhr, status, error) {
            UnblockUI();
            console.error('Error fetching data for', tabId, ':', error, xhr.status, xhr.responseText);
            Swal.fire({ icon: 'error', title: 'Error', text: 'Failed to load data for ' + tabId });
        }
    });
}

$('#myTab li').on('shown.bs.tab', function (e) {
    const tabId = e.target.id.replace('-tab', '');
    console.log("Tab switched to:", tabId);
    updateDateRange(tabId);
    loadTabData(tabId);
    if (tabId == "tab_PR") {
        $("[name='CopyFromDiv']").show();
        $("[name='CopyFromButton']").show();
    }
    else {
        $("[name='CopyFromDiv']").hide();
        $("[name='CopyFromButton']").hide();
    }
});

document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM loaded, loading initial tab1 data");
    updateDateRange('tab1');
    loadTabData('tab1');
});
$(document).on('submit', '#incrementForm', function (e) {
    e.preventDefault();

    Swal.fire({
        title: 'Please Wait',
        text: 'Saving percentages...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => { Swal.showLoading(); }
    });

    $.ajax({
        url: this.action,
        type: this.method,
        data: $(this).serialize(), // Send as form-encoded data
        success: function (result) {
            Swal.close();
            Swal.fire({
                icon: result.success ? 'success' : 'error',
                title: result.success ? 'Success' : 'Error',
                text: result.message
            }).then(() => {
                if (result.success) {
                    loadTabData('tab3');
                }
            });
        },
        error: function (xhr, status, error) {
            Swal.close();
            Swal.fire({ icon: 'error', title: 'Error', text: 'Failed to save percentages: ' + error });
        }
    });
});
$(document).on('submit', '#inventoryForm, #ratesForm, #restrictionsForm', function (e) {
    e.preventDefault();
    const formId = this.id;
    const selectedRows = $(this).find('[name$=".IsSelected"]:checked').length;

    if (selectedRows === 0) {
        Swal.fire({ icon: 'warning', title: 'No Selection', text: 'Please select at least one row to update.' });
        return;
    }

    Swal.fire({
        title: 'Please Wait',
        text: 'Saving updates...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => { Swal.showLoading(); }
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
                    loadTabData(formId.replace('Form', ''));
                }
            });
        },
        error: function (xhr, status, error) {
            Swal.close();
            Swal.fire({ icon: 'error', title: 'Error', text: 'Failed to save updates: ' + error });
        }
    });
});

function setRoomsOpenMaxValues() {
    document.querySelectorAll('input[name$=".RoomsOpen"]').forEach(input => {
        const row = input.closest('tr');
        const roomsCountInput = row.querySelector('input[name$=".RoomsAvailable"]');
        const roomsCount = parseInt(roomsCountInput.value) || 0;
        input.setAttribute('max', roomsCount);
    });
}

function validateRoomsOpen(input) {
    const roomsOpen = parseInt(input.value) || 0;
    const max = parseInt(input.getAttribute('max')) || 0;
    if (roomsOpen < 0) {
        input.value = 0;
        Swal.fire({ icon: 'warning', title: 'Invalid Input', text: 'Rooms Open cannot be less than 0.' });
    } else if (roomsOpen > max) {
        input.value = max;
        Swal.fire({ icon: 'warning', title: 'Limit Exceeded', text: `Rooms Open cannot exceed Rooms Count (${max}).` });
    }
}

function validateRate(saleRateId, minRateId, maxRateId) {
    const saleRate = parseFloat(document.getElementById(saleRateId).value) || 0;
    const minRate = parseFloat(document.getElementById(minRateId).value) || 0;
    const maxRate = parseFloat(document.getElementById(maxRateId).value) || 0;
    if (saleRate < minRate) document.getElementById(saleRateId).value = minRate;
    if (maxRate > 0 && saleRate > maxRate) document.getElementById(saleRateId).value = maxRate;
}

function toggleSelectAll(formId, isChecked) {
    document.querySelectorAll(`#${formId} [name$='.IsSelected']`).forEach(cb => cb.checked = isChecked);
}