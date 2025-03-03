// Helper
// function convertToUTC(date) {
//     return Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
//                      date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
// }

// Helper function to calculate relative time
function getRelativeTime(date) {
    if (typeof date !== "string" || !date.trim()) return "Unknown time";
    if (!date.endsWith("Z")) date += "Z";

    date = new Date(date);
    if (!(date instanceof Date)) return "Unknown time";
    if (isNaN(date)) return "Invalid date";

    // date = (new Date(Date.parse(date)))
    const now = new Date();
    const diff = date - now;
    // const diff = convertToUTC(date) - convertToUTC(now);

    const seconds = Math.floor(Math.abs(diff) / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (diff > 0) {
        return formatFutureTime(seconds, minutes, hours, days);
    } else {
        return formatPastTime(seconds, minutes, hours, days);
    }
}

function formatFutureTime(seconds, minutes, hours, days) {
    if (seconds < 60) return "In a few seconds";
    if (minutes < 60) return `In ${minutes} minute${minutes > 1 ? "s" : ""}`;
    if (hours < 24) return `In ${hours} hour${hours > 1 ? "s" : ""}`;
    if (days === 1) return "Tomorrow";
    return `In ${days} day${days > 1 ? "s" : ""}`;
}

function formatPastTime(seconds, minutes, hours, days) {
    if (seconds < 60) return "Just now";
    if (minutes < 60) return `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
    if (hours < 24) return `${hours} hour${hours > 1 ? "s" : ""} ago`;
    if (days === 1) return "Yesterday";
    return `${days} day${days > 1 ? "s" : ""} ago`;
}

// Add a new notification to the UI
function addNotificationToUI(data) {
    const notificationList = document.getElementById("notificationItems");
    if (!notificationList) return;

    const newNotificationHTML = generateNotificationHTML(data);
    // const newNotificationHTML = generateNotificationHTML({
    //     Id: data.id,
    //     Message: data.message,
    //     Link: data.link,
    //     IsRead: false,
    //     CreatedAt: new Date().toISOString()
    // });
    const tempContainer = document.createElement("div");
    tempContainer.innerHTML = newNotificationHTML;
    const newElement = tempContainer.firstElementChild;

    newElement.style.display = "none";
    notificationList.insertAdjacentElement("afterbegin", newElement);
    newElement.style.display = "";
    $(newElement).hide().fadeIn(500);
}

// SignalR connection setup (using negotiation to allow fallback transports)
function setupSignalRConnection() {
    const token = localStorage.getItem("authToken");
    if (!token) {
        console.error("No authentication token found");
        toastr.error("Authentication token missing. Please log in again.");
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub", { accessTokenFactory: () => token })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveNotification", (data) => {
        updateNotificationBadge(1);
        addNotificationToUI(data);
        toastr.info("🔔 " + data.message);
    });

    connection.onclose(() => {
        console.warn("SignalR connection lost. Reconnecting...");
        toastr.warning("Connection lost. Reconnecting...");
        setTimeout(startConnection, 5000);
    });

    function startConnection() {
        connection.start()
            .then(() => console.log("Connected to SignalR Hub"))
            .catch(err => {
                console.error("SignalR Connection Error:", err);
                toastr.error("Failed to connect to notifications service. Reconnecting...");
                setTimeout(startConnection, 5000);
            });
    }
    startConnection();
}

// Generate HTML for a notification
function generateNotificationHTML(data) {
    const relativeTime = getRelativeTime(data.CreatedAt);
    const unreadClass = data.IsRead ? "" : "unread-notification";
    return `
        <li class="dropdown-item notification-item ${unreadClass}">
            <a href="${data.Link}" class="text-decoration-none text-golden notification-link" data-id="${data.Id}" role="button">
                ${data.Message}
                <div class="text-end mt-1">
                    <small class="text-muted">${relativeTime}</small>
                </div>
            </a>
        </li>
    `;
}

// Mark a single notification as read
function markNotificationAsRead(id, element) {
    $.ajax({
        url: "/Notification/MarkAsRead?id=" + id,
        method: "POST",
        contentType: "application/json"
    }).done(function () {
        $(element).removeClass("border-warning");
        $(element).find("h5").removeClass("fw-bold");
        updateNotificationBadge(-1);
    }).fail(function (err) {
        console.error("Error marking notification as read", err);
    });
}

// Mark all notifications as read
function markAllAsRead() {
    $.ajax({
        url: "/Notification/MarkAllAsRead",
        method: "POST",
        contentType: "application/json"
    }).done(function () {
        // Remove unread styles from all notifications
        $(".list-group-item").removeClass("border-warning");
        $(".list-group-item h5").removeClass("fw-bold");
        updateNotificationBadge(0);
        collapseNotificationDropdown();
    }).fail(function (err) {
        console.error("Error marking all notifications as read", err);
    });
}

// Collapse the notification dropdown
function collapseNotificationDropdown() {
    const dropdown = document.getElementById("notificationDropdown");
    if (dropdown && dropdown.classList.contains("show")) {
        dropdown.classList.remove("show");
        dropdown.setAttribute("aria-expanded", "false");
    }
}

// Update the notification badge count
function updateNotificationBadge(delta) {
    const badge = document.getElementById("notificationBadge");
    if (!badge) return;

    if (delta === 0) badge.textContent = "0";
    else{
        let maxCount = 9;
        let count = Math.max(0, (parseInt(badge.textContent) || 0) + delta);
        badge.textContent = count > maxCount ? `${maxCount}+` : count;
    }
}

// Fetch notifications for the dropdown
async function fetchNotifications() {
    const token = localStorage.getItem("authToken");
    if (!token) {
        console.error("No authentication token found");
        return;
    }

    let limit = 5;
    const notificationList = document.getElementById("notificationItems");
    if (!notificationList) return;

    notificationList.innerHTML = `
        <div class="text-center p-3">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>`;

    try {
        const response = await fetch("/Notification/GetNotifications?limit=" + limit, {
            method: "GET",
            headers: {"Authorization": `Bearer ${token}`, "Content-Type": "application/json"}
        });
        if (!response.ok) throw new Error("Failed to fetch notifications");
        const result = await response.json();
        renderNotifications(result);
    } catch (error) {
        console.error("Error fetching notifications:", error);
        notificationList.innerHTML = `<div class="p-3 text-center text-danger">Failed to load notifications.</div>`;
    }
}

// Render notifications in the dropdown
function renderNotifications(data) {
    const notificationList = document.getElementById("notificationItems");
    if (!notificationList) return;

    let html = "";
    data.Notifications.forEach(notification => {
        html += generateNotificationHTML(notification);
        // html += addNotification(notification);
    });
    notificationList.innerHTML = html;
    updateNotificationBadge(data.UnreadCount);
}

// Mark notification as read on click (using delegation)
document.addEventListener("click", function (event) {
    const link = event.target.closest(".notification-link");
    if (link) {
        const notificationId = link.getAttribute("data-id");
        const token = localStorage.getItem("authToken");
        if (!token) {
            console.error("No authentication token found");
            return;
        }
        fetch(`/Notification/MarkAsRead?id=${notificationId}`, {
            method: "POST",
            headers: { "Authorization": `Bearer ${token}` }
        }).then(response => {
            if (response.ok) {
                const item = link.closest("li");
                if (item) {
                    item.classList.remove("unread-notification");
                }
                updateNotificationBadge(-1);
            }
        }).catch(error => console.error("Error marking as read:", error));
    }
});

// Initialize the notification system
document.addEventListener("DOMContentLoaded", () => {
    setupSignalRConnection();
    fetchNotifications();
});

// connection.onclose(() => setupSignalRConnection());