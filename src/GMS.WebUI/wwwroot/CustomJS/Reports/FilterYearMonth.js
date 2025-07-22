const monthNames_D = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];
$(document).ready(function () {
    const currentDate_Filter = new Date();

    const currentMonth_D = $('#month').attr("selectedattr");
    const currentYear_D = $('#year').attr("selectedattr");

    monthNames_D.forEach((month, index) => {
        const monthValue_D = index + 1;
        $('#month').append(
            `<option value="${monthValue_D}" ${monthValue_D == currentMonth_D ? 'selected' : ''}>${month}</option>`
        );
    });

    // Populate year dropdown (current year and previous 4 years)
    for (let i = 0; i < 5; i++) {
        const year_D = currentYear_D - i;
        $('#year').append(
            `<option value="${year_D}" ${year_D == currentYear_D ? 'selected' : ''}>${year_D}</option>`
        );
    }

});