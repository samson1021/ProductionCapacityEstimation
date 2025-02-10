using Microsoft.AspNetCore.SignalR;
using mechanical.Data;
using mechanical.Models.Entities;

namespace mechanical.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly CbeContext _context;

        public NotificationHub(CbeContext context)
        {
            _context = context;
        }

        public async Task SendNotification(Guid userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Status = "New",
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
        public async Task MarkAsRead(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}