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
        Task SendNotification(Guid userId, string message, string type, string link = "");
        Task SendNotifications(IEnumerable<Guid> userIds, string message, string type, string link = "");
        Task MarkAsRead(Guid userId, Guid Id);
        Task MarkAllAsRead(Guid userId);
        Task<int> GetUnreadCount(Guid userId);
    }
}
