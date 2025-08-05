document.addEventListener('DOMContentLoaded', function () {
    const hamburger = document.querySelector('.hamburger');
    const leftContainer = document.querySelector('.left_container');

    hamburger.addEventListener('click', function () {
        leftContainer.classList.toggle('active');
    });

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function (event) {
        if (window.innerWidth <= 1024 &&
            !leftContainer.contains(event.target) &&
            !hamburger.contains(event.target) &&
            leftContainer.classList.contains('active')) {
            leftContainer.classList.remove('active');
        }
    });
});

$(document).ready(function () {
    $('.numeric').forceNumeric();
    $('.numeric1').forceNumeric1();

    //document.querySelectorAll('.number').forEach(input => {
    //    input.addEventListener('input', function () {
    //        // Get the character limit from the charslimit attribute
    //        const charLimit = parseInt(this.getAttribute('charslimit'), 10) || 0;

    //        // Allow only numbers and limit to the charLimit
    //        this.value = this.value.replace(/[^0-9]/g, '').slice(0, charLimit);
    //    });
    //});
});

var myspinner1 = '<div class="preloader" id="preloader"><img src="/assets/img/loader.gif"></div>';

var myspinner = '<div class="spinner-border spinner-border-lg text-primary" role="status"><span class="visually-hidden"> Loading...</span></div>';
function BlockUI() {
    $.blockUI({ message: myspinner1 });
}
function UnblockUI() {
    $.unblockUI();
}
function $successalert(title, message) {
    Swal.fire({
        title: title,
        text: message,
        icon: 'success',
        customClass: {
            confirmButton: 'btn btn-success'
        },
        buttonsStyling: false
    });
}
function $erroralert(title, message) {
    Swal.fire({
        title: title,
        text: message,
        icon: 'error',
        customClass: {
            confirmButton: 'btn btn-primary'
        },
        buttonsStyling: false
    });
}

function $sadalert(title, message) {
    Swal.fire({
        title: title,
        text: message,
        imageUrl: 'https://cdn-icons-png.flaticon.com/512/742/742753.png', // Sad face icon
        imageWidth: 64,
        imageHeight: 64,
        imageAlt: 'Sad face',
        customClass: {
            confirmButton: 'btn btn-secondary'
        },
        buttonsStyling: false
    });
}

function $sadConfirmation(title, message, okText, iconType, onConfirmAction) {
    const iconMap = {
        sad: 'https://cdn-icons-png.flaticon.com/512/742/742753.png',
        smile: 'https://cdn-icons-png.flaticon.com/512/742/742751.png',
        tick: 'https://cdn-icons-png.flaticon.com/512/845/845646.png'
    };
    const imageUrl = iconMap[iconType] || iconMap.sad;
    Swal.fire({
        title: title,
        text: message,
        imageUrl: imageUrl,
        imageWidth: 60,
        imageHeight: 60,
        imageAlt: 'Sad face',
        showCancelButton: false,
        confirmButtonText: okText,
        customClass: {
            confirmButton: 'btn btn-primary'
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed && typeof onConfirmAction === 'function') {
            onConfirmAction(); // Perform redirection or any other action
        }
    });
}


jQuery.fn.forceNumeric = function () {
    return this.each(function () {
        const $this = $(this);
        const charLimit = parseInt($this.attr('charslimit'), 10) || 0;
        $(this).keydown(function (e) {
            var key = e.which || e.keyCode;

            if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
                // numbers   
                key >= 48 && key <= 57 ||
                //Numeric keypad
                key >= 96 && key <= 105 ||
                // comma, period and minus, . on keypad
                //key == 190 || key == 188 || key == 109 || key == 110 ||
                // Backspace and Tab and Enter
                key == 8 || key == 9 || key == 13 ||
                // Home and End
                key == 35 || key == 36 ||
                // left and right arrows
                key == 37 || key == 39 //||
                // Del and Ins
                //key == 45
            )
                return true;

            return false;
        });
        $this.on('input', function () {
            let value = $this.val().replace(/[^0-9]/g, ''); // Remove non-numeric
            if (charLimit > 0) {
                value = value.slice(0, charLimit); // Enforce character limit
            }
            $this.val(value);
        });
    });
}
jQuery.fn.forceNumeric1 = function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.which || e.keyCode;

            if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
                // numbers   
                key >= 48 && key <= 57 ||
                //Numeric keypad
                key >= 96 && key <= 105 ||
                // comma 
                //key == 188 || 
                //period 
                key == 190 ||
                //minus, .on keypad
                //key == 109 || key == 110 ||
                // Backspace and Tab and Enter
                key == 8 || key == 9 || key == 13 ||
                // Home and End
                key == 35 || key == 36 ||
                // left and right arrows
                key == 37 || key == 39 //||
                // Del and Ins
                //key == 45
            )
                return true;

            return false;
        });
    });
}

function numberToWords(num) {
    const a = [
        '', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine', 'Ten',
        'Eleven', 'Twelve', 'Thirteen', 'Fourteen', 'Fifteen', 'Sixteen', 'Seventeen',
        'Eighteen', 'Nineteen'
    ];
    const b = ['', '', 'Twenty', 'Thirty', 'Forty', 'Fifty', 'Sixty', 'Seventy', 'Eighty', 'Ninety'];

    if ((num = num.toString()).length > 9) return 'overflow';
    let n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
    if (!n) return;

    let str = '';
    str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + ' Crore ' : '';
    str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + ' Lakh ' : '';
    str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + ' Thousand ' : '';
    str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + ' Hundred ' : '';
    str += (n[5] != 0) ? ((str != '') ? 'and ' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) : '';
    return str.trim();
}

// Example usage:
console.log(numberToWords(1000));  // Output: "one thousand"
