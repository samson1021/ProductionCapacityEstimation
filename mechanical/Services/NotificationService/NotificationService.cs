﻿using AutoMapper;
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
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId)
                .ConfigureAwait(false);
            return _mapper.Map<NotificationReturnDto>(notification);
        }
        
        public async Task<NotificationResultDto> GetNotifications(Guid userId, bool includeRead = false, string mode = "active", int page = 1, int pageSize = 10)
        {
            var notificationsQuery = _cbeContext.Notifications.Where(n => n.UserId == userId &&
                                                                        (includeRead || !n.IsRead) &&
                                                                        (mode == "all" ||
                                                                            (mode == "active" && !n.IsArchived) ||
                                                                            (mode == "archived" && n.IsArchived))
                                                                    ).OrderByDescending(n => n.CreatedAt);

            int totalCount = await notificationsQuery.CountAsync().ConfigureAwait(false);
            var notifications = await notificationsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);

            int unreadCount = await GetUnreadCount(userId).ConfigureAwait(false);
            int unseenCount = await GetUnseenCount(userId).ConfigureAwait(false);
                
            return new NotificationResultDto {
                Notifications = notifications,
                TotalCount = totalCount,
                UnreadCount = unreadCount,
                UnseenCount = unseenCount
            };
        }

        // Single notification
        public async Task<NotificationReturnDto> AddNotification(Guid userId, string content, string type, string link = "")
        {
            try
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Content = content,
                    Type = type,
                    Link = link,
                    IsRead = false,
                    IsSeen = false,
                    IsArchived = false,
                    CreatedAt = DateTime.UtcNow
                };

                _cbeContext.Notifications.AddAsync(notification);
                // await _cbeContext.Notifications.AddAsync(notification).ConfigureAwait(false);
                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                
                return _mapper.Map<NotificationReturnDto>(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
                throw;
            }
        }


        // Batch notifications for multiple users.
        public async Task<IEnumerable<NotificationReturnDto>> AddNotifications(IEnumerable<(Guid userId, string content, string type, string link)> notificationsBatch)
        {
            var notifications = notificationsBatch.Select(n => new Notification
            {
                Id = Guid.NewGuid(),
                UserId = n.userId,
                Content = n.content,
                Type = n.type,
                Link = n.link,
                IsRead = false,
                IsSeen = false,
                IsArchived = false,
                CreatedAt = DateTime.UtcNow

            }).ToList();

            await _cbeContext.Notifications.AddRangeAsync(notifications).ConfigureAwait(false);
            await _cbeContext.SaveChangesAsync().ConfigureAwait(false);

            return _mapper.Map<IEnumerable<NotificationReturnDto>>(notifications);
        }


        public async Task<bool> MarkAsRead(Guid userId, Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                return false;
            }
            
            await _cbeContext.Notifications
                            .Where(n => n.UserId == userId && notificationId == n.Id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(n => n.IsRead, true)
                                .SetProperty(n => n.IsSeen, true)
                            )
                            .ConfigureAwait(false);

            // var notification = await _cbeContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId).ConfigureAwait(false);

            // if (notification == null) return false;

            // notification.IsRead = true;
            // notification.IsSeen = true;

            await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task MarkAllAsRead(Guid userId)
        {
            await _cbeContext.Notifications
                            .Where(n => n.UserId == userId)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(n => n.IsRead, true)
                                .SetProperty(n => n.IsSeen, true)
                            )
                            .ConfigureAwait(false);

            await _cbeContext.SaveChangesAsync().ConfigureAwait(false);

            // var notifications = await _cbeContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync().ConfigureAwait(false);
            // if (notifications.Any())
            // {
            //     foreach (var notification in notifications)
            //     {
            //         notification.IsRead = true;
            //         notification.IsSeen = true;
            //     }
            //     await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
            // }
        }

        // Mark notifications as seen
        public async Task MarkAsSeen(Guid userId, List<Guid> notificationIds)
        {
            if (notificationIds == null || !notificationIds.Any())
            {
                return; // No notifications to update
            }

            await _cbeContext.Notifications
                .Where(n => n.UserId == userId && notificationIds.Contains(n.Id))
                .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsSeen, true))
                .ConfigureAwait(false);
                
            // var notifications = await _cbeContext.Notifications.Where(n => n.UserId == userId && notificationIds.Contains(n.Id)).ToListAsync().ConfigureAwait(false);

            // foreach (var notification in notifications)
            // {
            //     notification.IsSeen = true;
            // }

            await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task MarkAllAsSeen(Guid userId)
        {
            
            await _cbeContext.Notifications
                            .Where(n => n.UserId == userId && !n.IsSeen)
                            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsSeen, true))
                            .ConfigureAwait(false);

            await _cbeContext.SaveChangesAsync().ConfigureAwait(false);

            // var notifications = await _cbeContext.Notifications.Where(n => n.UserId == userId && !n.IsSeen).ToListAsync().ConfigureAwait(false);
            // if (notifications.Any())
            // {
            //     foreach (var notification in notifications)
            //     {
            //         notification.IsSeen = true;
            //     }
            //     await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
            // }
        }

        public async Task Archive(Guid userId, Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                return;
            }

            await _cbeContext.Notifications
                            .Where(n => n.UserId == userId && notificationId == n.Id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(n => n.IsSeen, true)
                                .SetProperty(n => n.IsArchived, true)
                            )
                            .ConfigureAwait(false);

            //     var notification = await _cbeContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId).ConfigureAwait(false);
            //     if (notification != null)
            //     {
            //         notification.IsRead = true;
            //         notification.IsSeen = true;
            //     }
            //     await _cbeContext.SaveChangesAsync().ConfigureAwait(false);

            await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> GetUnreadCount(Guid userId)
        {
            return await _cbeContext.Notifications.Where(n => n.UserId == userId && !n.IsRead && !n.IsArchived).CountAsync().ConfigureAwait(false);
        }
        public async Task<int> GetUnseenCount(Guid userId)
        {
            return await _cbeContext.Notifications.Where(n => n.UserId == userId && !n.IsSeen).CountAsync().ConfigureAwait(false);
        }

        public async Task SendNotification(NotificationReturnDto notification)
        {
            await _notificationHub.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification).ConfigureAwait(false);
        }

        public async Task SendNotifications(IEnumerable<NotificationReturnDto> notifications)
        {
            var notificationTasks = notifications.Select(notification =>
                _notificationHub.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification)
            );
            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
        }
        
        public async Task UnicastNotification(Guid userId, string notification)
        {
            await _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification).ConfigureAwait(false);
        }
        
        public async Task BroadcastNotification(string notification)
        {
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", notification).ConfigureAwait(false);
        }

        public async Task MulticastNotification(IEnumerable<Guid> userIds, string notification)
        {
            var notificationTasks = userIds.Select(userId =>
                _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification)
            );
            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
        }

        public async Task UnicastNotifications(Guid userId, IEnumerable<string> notifications)
        {
            var notificationTasks = notifications.Select(notification =>
                _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification)
            );
            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
        }

        public async Task BroadcastNotifications(IEnumerable<string> notifications)
        {
            var notificationTasks = notifications.Select(notification =>
                _notificationHub.Clients.All.SendAsync("ReceiveNotification", notification)
            );
            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
        }

        public async Task MulticastNotifications(IEnumerable<Guid> userIds, IEnumerable<string> notifications)
        {
            var notificationTasks = notifications.SelectMany(notification =>
                userIds.Select(userId =>
                    _notificationHub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification)
                )
            );

            await Task.WhenAll(notificationTasks).ConfigureAwait(false);
        }
    }
}
