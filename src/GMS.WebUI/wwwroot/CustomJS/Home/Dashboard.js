
let gmonth = 0;
let gyear = 0;
//$(function () {
//    $('#inputmonthyearselector').datepicker({
//        dateFormat: 'mm/yy',
//        changeMonth: true,
//        changeYear: true,
//        showButtonPanel: true,

//        onClose: function (dateText, inst) {
//            const monthIndex = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
//            const year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();

//            const monthAbbr = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
//                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

//            const display = `${monthAbbr[monthIndex]}-${year}`;
//            $('#inputmonthyearselector').val(display);

//            // Pass these values to your logic
//            const month = (parseInt(monthIndex) + 1).toString().padStart(2, '0');
//            window.location.href = `/Home/Dashboard?month=${month}&year=${year}`;
//            ReveneVsOccupancy(month, year);
//        },

//        beforeShow: function (input, inst) {
//            // Add class to hide calendar
//            $(input).datepicker("widget").addClass("hide-calendar");
//        }
//    });
//});
const scrollContainer = document.getElementById("scrollContainer");
const scrollLeft = document.getElementById("scrollLeft");
const scrollRight = document.getElementById("scrollRight");
const scrollStep = 200; // Pixels to scroll on each click

function updateButtons() {
    scrollLeft.classList.toggle(
        "hidden", scrollContainer
            .scrollLeft <= 0);
    scrollRight.classList.toggle(
        "hidden", scrollContainer
            .scrollLeft +
        scrollContainer
            .clientWidth >=
    scrollContainer.scrollWidth);
}

scrollLeft.addEventListener("click",
    () => {
        scrollContainer.scrollBy({
            left: -scrollStep,
            behavior: "smooth"
        });
    });

scrollRight.addEventListener("click",
    () => {
        scrollContainer.scrollBy({
            left: scrollStep,
            behavior: "smooth"
        });
    });

scrollContainer.addEventListener(
    "scroll", updateButtons);
window.addEventListener("resize",
    updateButtons);

updateButtons
    (); // Ensure correct button visibility on load


function ReveneVsOccupancy(month, year) {
    BlockUI();
    $.ajax({
        type: "POST",
        url: `/Home/GetDashboardChartRevenueVsOccupancy?month=${month}&year=${year}`,
        contentType: 'application/json',
        //data: JSON.stringify(inputDTO),
        success: function (data) {
            UnblockUI();
            ADRVsRevPAR(data);
            var revenueData = [];
            // Group distinct dates from RoomOccupancyDataList
            var dates = [...new Set(data.roomOccupancyDataList.map(x => x.theDate))];

            dates.forEach(function (date) {
                var dateDataRoom = data.roomRevenueDataList.find(x => x.theDate === date);
                var dateDataFnB = data.packageRevenueDataList.find(x => x.theDate === date);
                var dateDataUpsell = data.upsellRevenueDataList.find(x => x.theDate === date);
                var totalRevenue = (dateDataRoom?.totalRevenue || 0) + (dateDataFnB?.totalRevenue || 0) + (dateDataUpsell?.totalRevenue || 0);
                revenueData.push(parseFloat(totalRevenue.toFixed(0))); // optional: round to 2 decimals
            });

            var occupancyPercentages = [];
            dates.forEach(function (date) {
                var dateData = data.roomOccupancyDataList.filter(x => x.theDate === date);
                var bookedRooms = dateData.reduce((sum, x) => sum + (x.bookedRooms || 0), 0);
                var totalRooms = dateData.reduce((sum, x) => sum + (x.totalRooms || 0), 0);
                var totalOccupancyPercent = totalRooms > 0 ? (bookedRooms * 100) / totalRooms : 0;
                occupancyPercentages.push(parseFloat(totalOccupancyPercent.toFixed(0)));
            });

            var formattedDates = dates.map(dateStr => {
                return moment(dateStr).format("D-MMMM-YYYY");  // e.g., "1-March-2025"
            });
            var options = {
                series: [{
                    name: 'Revenue',
                    type: 'column',
                    data: revenueData
                },
                {
                    name: 'Occupancy Rate',
                    type: 'line',
                    data: occupancyPercentages
                }
                ],
                chart: {
                    height: 350,
                    type: 'line',
                },
                stroke: {
                    width: [0, 4]
                },
                // title: {
                //   text: 'Traffic Sources'
                // },
                dataLabels: {
                    enabled: true,
                    enabledOnSeries: [1]
                },
                labels: formattedDates,
                yaxis: [{
                    title: {
                        text: 'Revenue',
                    },
                },
                {
                    opposite: true,
                    title: {
                        text: 'Occupancy Rate'
                    }
                }
                ]
            };

            var chart =
                new ApexCharts(
                    document
                        .querySelector(
                            "#ReveneVsOccupancy"
                        ),
                    options
                );
            chart.render();
        },
        error: function (error) {
            UnblockUI();
        }
    });
}

function ADRVsRevPAR(data) {
    var adrValues = [];
    var revparValues = [];
    var dates = [...new Set(data.roomOccupancyDataList.map(x => x.theDate))];
    dates.forEach(function (date) {
        var roomOccupied = data.roomOccupancyDataList.filter(x => x.theDate === date).reduce((sum, x) => sum + (x.bookedRooms || 0), 0);
        var roomRevenue = data.roomRevenueDataList.find(x => x.theDate === date)?.totalRevenue || 0;
        var adr = roomOccupied === 0 ? 0 : roomRevenue / roomOccupied;
        adrValues.push(parseFloat(adr.toFixed(0)));

        var totalRooms = data.roomOccupancyDataList.filter(x => x.theDate === date).reduce((sum, x) => sum + (x.totalRooms || 0), 0);
        var revpar = totalRooms === 0 ? 0 : roomRevenue / totalRooms;
        revparValues.push(parseFloat(revpar.toFixed(0)));
    });
    var formattedDates = dates.map(dateStr => {
        return moment(dateStr).format("D-MMM-YYYY");  // e.g., "1-March-2025"
    });
    var options = {
        series: [{
            name: "ADR",
            data: adrValues
        },
        {
            name: "RevPAR",
            data: revparValues
        }
        ],
        chart: {
            height: 350,
            type: 'line',
            dropShadow: {
                enabled: true,
                color: '#000',
                top: 18,
                left: 7,
                blur: 10,
                opacity: 0.5
            },
            zoom: {
                enabled: false
            },
            toolbar: {
                show: false
            }
        },
        colors: ['#77B6EA',
            '#545454'
        ],
        dataLabels: {
            enabled: true,
        },
        stroke: {
            curve: 'smooth'
        },
        // title: {
        //   text: 'Average High & Low Temperature',
        //   align: 'left'
        // },
        grid: {
            borderColor: '#e7e7e7',
            row: {
                colors: ['#f3f3f3',
                    'transparent'
                ], // takes an array which will be repeated on columns
                opacity: 0.5
            },
        },
        markers: {
            size: 1
        },
        xaxis: {
            categories: formattedDates,
            title: {
                text: 'Month'
            }
        },
        yaxis: {
            title: {
                text: 'Rate'
            },
            min: 5,
            //max: 40
        },
        legend: {
            position: 'top',
            horizontalAlign: 'right',
            floating: true,
            offsetY: -25,
            offsetX: -5
        }
    };

    var chart = new ApexCharts(document
        .querySelector(
            "#ADRVsRevPAR"), options
    );
    chart.render();
}