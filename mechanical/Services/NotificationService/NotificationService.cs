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
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;      
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<NotificationService> _logger;      

        public NotificationService(CbeContext cbeContext, IHubContext<NotificationHub> hubContext, IMapper mapper, ILogger<NotificationService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _hubContext = hubContext;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }
        
        public async Task<NotificationReturnDto> GetNotification(Guid userId, Guid notificationId)
        {
                
                var notification = _cbeContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
                return _mapper.Map<NotificationReturnDto>(notification);
        }
        public async Task<IEnumerable<NotificationReturnDto>> GetNotifications(Guid userId)
        {
                var notifications = _cbeContext.Notifications
                            .Where(n => n.UserId == userId)
                            .OrderByDescending(n => n.CreatedAt)
                            .ToList();

                return _mapper.Map<IEnumerable<NotificationReturnDto>>(notifications);
        }
        public async Task<IEnumerable<NotificationReturnDto>> GetUnreadNotifications(Guid userId)
        {
            
                var notifications = _cbeContext.Notifications
                            .Where(n => n.UserId == userId && !n.IsRead)
                            .OrderByDescending(n => n.CreatedAt)
                            .ToList();
                return _mapper.Map<IEnumerable<NotificationReturnDto>>(notifications);
            
        }
        
        public async Task SendNotification(Guid userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.Now,
                Status = "New",
                IsRead = false
            };

            _cbeContext.Notifications.Add(notification);
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}