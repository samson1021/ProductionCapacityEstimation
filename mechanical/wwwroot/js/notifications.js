// SignalR Connection
const connection = new signalR.HubConnectionBuilder()
    // .withUrl("/notificationHub")
    // // .withUrl("/notificationHub", {
    // //     accessTokenFactory: () => localStorage.getItem('authToken')
    // // })
    .withUrl("/notificationHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Reconnect logic
let reconnectTimeout;
function startConnection() {
    connection.start()
        .then(() => {
            console.log("Connected to SignalR Hub");
            if (reconnectTimeout) {
                clearTimeout(reconnectTimeout);
                reconnectTimeout = null;
            }
        })
        .catch(err => {
            console.error("SignalR Connection Error:", err);
            toastr.error("Failed to connect to notifications service. Reconnecting...");
            reconnectTimeout = setTimeout(startConnection, 5000);
        });
}

connection.on("ReceiveNotification", function (message) {
    
    // Update UI with the new notification
    updateNotificationBadge();
    showNotificationDropdown(message);

});


function addNotification(data) {
    return `
        <li class="dropdown-item">
            <div class="d-flex justify-content-between">
                <a href="${data.link}" class="text-decoration-none text-dark">
                    ${data.message}
                </a>
                <button class="btn btn-sm btn-link mark-as-read" data-id="${data.id}">
                    <i class="bi bi-check2"></i>
                </button>
            </div>
            <small class="text-muted">Just now</small>
        </li>
    `;
}

// Notification Handler
connection.on("ReceiveNotification", (data) => {
    // Update badge
    const badge = document.getElementById("notificationBadge");
    badge.textContent = parseInt(badge.textContent || 0) + 1;

    // Add to dropdown    
    $("#notificationItems").prepend(addNotification(data));
    
    // Show Alert
    alert("🔔 Notification: " + message);
    console.log("New notification:", message);

    // Show Toast Notification
    toastr.success(message);
    toastr.info(message);
});

// Mark as Read
$(document).on("click", ".mark-as-read", function() {
    const notificationId = $(this).data("id");
    $.post(`/Notification/MarkAsRead?id=${notificationId}`, () => {
        $(this).closest("li").remove();
        updateNotificationCount(-1);
    });
});

// Initialize
startConnection();
connection.onclose(() => startConnection());

// Helper function
function updateNotificationCount(delta) {
    const badge = document.getElementById("notificationBadge");
    badge.textContent = Math.max(0, parseInt(badge.textContent || 0) + delta);
}

// async function fetchNotifications() {
//     const response = await fetch('/Notification/getNotifications');
//     const notifications = await response.json();
//     renderNotifications(notifications);
// }

// document.getElementById("notification-list").addEventListener("click", function (event) {
//     let notificationId = event.target.getAttribute("data-id");
//     if (notificationId) {
//         fetch(`/api/notifications/markAsRead/${notificationId}`, { method: "POST" })
//             .then(() => event.target.classList.add("text-muted"));
//     }
// });
