using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using mechanical.Data;
using mechanical.Hubs;
using mechanical.Models.Entities;
using mechanical.Models.Dto.NotificationDto;

namespace mechanical.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(CbeContext cbeContext, IHubContext<NotificationHub> notificationHub, IMapper mapper, ILogger<NotificationService> logger)
        {
            _cbeContext = cbeContext;
            _notificationHub = notificationHub;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<NotificationReturnDto> GetNotification(Guid userId, Guid notificationId)
        {
            var notification = await _cbeContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            return _mapper.Map<NotificationReturnDto>(notification);
        }
        
        public async Task<NotificationResultDto> GetNotifications(Guid userId, int page = 1, int pageSize = 10)
        {
            var notificationsQuery = _cbeContext.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt);
            
            int totalCount = await notificationsQuery.CountAsync();
            int unreadCount = await notificationsQuery.Where(n => !n.IsRead).CountAsync();
            var notifications = await notificationsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                
            return new NotificationResultDto {
                Notifications = notifications,
                UnreadCount = unreadCount,
                TotalCount = totalCount
            };
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

        // Single notification
        public async Task<NotificationReturnDto> SendNotification(Guid userId, string message, string type, string link = "")
        {
            try
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

                _cbeContext.Notifications.AddAsync(notification);
                await _cbeContext.SaveChangesAsync();
                
                return _mapper.Map<NotificationReturnDto>(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
                throw;
            }
        }

        // Batch notifications for multiple users.
        public async Task<IEnumerable<NotificationReturnDto>> SendNotifications(IEnumerable<Guid> userIds, string message, string type, string link = "")
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

            _cbeContext.Notifications.AddRangeAsync(notifications);
            await _cbeContext.SaveChangesAsync();

            return _mapper.Map<IEnumerable<NotificationReturnDto>>(notifications);
        }
        
        public async Task MarkAsRead(Guid userId, Guid notificationId)
        {
            var notification = await _cbeContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _cbeContext.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsRead(Guid userId)
        {
            var notifications = await _cbeContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }
                await _cbeContext.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCount(Guid userId)
        {
            return await _cbeContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).CountAsync();
        }

        
        public async Task SendRealTimeNotification(NotificationReturnDto notification)
        {
            await _notificationHub.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification);
        }

        public async Task SendRealTimeNotifications(IEnumerable<NotificationReturnDto> notifications)
        {
            var notificationTasks = notifications.Select(notification =>
                _notificationHub.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification)
            );
            await Task.WhenAll(notificationTasks);
        }

        
        public async Task UnicastNotification(Guid userId, string notification)
        {
            await _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification);
        }
        
        public async Task BroadcastNotification(string notification)
        {
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", notification);
        }

        public async Task MulticastNotification(IEnumerable<Guid> userIds, string notification)
        {
            var notificationTasks = userIds.Select(userId =>
                _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification)
            );
            await Task.WhenAll(notificationTasks);
        }        

        public async Task UnicastNotifications(Guid userId, IEnumerable<string> notifications)
        {
            var notificationTasks = notifications.Select(notification =>
                _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification)
            );
            await Task.WhenAll(notificationTasks);
        }

        public async Task BroadcastNotifications(IEnumerable<string> notifications)
        {
            var notificationTasks = notifications.Select(notification =>
                _notificationHub.Clients.All.SendAsync("ReceiveNotification", notification)
            );
            await Task.WhenAll(notificationTasks);
        }

        public async Task MulticastNotifications(IEnumerable<Guid> userIds, IEnumerable<string> notifications)
        {
            var notificationTasks = notifications.SelectMany(notification =>
                userIds.Select(userId =>
                    _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification)
                )
            );

            await Task.WhenAll(notificationTasks);
        }
    }
}
