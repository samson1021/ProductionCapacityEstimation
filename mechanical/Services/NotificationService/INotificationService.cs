using Microsoft.AspNetCore.Mvc;
using mechanical.Models.Entities;
using mechanical.Models.Dto.NotificationDto;

namespace mechanical.Services.NotificationService
{
    public interface INotificationService
    {
        Task<NotificationReturnDto> GetNotification(Guid userId, Guid notificationId);
        Task<IEnumerable<NotificationReturnDto>> GetNotifications(Guid userId);
        Task<IEnumerable<NotificationReturnDto>> GetUnreadNotifications(Guid userId);
    }
}
