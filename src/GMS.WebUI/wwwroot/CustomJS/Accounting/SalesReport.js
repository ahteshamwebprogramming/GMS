const monthNames_D = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];
$(document).ready(function () {



    const currentDate = new Date();
    const currentMonth_D = currentDate.getMonth() + 1;
    const currentYear_D = currentDate.getFullYear();
    let allBillingData = []; // Will store full dataset from server
    let allAuditRevenueData = []; // Will store full dataset from server

    // Populate month dropdown
    monthNames_D.forEach((month, index) => {
        const monthValue_D = index + 1;
        $('#month').append(
            `<option value="${monthValue_D}" ${monthValue_D === currentMonth_D ? 'selected' : ''}>${month}</option>`
        );
    });

    // Populate year dropdown (current year and previous 4 years)
    for (let i = 0; i < 5; i++) {
        const year_D = currentYear_D - i;
        $('#year').append(
            `<option value="${year_D}" ${year_D === currentYear_D ? 'selected' : ''}>${year_D}</option>`
        );
    }

    // On dropdown change, filter and update
    $('#month, #year').on('change', function () {
        const selectedMonth = parseInt($('#month').val());
        const selectedYear = parseInt($('#year').val());
        document.getElementById("currentMonthLabel").textContent = `${monthNames_D[selectedMonth - 1]} ${selectedYear}`;

        //const filtered = filterDataByMonthYear(allBillingData, selectedMonth, selectedYear);
        //const previous = getPreviousMonthYear(selectedMonth, selectedYear);
        //const previousFiltered = filterDataByMonthYear(allBillingData, previous.month, previous.year);

        //renderChart(groupDataByDate(filtered, selectedMonth, selectedYear));
        //calculateRevenueAndCollection(filtered, previousFiltered);

        getdateByMonthAndYear();
    });

    // Initial load from backend
    getdateByMonthAndYear();
});


function filterDataByMonthYear(data, month, year) {
    return data.filter(item => {
        const date = new Date(item.createdDate);
        return date.getMonth() + 1 === month && date.getFullYear() === year;
    });
}
function filterDataByMonthYearRevenue(data, month, year) {
    return data.filter(item => {
        const date = new Date(item.date);
        return date.getMonth() + 1 === month && date.getFullYear() === year;
    });
}

function getPreviousMonthYear(month, year) {
    const prevMonth = month === 1 ? 12 : month - 1;
    const prevYear = month === 1 ? year - 1 : year;
    return { month: prevMonth, year: prevYear };
}

function groupDataByDate(data, month, year) {
    const dailyTotals = {};
    const numDaysInMonth = new Date(year, month, 0).getDate();

    for (let day = 1; day <= numDaysInMonth; day++) {
        const dateKey = new Date(year, month - 1, day).toISOString().split('T')[0];
        dailyTotals[dateKey] = { invoice: 0, payment: 0 };
    }

    data.forEach(item => {
        const date = new Date(item.createdDate);
        const key = date.toISOString().split('T')[0];
        if (dailyTotals[key]) {
            dailyTotals[key].invoice += item.invoicedAmount || 0;
            dailyTotals[key].payment += item.paymentCollected || 0;
        }
    });

    const sortedDates = Object.keys(dailyTotals).sort();
    const formattedDates = sortedDates.map(date => {
        const d = new Date(date);
        const day = d.getDate();
        const shortMonth = d.toLocaleString('default', { month: 'short' });
        return `${day}-${shortMonth}`;
    });

    return {
        categories: formattedDates,
        invoices: sortedDates.map(date => dailyTotals[date].invoice),
        payments: sortedDates.map(date => dailyTotals[date].payment)
    };
}

function groupDataByDate1(data, month, year) {
    const dailyTotals = {};
    const numDaysInMonth = new Date(year, month, 0).getDate();

    for (let day = 1; day <= numDaysInMonth; day++) {
        const dateKey = new Date(year, month - 1, day).toISOString().split('T')[0];
        dailyTotals[dateKey] = { invoice: 0, payment: 0 };
    }

    data.forEach(item => {
        const date = new Date(item.date);
        const key = date.toISOString().split('T')[0];
        if (dailyTotals[key]) {
            dailyTotals[key].invoice += item.totalDueAmount || 0;
            dailyTotals[key].payment += item.payments || 0;
        }
    });

    const sortedDates = Object.keys(dailyTotals).sort();
    const formattedDates = sortedDates.map(date => {
        const d = new Date(date);
        const day = d.getDate();
        const shortMonth = d.toLocaleString('default', { month: 'short' });
        return `${day}-${shortMonth}`;
    });

    return {
        categories: formattedDates,
        invoices: sortedDates.map(date => dailyTotals[date].invoice),
        payments: sortedDates.map(date => dailyTotals[date].payment)
    };
}

function calculateRevenueAndCollection(currentData, previousData) {
    const sum = arr => arr.reduce((acc, item) => acc + (item || 0), 0);

    const currentRevenue = sum(currentData.filter(x => x.invoicedAmount != null).map(x => x.invoicedAmount));
    const currentCollection = sum(currentData.map(x => x.paymentCollected)) - sum(currentData.map(x => x.refund));
    const previousRevenue = sum(previousData.map(x => x.invoicedAmount));
    const previousCollection = sum(previousData.map(x => x.paymentCollected)) - sum(previousData.map(x => x.refund));
    let currentBalance = sum(currentData.map(x => x.balance)) + sum(currentData.map(x => x.refund)) + sum(currentData.map(x => x.creditAmount));
    let previousBalance = sum(previousData.map(x => x.balance)) + sum(previousData.map(x => x.refund)) + sum(previousData.map(x => x.creditAmount));

    //currentBalance = currentBalance < 0 ? 0 : currentBalance;
    //previousBalance = previousBalance < 0 ? 0 : previousBalance;

    const currentAdvances = sum(currentData.map(x => x.creditAmount));
    const previousAdvances = sum(previousData.map(x => x.creditAmount));

    //const currentBalanceAndAdvances = CalculateBalancesAndAdvancePayment(currentData); //sum(currentData.map(x => x.balance));
    //const previousBalanceAndAdvances = CalculateBalancesAndAdvancePayment(previousData);//sum(previousData.map(x => x.balance)

    //const currentBalance = currentBalanceAndAdvances.totalBalance;
    //const previousBalance = previousBalanceAndAdvances.totalBalance;

    //const currentAdvances = currentBalanceAndAdvances.totalAdvancePayment;
    //const previousAdvances = previousBalanceAndAdvances.totalAdvancePayment;

    const revenueChange = calculatePercentageChange(currentRevenue, previousRevenue);
    const collectionChange = calculatePercentageChange(currentCollection, previousCollection);
    const balanceChange = calculatePercentageChange(currentBalance, previousBalance);
    const advancesChange = calculatePercentageChange(currentAdvances, previousAdvances);

    // Revenue card
    document.querySelector(".rCard1 .value-text").textContent = `₹${currentRevenue.toLocaleString('en-IN')}`;
    const revLabel = document.querySelector(".rCard1 .change-positive");
    revLabel.innerHTML = revenueChange === 0 ? `No change from last month`
        : `${revenueChange > 0 ? '↑' : '↓'} ${Math.abs(revenueChange)}% from last month`;
    revLabel.style.color = revenueChange > 0 ? 'green' : revenueChange < 0 ? 'red' : 'gray';

    // Collection card
    document.querySelector(".rCard2 .value-text").textContent = `₹${currentCollection.toLocaleString('en-IN')}`;
    const collLabel = document.querySelector(".rCard2 .change-negative");
    collLabel.innerHTML = collectionChange === 0 ? `No change from last month`
        : `${collectionChange > 0 ? '↑' : '↓'} ${Math.abs(collectionChange)}% from last month`;
    collLabel.style.color = collectionChange > 0 ? 'green' : collectionChange < 0 ? 'red' : 'gray';

    // Balance card
    document.querySelector(".rCard3 .value-text").textContent = `₹${currentBalance.toLocaleString('en-IN')}`;
    const balLabel = document.querySelector(".rCard3 .change-negative");
    balLabel.innerHTML = balanceChange === 0 ? `No change from last month`
        : `${balanceChange > 0 ? '↑' : '↓'} ${Math.abs(balanceChange)}% from last month`;
    balLabel.style.color = balanceChange > 0 ? 'green' : balanceChange < 0 ? 'red' : 'gray';

    // Advances card
    document.querySelector(".rCard4 .value-text").textContent = `₹${currentAdvances.toLocaleString('en-IN')}`;
    const advLabel = document.querySelector(".rCard4 .change-negative");
    advLabel.innerHTML = advancesChange === 0 ? `No change from last month`
        : `${advancesChange > 0 ? '↑' : '↓'} ${Math.abs(advancesChange)}% from last month`;
    balLabel.style.color = advancesChange > 0 ? 'green' : advancesChange < 0 ? 'red' : 'gray';
}

function CalculateBalancesAndAdvancePayment(auditedRevenueData) {
    const latestEntries = {};

    auditedRevenueData
        .filter(x => x.isActive === true && (x.balance != null || x.advancedPayment != null))
        .forEach(entry => {
            const key = `${entry.groupId}-${entry.roomNumber}`;
            const current = latestEntries[key];

            if (!current) {
                latestEntries[key] = entry;
            } else {
                const entryDate = new Date(entry.date);
                const currentDate = new Date(current.date);

                if (
                    entryDate > currentDate ||
                    (entryDate.getTime() === currentDate.getTime() && entry.id > current.id)
                ) {
                    latestEntries[key] = entry;
                }
            }
        });

    const finalEntries = Object.values(latestEntries).map(x => ({
        GroupId: x.groupId,
        RoomNumber: x.roomNumber,
        Balance: x.balance || 0,
        AdvancePayment: x.advancedPayment || 0
    }));

    const totalBalance = finalEntries.reduce((sum, x) => sum + x.Balance, 0);
    const totalAdvancePayment = finalEntries.reduce((sum, x) => sum + x.AdvancePayment, 0);

    return {
        totalBalance,
        totalAdvancePayment,
        details: finalEntries
    };
}


function CalculateBalances(auditedRevenueData) {
    const latestBalances = {};

    auditedRevenueData
        .filter(x => x.isActive === true && x.balance != null)
        .forEach(entry => {
            const key = `${entry.groupId}-${entry.roomNumber}`;
            const current = latestBalances[key];

            if (!current) {
                latestBalances[key] = entry;
            } else {
                const entryDate = new Date(entry.date);
                const currentDate = new Date(current.date);

                if (
                    entryDate > currentDate ||
                    (entryDate.getTime() === currentDate.getTime() && entry.id > current.id)
                ) {
                    latestBalances[key] = entry;
                }
            }
        });

    const finalBalances = Object.values(latestBalances).map(x => ({
        GroupId: x.groupId,
        RoomNumber: x.roomNumber,
        Balance: x.balance
    }));

    const currentBalance = finalBalances.reduce((sum, x) => sum + (x.Balance || 0), 0);
    return currentBalance;
}


function CalculateBalances1(auditedRevenueData) {
    const latestBalances = {};

    auditedRevenueData
        .filter(x => x.isActive === true && x.balance != null)
        .forEach(entry => {
            const key = `${entry.groupId}-${entry.roomNumber}`;
            const current = latestBalances[key];

            if (!current || new Date(entry.date) > new Date(current.date)) {
                latestBalances[key] = entry;
            }
        });

    const finalBalances = Object.values(latestBalances).map(x => ({
        GroupId: x.groupId,
        RoomNumber: x.roomNumber,
        Balance: x.balance
    }));

    // To get total current balance:
    const currentBalance = finalBalances.reduce((sum, x) => sum + (x.Balance || 0), 0);
    return currentBalance;
    //console.log(finalBalances);
    //console.log('Total Current Balance:', currentBalance);

}

function calculatePercentageChange(current, previous) {
    if (previous === 0) {
        return current > 0 ? 100 : 0;
    }
    return parseFloat(((current - previous) / previous) * 100).toFixed(2);
}

function renderChart(data) {
    const chartOptions = {
        series: [{
            name: 'Invoice Amount',
            data: data.invoices,
            color: '#4F3E9E'
        }, {
            name: 'Payment Collected',
            data: data.payments,
            color: '#6EBCB7'
        }],
        chart: {
            type: 'area',
            height: 350
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            width: [0, 3],
            curve: 'smooth'
        },
        xaxis: {
            categories: data.categories,
            labels: {
                rotate: -45
            }
        },
        yaxis: {
            labels: {
                formatter: val => '₹' + val.toLocaleString('en-IN')
            }
        },
        tooltip: {
            y: {
                formatter: val => '₹' + val.toLocaleString('en-IN')
            }
        },
        legend: {
            position: 'top',
            horizontalAlign: 'right',
            floating: true,
            offsetY: -25,
            offsetX: -5
        }
    };

    const chartContainer = document.querySelector("#revenueChart");
    chartContainer.innerHTML = ""; // clear previous chart
    const chart = new ApexCharts(chartContainer, chartOptions);
    chart.render();
}

function renderChart1(data) {
    const chartOptions = {
        series: [{
            name: 'Invoice Amount',
            data: data.invoices,
            color: '#4F3E9E'
        }, {
            name: 'Payment Collected',
            data: data.payments,
            color: '#6EBCB7'
        }],
        chart: {
            type: 'area',
            height: 350
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            width: [0, 3],
            curve: 'smooth'
        },
        xaxis: {
            categories: data.categories,
            labels: {
                rotate: -45
            }
        },
        yaxis: {
            labels: {
                formatter: val => '₹' + val.toLocaleString('en-IN')
            }
        },
        tooltip: {
            y: {
                formatter: val => '₹' + val.toLocaleString('en-IN')
            }
        },
        legend: {
            position: 'top',
            horizontalAlign: 'right',
            floating: true,
            offsetY: -25,
            offsetX: -5
        }
    };

    const chartContainer = document.querySelector("#revenueChart");
    chartContainer.innerHTML = ""; // clear previous chart
    const chart = new ApexCharts(chartContainer, chartOptions);
    chart.render();
}




function getdateByMonthAndYear() {
    let inputDTO = {};
    inputDTO.Month = parseInt($('#month').val());
    inputDTO.Year = parseInt($('#year').val());
    $.ajax({
        type: "POST",
        url: "/SalesReport/GetServicesForBilling",
        contentType: 'application/json',
        data: JSON.stringify(inputDTO),
        success: function (data) {
            let result = data.settlements;
            allBillingData = result; // store all data
            allAuditRevenueData = data.auditedRevenues; // store all data
            const initialMonth = inputDTO.Month;
            const initialYear = inputDTO.Year;

            const filtered = filterDataByMonthYear(allBillingData, initialMonth, initialYear);
            const previous = getPreviousMonthYear(initialMonth, initialYear);
            const previousFiltered = filterDataByMonthYear(allBillingData, previous.month, previous.year);

            const filteredAuditRevenueData = filterDataByMonthYearRevenue(allAuditRevenueData, initialMonth, initialYear);
            const previousFilteredAuditRevenueData = filterDataByMonthYearRevenue(allAuditRevenueData, previous.month, previous.year);

            document.getElementById("currentMonthLabel").textContent = `${monthNames_D[initialMonth - 1]} ${initialYear}`;
            renderChart(groupDataByDate(filtered, initialMonth, initialYear));
            //renderChart1(groupDataByDate1(filteredAuditRevenueData, initialMonth, initialYear));
            //calculateRevenueAndCollection(filteredAuditRevenueData, previousFilteredAuditRevenueData);
            calculateRevenueAndCollection(filtered, previousFiltered);
        },
        error: function (error) {
            console.error("Failed to fetch billing data:", error.responseText);
        }
    });
}