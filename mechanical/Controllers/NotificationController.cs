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
        public async Task<IActionResult> Index()
        {
            // // var userId = User.Identity.GetUserId();
            // var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (!Guid.TryParse(userIdStr, out var userId))
            //     return Unauthorized();

            // var result = await _notificationService.GetNotifications(userId, includeRead: true, page: page, pageSize: pageSize);
            
            // var viewModel = new NotificationViewModel
            // {
            //     Notifications = result.Notifications,
            //     CurrentPage = page,
            //     TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
            // };

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            return View();
        }

        // [HttpGet("api/Notification/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetNotification(Guid id)
        {
            var notification = await _notificationService.GetNotification(base.GetCurrentUserId(), id);
            return Ok(notification);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(int page = 1, int pageSize = 10, string mode = "active")
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _notificationService.GetNotifications(userId, includeRead: true, mode: mode, page: page, pageSize: pageSize);

            var response = new NotificationResponseDto
            {
                Notifications = result.Notifications,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadNotifications(int limit = 5)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _notificationService.GetNotifications(userId, includeRead: false, pageSize: limit);
            return Ok(result);
            // return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            int unreadCount = await _notificationService.GetUnreadCount(userId);
            return Ok(new { unreadCount });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnseenCount()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            int unreadCount = await _notificationService.GetUnseenCount(userId);
            return Ok(new { unreadCount });
        }
        // [HttpPut("api/Notification/{id}/mark-read")]
        // [HttpPost("MarkAsRead")]
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            if (id == Guid.Empty)
            {
                return BadRequest("No notification ID provided.");
            }

            var result = await _notificationService.MarkAsRead(userId, id);
            return Ok(new { success = result });
            // return result ? NoContent() : NotFound();
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
        
        // [HttpPost("MarkAllAsRead")]
        [HttpPost]
        public async Task<IActionResult> MarkAllAsSeen()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            await _notificationService.MarkAllAsSeen(userId);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> MarkAsSeen(List<Guid> ids)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();
                
            if (ids == null || !ids.Any())
            {
                return BadRequest("No notification IDs provided.");
            }
            await _notificationService.MarkAsSeen(userId, ids);
            return Ok(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> Archive(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            if (id == Guid.Empty)
            {
                return BadRequest("No notification ID provided.");
            }

            await _notificationService.Archive(userId, id);
            return Ok(new { success = true });
        }
    }
}
