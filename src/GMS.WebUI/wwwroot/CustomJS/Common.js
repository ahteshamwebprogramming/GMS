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

var myspinner1 = '<div class="preloader" id="preloader"><img src="../assets/img/loader.gif"></div>';
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

