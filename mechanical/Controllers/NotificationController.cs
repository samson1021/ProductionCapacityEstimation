using Newtonsoft.Json;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using mechanical.Controllers;
using mechanical.Models.ViewModels;
using mechanical.Models.Dto.NotificationDto;
using mechanical.Services.NotificationService;

// [ApiController]
// [Route("api/Notification")]

namespace mechanical.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;
        
        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _notificationService.GetNotifications(userId, page, pageSize);
            
            var viewModel = new NotificationViewModel
            {
                Notifications = result.Notifications,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Notifications(Guid id)
        {
            var notification = await _notificationService.GetNotification(base.GetCurrentUserId(), id);
            return View();
        }

        public async Task<IActionResult> GetNotification(Guid id)
        {
            var notification = await _notificationService.GetNotification(base.GetCurrentUserId(), id);
            return Json(notification);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var notification = await _notificationService.GetNotification(base.GetCurrentUserId(), id);
                if (notification == null)
                {
                    return NotFound();
                }

                return PartialView("_NotificationDetailsPartial", notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching notification details.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(int limit = 5)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _notificationService.GetNotifications(userId, pageSize: limit);
            return Json(result);
            // return Ok(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            int unreadCount = await _notificationService.GetUnreadCount(userId);
            return Json(new { unreadCount });
        }

        // [HttpPost("MarkAsRead")]
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            await _notificationService.MarkAsRead(userId, id);
            return Ok();
        }
        
        // [HttpPost("MarkAllAsRead")]
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            await _notificationService.MarkAllAsRead(userId);
            return Ok();
        }
    }
}
