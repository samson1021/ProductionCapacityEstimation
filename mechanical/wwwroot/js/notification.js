// Helper function to calculate relative time
function getRelativeTime(date) {
    if (typeof date !== "string" || !date.trim()) return "Unknown time";
    if (!date.endsWith("Z")) date += "Z";

    date = new Date(date);
    if (!(date instanceof Date) || isNaN(date)) return "Invalid date";

    const now = new Date();
    const diff = now - date;

    const seconds = Math.floor(Math.abs(diff) / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (seconds < 60) return "Just now";
    if (minutes < 60) return `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
    if (hours < 24) return `${hours} hour${hours > 1 ? "s" : ""} ago`;
    if (days === 1) return "Yesterday";
    return `${days} day${days > 1 ? "s" : ""} ago`;
}

// SignalR connection setup
function setupSignalRConnection() {
    const token = localStorage.getItem("authToken");
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub", { accessTokenFactory: () => token })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveNotification", (data) => {
        updateNotificationBadge(1);
        addNotificationToUI(data);
        toastr.info("🔔 " + data.content);
    });

    connection.onclose(() => {
        console.warn("SignalR connection lost. Reconnecting...");
        toastr.warning("Connection lost. Reconnecting...");
        setTimeout(() => connection.start(), 5000);
    });

    connection.start()
        .then(() => console.log("Connected to SignalR Hub"))
        .catch(err => {
            console.error("SignalR Connection Error:", err);
            toastr.error("Failed to connect to notifications service. Reconnecting...");
            setTimeout(() => connection.start(), 5000);
        });
}

// Mark a single notification as read
async function markNotificationAsRead(id, element) {
    try {
        const response = await fetch(`/Notification/MarkAsRead?ids=${id}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        });
        if (!response.ok) throw new Error("Failed to mark as read");
        if (element) {
            element.classList.remove("unread", "unseen");
            element.querySelector("h5").classList.remove("fw-bold");
        }
        updateNotificationBadge(-1);
    } catch (err) {
        console.error("Error marking notification as read", err);
        toastr.error("Failed to mark notification as read");
    }
}

let isDropdownOpen = false;

// Function to mark notifications as seen when the dropdown is expanded
function handleDropdownToggle() {
    const dropdown = document.getElementById("notificationDropdown");
    if (!dropdown) return;

    dropdown.addEventListener("show.bs.dropdown", async () => {
        isDropdownOpen = true;
        const notificationIds = getVisibleNotificationIds();
        if (notificationIds.length > 0) {
            await markNotificationsAsSeen(notificationIds);
        }
    });

    dropdown.addEventListener("hide.bs.dropdown", () => {
        isDropdownOpen = false;
    });
}

// Get IDs of visible notifications
function getVisibleNotificationIds() {
    const notificationItems = document.querySelectorAll(".notification-item");
    const notificationIds = [];
    notificationItems.forEach(item => {
        if (isElementInViewport(item)) {
            const id = item.querySelector(".notification-link")?.getAttribute("data-id");
            if (id) notificationIds.push(id);
        }
    });
    return notificationIds;
}

// Check if an element is in the viewport
function isElementInViewport(el) {
    const rect = el.getBoundingClientRect();
    return (
        rect.top >= 0 &&
        rect.left >= 0 &&
        rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
        rect.right <= (window.innerWidth || document.documentElement.clientWidth)
    );
}

// Mark notifications as seen
async function markNotificationsAsSeen(notificationIds) {
    try {
        const response = await fetch('/Notification/MarkAsSeen', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(notificationIds),
        });

        if (!response.ok) throw new Error("Failed to mark notifications as seen");
        console.log('Notifications marked as seen.');
    } catch (error) {
        console.error('Error marking notifications as seen:', error);
    }
}

// Mark all notifications as read
async function markAllAsRead() {
    try {
        const response = await fetch("/Notification/MarkAllAsRead", {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        });
        if (!response.ok) throw new Error("Failed to mark all as read");
        document.querySelectorAll(".notification-item.unread").forEach(item => {
            item.classList.remove("unread", "unseen");
            item.querySelector("h5").classList.remove("fw-bold");
        });
        updateNotificationBadge(0);
        collapseNotificationDropdown();
    } catch (err) {
        console.error("Error marking all notifications as read", err);
        toastr.error("Failed to mark all notifications as read");
    }
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

    let count = parseInt(badge.textContent) || 0;
    count = Math.max(0, count + delta);
    badge.textContent = count > 9 ? "9+" : count;
    badge.style.display = count > 0 ? "inline-block" : "none";
}

// Generate HTML for a notification
function generateNotificationHTML(data) {
    const relativeTime = getRelativeTime(data.CreatedAt);
    const unreadClass = data.IsRead ? "" : "unread";
    const unseenClass = data.IsSeen ? "" : "unseen";
    return `
        <div class="list-group-item notification-item ${unreadClass} ${unseenClass}">
            <a href="${data.Link}" class="text-decoration-none text-dark" data-id="${data.Id}" onclick="markNotificationAsRead('${data.Id}', this)">
                <div class="card-body">
                    <h5 class="${data.IsRead ? "card-title" : "card-title fw-bold"}">${data.Content}</h5>
                    <p class="card-text text-end mt-1">
                        <small class="text-muted">${relativeTime}</small>
                    </p>
                </div>
            </a>
        </div>
    `;
}

// Add a new notification to the UI
function addNotificationToUI(data) {
    const notificationList = document.getElementById("notificationItems");
    if (!notificationList) return;

    const newNotificationHTML = generateNotificationHTML(data);
    const tempContainer = document.createElement("div");
    tempContainer.innerHTML = newNotificationHTML;
    const newElement = tempContainer.firstElementChild;

    newElement.style.display = "none";
    notificationList.insertAdjacentElement("afterbegin", newElement);
    newElement.style.display = "";
    $(newElement).hide().fadeIn(500);
}

// Fetch notifications for the dropdown
async function fetchNotifications() {
    const notificationList = document.getElementById("notificationItems");
    if (!notificationList) return;

    notificationList.innerHTML = `
        <div class="text-center p-3">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>`;

    try {
        const response = await fetch("/Notification/GetUnreadNotifications?limit=5");
        if (!response.ok) throw new Error("Failed to fetch notifications");
        const result = await response.json();
        var dropdownFooter = document.getElementById('dropdownFooter');
        if (result.UnseenCount == 0){
            notificationList.innerHTML = `<div class="p-3 text-center text-info">No new notifications.</div>`;
            dropdownFooter.style.display = "none";
        }
        else{
            renderNotifications(result);
            dropdownFooter.style.display = "";
        }

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
    });
    notificationList.innerHTML = html;
    updateNotificationBadge(data.UnseenCount);
}

// Initialize the notification system
document.addEventListener("DOMContentLoaded", () => {
    setupSignalRConnection();
    fetchNotifications();
    handleDropdownToggle();
});

// Event delegation for marking notifications as read
document.addEventListener("click", (event) => {
    const link = event.target.closest(".notification-link");
    if (link) {
        const notificationId = link.getAttribute("data-id");
        markNotificationAsRead(notificationId, link.closest(".notification-item"));
    }
});