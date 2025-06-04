const NOTIFICATION_CLASSES = {
    PAGE: {
        ITEM: 'notifications-page-item',
        UNREAD: 'notifications-page-unread',
        UNSEEN: 'notifications-page-unseen',
        CARD: 'notifications-page-card',
        CONTENT: 'notifications-page-content'
    },
    DROPDOWN: {
        ITEM: 'dropdown-notification-item',
        UNREAD: 'dropdown-notification-unread',
        UNSEEN: 'dropdown-notification-unseen'
    }
};

// Helper functions to calculate relative time

// function convertToUTC(date) {
//     return Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
//                      date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
// }

function getRelativeTime(date) {
    if (typeof date !== "string" || !date.trim()) return "Unknown time";
    if (!date.endsWith("Z")) date += "Z";

    date = new Date(date);
    if (!(date instanceof Date) || isNaN(date)) return "Invalid date";

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
    if (days < 30) return days === 1 ? "Tomorrow" : `In ${days} day${days > 1 ? "s" : ""}`;
    if (days < 365) return `In ${Math.floor(days / 30)} month${Math.floor(days / 30) > 1 ? "s" : ""}`;
    return `In ${Math.floor(days / 365)} year${Math.floor(days / 365) > 1 ? "s" : ""}`;
}

function formatPastTime(seconds, minutes, hours, days) {
    if (seconds < 60) return "Just now";
    if (minutes < 60) return `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
    if (hours < 24) return `${hours} hour${hours > 1 ? "s" : ""} ago`;
    if (days < 30) return days === 1 ? "Yesterday" : `${days} day${days > 1 ? "s" : ""} ago`;
    if (days < 365) return `${Math.floor(days / 30)} month${Math.floor(days / 30) > 1 ? "s" : ""} ago`;
    return `${Math.floor(days / 365)} year${Math.floor(days / 365) > 1 ? "s" : ""} ago`;
}

async function ensureSignalRConnection() {
    if (connection.state === signalR.HubConnectionState.Disconnected) {
        try {
            await connection.start();
            // hideConnectionBanner();
            // setTimeout(() => connection.start(), 5000);
            console.log("Reconnected to SignalR Hub");
        } catch (err) {
            console.error("SignalR reconnection failed:", err);
            setTimeout(ensureSignalRConnection, 5000);
        }
    }
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
        addNotificationToUI(data);
        toastr.info("🔔 " + data.Content);
    });

    connection.onclose(() => {
        console.warn("SignalR connection lost. Reconnecting...");
        toastr.warning("Connection lost. Reconnecting...");
        // showConnectionBanner("Disconnected. Reconnecting...");
        ensureSignalRConnection();
    });

    connection.start()
        .then(() => {
            console.log("Connected to SignalR Hub");
            // hideConnectionBanner();
        })
        .catch(err => {
            console.error("SignalR Connection Error:", err);
            // showConnectionBanner("Failed to connect. Retrying...");
            toastr.error("Failed to connect to notifications service. Reconnecting...");
            setTimeout(() => connection.start(), 5000);
        });
}

// // Show connection status banner
// function showConnectionBanner(message) {
//     let banner = document.getElementById("connectionBanner");
//     if (!banner) {
//         banner = document.createElement("div");
//         banner.id = "connectionBanner";
//         banner.className = "alert alert-warning alert-dismissible fade show";
//         banner.innerHTML = `
//             <strong>${message}</strong>
//             <button type="button" class="btn-close" data-dismiss="alert" aria-label="Close"></button>
//             <button class="btn btn-sm btn-primary ms-2" onclick="ensureSignalRConnection()">Retry Now</button>
//         `;
//         document.body.prepend(banner);
//     }
// }

// // Hide connection status banner
// function hideConnectionBanner() {
//     const banner = document.getElementById("connectionBanner");
//     if (banner) banner.remove();
// }

// Mark notifications as seen
async function markNotificationsAsSeen() {
    const notificationIds = Array.from(document.querySelectorAll(`.${NOTIFICATION_CLASSES.DROPDOWN.ITEM}`))
        .map(item => item.getAttribute("data-id"))
        .filter(id => id);

    if (notificationIds.length === 0) return;

    try {
        const response = await fetch(`/Notification/MarkAsSeen`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(notificationIds),
        });

        if (!response.ok) throw new Error("Failed to mark notifications as seen");
        document.querySelectorAll(`.${NOTIFICATION_CLASSES.DROPDOWN.ITEM}`).forEach(item => {
            item.classList.remove(NOTIFICATION_CLASSES.DROPDOWN.UNSEEN);
        });
        updateNotificationBadge(0);
        console.log('Notifications marked as seen.');
    } catch (error) {
        console.error('Error marking notifications as seen:', error);
    }
}

// Mark a single notification as read
async function markNotificationAsRead(id, element) {
    try {
        if (element.closest(`.${NOTIFICATION_CLASSES.DROPDOWN.ITEM}`)?.classList.contains(NOTIFICATION_CLASSES.DROPDOWN.UNREAD) ||
            element.closest(`.${NOTIFICATION_CLASSES.PAGE.ITEM}`)?.classList.contains(NOTIFICATION_CLASSES.PAGE.UNREAD)
        ) {
            const response = await fetch(`/Notification/MarkAsRead?id=${id}`, {
                method: "POST",
                headers: { "Content-Type": "application/json" }
            });
            
            if (!response.ok) throw new Error("Failed to mark as read");
            
            // Update both page and dropdown versions
            document.querySelectorAll(`[data-id="${id}"]`).forEach(notification => {
                const parentItem = notification.closest(`.${NOTIFICATION_CLASSES.DROPDOWN.ITEM}, .${NOTIFICATION_CLASSES.PAGE.ITEM}`);
                parentItem?.classList.remove(
                    NOTIFICATION_CLASSES.PAGE.UNREAD,
                    NOTIFICATION_CLASSES.PAGE.UNSEEN,
                    NOTIFICATION_CLASSES.DROPDOWN.UNREAD,
                    NOTIFICATION_CLASSES.DROPDOWN.UNSEEN
                );
                
                const titleElement = parentItem?.querySelector('h6');
                titleElement?.classList.remove('fw-bold');
            });

            updateNotificationBadge(-1);
        }
    } catch (err) {
        console.error("Error marking notification as read", err);
        toastr.error("Failed to mark notification as read");
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
        
        document.querySelectorAll(`.${NOTIFICATION_CLASSES.PAGE.ITEM}.notifications-page-unread, 
                                    .${NOTIFICATION_CLASSES.DROPDOWN.ITEM}.dropdown-notification-unread`)
            .forEach(item => {
                item.classList.remove(
                    NOTIFICATION_CLASSES.PAGE.UNREAD,
                    NOTIFICATION_CLASSES.PAGE.UNSEEN,
                    NOTIFICATION_CLASSES.DROPDOWN.UNREAD,
                    NOTIFICATION_CLASSES.DROPDOWN.UNSEEN
                );
                
                const titleElement = item.querySelector('h6');
                if (titleElement) {
                    titleElement.classList.remove('fw-bold');
                }
            });
        
        updateNotificationBadge(-Infinity);
        // fetchUnreadNotifications()
        collapseNotificationDropdown();
    } catch (err) {
        console.error("Error marking all notifications as read", err);
        toastr.error("Failed to mark all notifications as read");
    }
}

// Archive a notification
async function archiveNotification(id, element) {
    try {
        const response = await fetch(`/Notification/Archive?id=${id}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        });
        
        if (!response.ok) throw new Error("Failed to archive the notification.");

        document.querySelectorAll(`[data-id="${id}"]`).forEach(notification => {
            notification.closest(`.${NOTIFICATION_CLASSES.DROPDOWN.ITEM}, .${NOTIFICATION_CLASSES.PAGE.ITEM}`)?.remove();
        });

        toastr.success("Notification archived");
        updateNotificationBadge(element.closest(`.${NOTIFICATION_CLASSES.DROPDOWN.ITEM}`) ? -1 : 0);
    } catch (err) {
        console.error("Error archiving notification", err);
        toastr.error("Failed to archive notification");
    }
}

// Collapse the notification dropdown
function collapseNotificationDropdown() {
    const dropdown = document.getElementById("notificationDropdown");
    if (dropdown && dropdown.classList.contains("show")) {
        $(dropdown).dropdown("hide");
    }
    markNotificationsAsSeen();
}

// Update the notification badge count
function updateNotificationBadge(count) {
    const badge = document.getElementById("notificationBadge");
    if (!badge) return;

    badge.classList.add('update');
    setTimeout(() => badge.classList.remove('update'), 300);

    let currentCount = Math.max(0, (parseInt(badge.textContent) || 0) + count);
    if (count === -Infinity) currentCount = 0;
    
    badge.textContent = currentCount > 9 ? "9+" : currentCount;
    badge.style.display = currentCount > 0 ? "inline-block" : "none";
}

// Generate HTML for a notification
function generatePageNotificationHTML(data) {
    const stateClasses = [
        data.IsRead ? '' : NOTIFICATION_CLASSES.PAGE.UNREAD,
        data.IsSeen ? '' : NOTIFICATION_CLASSES.PAGE.UNSEEN
    ].join(' ');

    return `
        <div class="list-group-item ${NOTIFICATION_CLASSES.PAGE.ITEM} ${stateClasses}">
            <a href="${data.Link}" class="notifications-page-link text-dark" data-id="${data.Id}" onclick="markNotificationAsRead('${data.Id}', this)">
                <div class="${NOTIFICATION_CLASSES.PAGE.CARD}">
                    <h6 class="${NOTIFICATION_CLASSES.PAGE.CONTENT}">${data.Content}</h6>
                    <p class="notifications-page-time card-text text-end mt-1">
                        <small class="text-muted">${getRelativeTime(data.CreatedAt)}</small>
                    </p>
                </div>
            </a>
            <button class="btn btn-sm btn-outline-danger notifications-page-archive-btn" data-id="${data.Id}" onclick="archiveNotification('${data.Id}', this)">
                Archive
            </button>
        </div>
    `;
    // return `
    //     <div class="list-group-item ${NOTIFICATION_CLASSES.PAGE.ITEM} ${stateClasses}">
    //         <a href="#" data-link="${notification.Link}" data-id="${data.Id}" class="notifications-page-link text-dark" onclick="markNotificationAsRead('${data.Id}', this)">
    //             <div class="${NOTIFICATION_CLASSES.PAGE.CARD} notification-link">
    //                 <span class="notification-type">${data.Type}</span>
    //                 <h6 class="${NOTIFICATION_CLASSES.PAGE.CONTENT}">${data.Content}</h6>
    //                 <p class="notifications-page-time card-text text-end mt-1">
    //                     <small class="text-muted">${getRelativeTime(data.CreatedAt)}</small>
    //                 </p>
    //             </div>
    //         </a>
    //         <button class="btn btn-sm btn-outline-danger notifications-page-archive-btn" data-id="${data.Id}" onclick="archiveNotification('${data.Id}', this)">
    //             Archive
    //         </button>
    //     </div>
    // `;
}

function generateDropdownNotificationHTML(data) {
    const stateClasses = [
        data.IsRead ? '' : NOTIFICATION_CLASSES.DROPDOWN.UNREAD,
        data.IsSeen ? '' : NOTIFICATION_CLASSES.DROPDOWN.UNSEEN
    ].join(' ');

    return `
            <a href="#" data-link="${data.Link}" data-id="${data.Id}"
                class="notification-link text-decoration-none text-dark list-group-item list-group-item-action ${NOTIFICATION_CLASSES.DROPDOWN.ITEM} ${stateClasses}">
                <div class="notification-content">
                    <span class="notification-type">${data.Type}</span>
                    <h6 class="notification-preview dropdown-notification-content mb-1">${data.Content}</h6>
                    <p class="dropdown-notification-time text-end mt-1">
                        <small class="text-muted">${getRelativeTime(data.CreatedAt)}</small>
                    </p>
                </div>
            </a>
        `;

    // onclick="markNotificationAsRead('${data.Id}', this)">
    // return `
    //         <a href="#" data-link="${data.Link}" data-id="${data.Id}"
    //             class="notification-link text-decoration-none text-dark list-group-item list-group-item-action ${NOTIFICATION_CLASSES.DROPDOWN.ITEM} ${stateClasses}">
    //             <div class="notification-content">
    //                 <span class="notification-type">${data.Type}</span>
    //                 <h6 class="notification-preview dropdown-notification-content mb-1">${data.Content}</h6>
    //                 <p class="dropdown-notification-time text-end mt-1">
    //                     <small class="text-muted">${getRelativeTime(data.CreatedAt)}</small>
    //                 </p>
    //             </div>
    //         </a>
    //     `;
}

function addNotificationToUI(data) {
    // Handle Dropdown Notifications
    const dropdownList = document.getElementById('notificationItems');
    if (dropdownList) {
        // Remove empty state message
        const emptyState = dropdownList.querySelector('.no-notifications-message');
        if (emptyState) emptyState.remove();
    
        const dropdownHTML = generateDropdownNotificationHTML(data);
        const dropdownTemp = document.createElement('div');
        dropdownTemp.innerHTML = dropdownHTML;
        
        // Insert new dropdown notification
        let firstElementChild = dropdownTemp.firstElementChild;
        while (firstElementChild) {
            dropdownList.insertAdjacentElement("afterbegin", firstElementChild);
            $(firstElementChild).hide().fadeIn(500);
            firstElementChild = dropdownTemp.firstElementChild;
        }


        // Maintain dropdown notification limit
        const dropdownNotifications = dropdownList.querySelectorAll(`.${NOTIFICATION_CLASSES.DROPDOWN.ITEM}`);
        if (dropdownNotifications.length > 5) {
            dropdownNotifications[dropdownNotifications.length - 1].remove();
        }

        // Show mark all button if notifications exist
        const markAllButton = document.getElementById("markAll");
        if (markAllButton) markAllButton.style.display = "flex";
    }

    // Handle Page Notifications
    const pageList = document.getElementById('notificationsContainer');
    if (pageList) {

        const emptyState = pageList.querySelector('.notifications-page-empty');
        if (emptyState) emptyState.remove();

        const pageHTML = generatePageNotificationHTML(data);
        const pageTemp = document.createElement('div');
        pageTemp.innerHTML = pageHTML;
        
        // Insert new page notification
        let firstElementChild = pageTemp.firstElementChild;
        while (firstElementChild) {
            pageList.insertAdjacentElement("afterbegin", firstElementChild);
            $(firstElementChild).hide().fadeIn(500);
            firstElementChild = pageTemp.firstElementChild;
        }

        // Maintain page notification limit
        const pageNotifications = pageList.querySelectorAll(`.${NOTIFICATION_CLASSES.PAGE.ITEM}`);
        if (pageNotifications.length > 10) {
            pageNotifications[pageNotifications.length - 1].remove();
        }
    }

    // Update badge count
    updateNotificationBadge(1);
}

// Fetch notifications for the dropdown
async function fetchUnreadNotifications() {
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
        
        document.getElementById("markAll").style.display = result.Notifications.length > 0 ? "" : "none";
        notificationList.innerHTML = result.Notifications.length > 0 ? result.Notifications.map(generateDropdownNotificationHTML).join("") : `<div class="p-3 text-center text-info">No new notifications.</div>`;
        updateNotificationBadge(result.UnseenCount);

    } catch (error) {
        console.error("Error fetching notifications:", error);
        notificationList.innerHTML = `<div class="p-3 text-center text-danger">Failed to load notifications.</div>`;
    }
}

// Function to mark notifications as seen when the dropdown is expanded
function handleDropdownToggle() {
    const dropdown = document.getElementById("notificationDropdown");
    if (!dropdown) return;

    // dropdown.addEventListener("show.bs.dropdown", async () => { await markNotificationsAsSeen(); });
    // const bsDropdown = new bootstrap.Dropdown(dropdown);
    // document.addEventListener("click", (event) => { if (!dropdown.contains(event.target)) { bsDropdown.hide(); } });

    let hoverTimeout;
    const menu = document.querySelector('.dropdown-notifications-menu');
    menu.addEventListener('mouseleave', () => {
        hoverTimeout = setTimeout(() => { bootstrap.Dropdown.getInstance(dropdown)?.hide(); }, 300);
    });
    menu.addEventListener('mouseenter', () => { clearTimeout(hoverTimeout); });
    dropdown.addEventListener('shown.bs.dropdown', () => { menu.style.pointerEvents = 'auto'; });
    dropdown.addEventListener('hide.bs.dropdown', () => { markNotificationsAsSeen(); });
}

// Initialize the notification system
document.addEventListener("DOMContentLoaded", () => {
    setupSignalRConnection();
    handleDropdownToggle();
    fetchUnreadNotifications();
});

// Event delegation for marking notifications as read
document.addEventListener("click", (event) => {
    const link = event.target.closest(".notification-link");
    if (link) {
        const notificationId = link.getAttribute("data-id");
        markNotificationAsRead(notificationId, link.closest(NOTIFICATION_CLASSES.DROPDOWN.ITEM));
    }
});
