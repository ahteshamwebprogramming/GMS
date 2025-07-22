let timeoutMinutes = 30;
let warningSecondsBefore = 30;
let timeoutMs = timeoutMinutes * 60 * 1000;
let warningMs = timeoutMs - (warningSecondsBefore * 1000);

let interval;

setTimeout(function () {
    document.getElementById('session-warning').style.display = 'block';
    let countdown = warningSecondsBefore;
    document.getElementById('countdown-seconds').innerText = countdown;

    interval = setInterval(function () {
        countdown--;
        document.getElementById('countdown-seconds').innerText = countdown;
        if (countdown <= 0) {
            clearInterval(interval);
            window.location.href = '/Account/Logout';
        }
    }, 1000);
}, warningMs);

function stayLoggedIn() {
    fetch('/Account/KeepAlive', { method: 'POST' })
        .then(() => {
            clearInterval(interval);
            document.getElementById('session-warning').style.display = 'none';
            location.reload(); // restart timers by refreshing the page
        });
}