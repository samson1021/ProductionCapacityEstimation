using Microsoft.AspNetCore.Mvc;
using mechanical.Models.Entities;
using mechanical.Models.Dto.NotificationDto;

namespace mechanical.Services.NotificationService
{
    public interface INotificationService
    {
        Task<NotificationReturnDto> GetNotification(Guid userId, Guid notificationId);
        Task<NotificationResultDto> GetNotifications(Guid userId, int page = 1, int pageSize = 10);
        Task<IEnumerable<NotificationReturnDto>> GetUnreadNotifications(Guid userId, int page = 1, int pageSize = 10);
        
        Task MarkAsRead(Guid userId, Guid Id);
        Task MarkAllAsRead(Guid userId);
        Task<int> GetUnreadCount(Guid userId);

        Task<NotificationReturnDto> SendNotification(Guid userId, string message, string type, string link = "");
        Task<IEnumerable<NotificationReturnDto>> SendNotifications(IEnumerable<Guid> userIds, string message, string type, string link = "");
        Task SendRealTimeNotification(NotificationReturnDto notification);
        Task SendRealTimeNotifications(IEnumerable<NotificationReturnDto> notifications);

        Task UnicastNotification(Guid userId, string notification);
        Task MulticastNotification(IEnumerable<Guid> userIds, string notification);
        Task BroadcastNotification(string notification);
        Task UnicastNotifications(Guid userId, IEnumerable<string> notifications);
        Task MulticastNotifications(IEnumerable<Guid> userIds, IEnumerable<string> notifications);
        Task BroadcastNotifications(IEnumerable<string> notifications);

    }
}
