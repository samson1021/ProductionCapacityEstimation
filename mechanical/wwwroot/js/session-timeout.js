// Handles session timeout notification and redirect
function showSessionTimeoutNotification() {
    if (window.toastr) {
        toastr.warning('Your session has expired. Please log in again.', 'Session Timeout');
    } else {
        alert('Your session has expired. Please log in again.');
    }
    setTimeout(function () {
        window.location.href = '/Home/Index';
    }, 3000);
}

// Global AJAX error handler for session timeout
if (window.jQuery) {
    $(document).ajaxError(function(event, jqxhr) {
        if (jqxhr.status === 401 || jqxhr.status === 440) {
            showSessionTimeoutNotification();
        }
    });
}

(function() {
    // Get session timeout from a data attribute (in minutes), fallback to 10 minutes if not set
    var timeoutMinutes = 20;
    var timeoutAttr = document.body.getAttribute('data-session-timeout-minutes');
    if (timeoutAttr && !isNaN(parseInt(timeoutAttr))) {
        timeoutMinutes = parseInt(timeoutAttr);
    }
    var sessionTimeout = timeoutMinutes * 60 * 1000;
    var sessionTimer = setTimeout(showSessionTimeoutNotification, sessionTimeout);
    function resetSessionTimer() {
        clearTimeout(sessionTimer);
        sessionTimer = setTimeout(showSessionTimeoutNotification, sessionTimeout);
        // Ping the server to keep session alive (sliding expiration)
        fetch('/Home/Ping', { method: 'GET', credentials: 'same-origin' });
    }
    ['click', 'mousemove', 'keydown', 'scroll'].forEach(function(evt) {
        document.addEventListener(evt, resetSessionTimer, false);
    });
})();
