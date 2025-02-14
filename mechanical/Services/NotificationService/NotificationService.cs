using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Hubs;
using mechanical.Utils;
using mechanical.Models.Entities;
using mechanical.Models.Dto.NotificationDto;
using mechanical.Services.CaseTimeLineService;

namespace mechanical.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<NotificationService> _logger;
        private readonly NotificationHub _notificationHub;

        public NotificationService(CbeContext cbeContext, NotificationHub notificationHub, IMapper mapper, ILogger<NotificationService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _notificationHub = notificationHub;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }
        
        public async Task<NotificationReturnDto> GetNotification(Guid userId, Guid notificationId)
        {
                
                var notification = await _cbeContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
                return _mapper.Map<NotificationReturnDto>(notification);
        }
        
        public async Task<IEnumerable<NotificationReturnDto>> GetNotifications(Guid userId, int page = 1, int pageSize = 10)
        {
            var notifications = await _cbeContext.Notifications
                                                .Where(n => n.UserId == userId)
                                                .OrderByDescending(n => n.CreatedAt)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();

            return _mapper.Map<IEnumerable<NotificationReturnDto>>(notifications);
        }

        public async Task<IEnumerable<NotificationReturnDto>> GetUnreadNotifications(Guid userId, int page = 1, int pageSize = 10)
        {
            var notifications = await _cbeContext.Notifications
                                                .Where(n => n.UserId == userId && !n.IsRead)
                                                .OrderByDescending(n => n.CreatedAt)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToListAsync();

            return _mapper.Map<IEnumerable<NotificationReturnDto>>(notifications);
        }

        // Batch notifications for multiple users
        public async Task SendNotifications(IEnumerable<Guid> userIds, string message, string type, string link = "")
        {
            var notifications = userIds.Select(userId => new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = message,
                Type = type,
                Link = link,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            }).ToList();

            _cbeContext.Notifications.AddRange(notifications);
            await _cbeContext.SaveChangesAsync();

            foreach (var userId in userIds)
            {
                await _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new {
                        message, link, id = notifications.First(n => n.UserId == userId).Id});
            }
        }

        // Single notification
        public async Task SendNotification(Guid userId, string message, string type, string link = "")
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = message,
                Type = type,
                Link = link,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _cbeContext.Notifications.Add(notification);
            await _cbeContext.SaveChangesAsync();

            await _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new {
                    message, link, id = notification.Id });
        }
        
        public async Task MarkAsRead(Guid userId, Guid Id)
        {
            var notification = await _cbeContext.Notifications.FindAsync(Id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _cbeContext.SaveChangesAsync();
            }
        }
    }
}