// Helper: Get relative time (returns a string like "2 minutes ago")
function getRelativeTime(date) {
    const now = new Date();
    const diff = now - date;
    if (isNaN(diff)) return "Unknown time";
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(diff / (1000 * 60));
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));

    if (seconds < 60) return "Just now";
    if (minutes < 60) return `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
    if (hours < 24) return `${hours} hour${hours > 1 ? "s" : ""} ago`;
    return `${days} day${days > 1 ? "s" : ""} ago`;
}

// SignalR connection (using negotiation to allow fallback transports)
const token = localStorage.getItem("authToken"); // Must be set at login
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub", { accessTokenFactory: () => token })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

function startConnection() {
    connection.start()
        .then(() => console.log("Connected to SignalR Hub"))
        .catch(err => {
            console.error("SignalR Connection Error:", err);
            toastr.error("Failed to connect to notifications service. Reconnecting...");
            setTimeout(startConnection, 5000);
        });
}

connection.on("ReceiveNotification", (data) => {

    // Update dropdown badge
    updateNotificationCount(1);

    // Prepend new notification (with fade-in)
    const notificationList = document.getElementById("notificationItems");
    const newNotificationHTML = addNotification(data);
    const tempContainer = document.createElement("div");
    tempContainer.innerHTML = newNotificationHTML;
    const newElement = tempContainer.firstElementChild;
    newElement.style.display = "none";
    notificationList.insertAdjacentElement("afterbegin", newElement);
    newElement.style.display = "";
    $(newElement).hide().fadeIn(500); // Using jQuery here only for fadeIn

    // Display a toast
    toastr.info("🔔 " + message);
});

function addNotification(data) {
    const relativeTime = getRelativeTime(new Date(data.CreatedAt));
    const unreadClass = data.IsRead ? "" : "unread-notification";
    return `
        <li class="dropdown-item notification-item ${unreadClass}">
            <a href="${data.Link}" class="text-decoration-none text-golden notification-link" data-id="${data.Id}">
                ${data.Message}
                <div class="text-end mt-1">
                    <small class="text-muted">${relativeTime}</small>
                </div>
            </a>
        </li>
    `;
}

// Mark notification as read on click (using delegation)
document.addEventListener("click", function (event) {
    const link = event.target.closest(".notification-link");
    if (link) {
        const notificationId = link.getAttribute("data-id");
        fetch(`/Notification/MarkAsRead?id=${notificationId}`, {
            method: "POST",
            headers: { "Authorization": `Bearer ${token}` }
        }).then(response => {
            if (response.ok) {
                const item = link.closest("li");
                if (item) {
                    item.classList.remove("unread-notification");
                }
                updateNotificationCount(-1);
            }
        }).catch(error => console.error("Error marking as read:", error));
    }
});

function updateNotificationCount(delta) {
    const badge = document.getElementById("notificationBadge");
    let maxCount = 9;
    let count = Math.max(0, (parseInt(badge.textContent) || 0) + delta)
    if (count > maxCount) {
        badge.textContent = maxCount + '+';
    } else {
        badge.textContent = count;
    }
}

// Fetch notifications for the dropdown (limit to 5)
async function fetchNotifications() {
    let limit = 5;
    const notificationList = document.getElementById("notificationItems");
    notificationList.innerHTML = `
        <div class="text-center p-3">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>`;
    try {
        const response = await fetch("/Notification/GetNotifications?limit=" + limit, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });
        if (!response.ok) throw new Error("Failed to fetch notifications");
        const result = await response.json();
        renderNotifications(result);
    } catch (error) {
        console.error("Error fetching notifications:", error);
        notificationList.innerHTML = `<div class="p-3 text-center text-danger">Failed to load notifications.</div>`;
    }
}

function renderNotifications(data) {
    const notificationList = document.getElementById("notificationItems");
    let html = "";
    data.Notifications.forEach(notification => {
        html += addNotification(notification);
    });
    notificationList.innerHTML = html;
    updateNotificationCount(data.UnreadCount)
}

// Initialize
document.addEventListener("DOMContentLoaded", () => {
    startConnection();
    fetchNotifications();
});
connection.onclose(() => startConnection());
