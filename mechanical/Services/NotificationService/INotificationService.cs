using Microsoft.AspNetCore.Mvc;
using mechanical.Models.Entities;
using mechanical.Models.Dto.NotificationDto;

namespace mechanical.Services.NotificationService
{
    public interface INotificationService
    {
        Task<NotificationReturnDto> GetNotification(Guid userId, Guid notificationId);
        Task<NotificationResultDto> GetNotifications(Guid userId, bool includeRead = false, string mode = "active", int page = 1, int pageSize = 10);
        Task MarkAsRead(Guid userId, Guid notificationId);
        Task MarkAllAsRead(Guid userId);
        Task MarkAsSeen(Guid userId, List<Guid> notificationIds);
        Task MarkAllAsSeen(Guid userId);
        Task Archive(Guid userId, Guid notificationId);
        Task<int> GetUnreadCount(Guid userId);
        Task<int> GetUnseenCount(Guid userId);

        Task<NotificationReturnDto> AddNotification(Guid userId, string content, string type, string link = "");
        Task<IEnumerable<NotificationReturnDto>> AddNotifications(IEnumerable<(Guid userId, string content, string type, string link)> notificationsBatch);
        Task SendNotification(NotificationReturnDto notification);
        Task SendNotifications(IEnumerable<NotificationReturnDto> notifications);

        Task UnicastNotification(Guid userId, string notification);
        Task MulticastNotification(IEnumerable<Guid> userIds, string notification);
        Task BroadcastNotification(string notification);
        Task UnicastNotifications(Guid userId, IEnumerable<string> notifications);
        Task MulticastNotifications(IEnumerable<Guid> userIds, IEnumerable<string> notifications);
        Task BroadcastNotifications(IEnumerable<string> notifications);

    }
}
