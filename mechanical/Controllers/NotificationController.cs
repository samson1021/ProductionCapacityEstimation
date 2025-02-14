using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using mechanical.Controllers;
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
        public async Task<ActionResult<IEnumerable<NotificationReturnDto>>> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var guid))
                return Unauthorized();

            var notifications = await _notificationService.GetNotifications(guid);
            return Ok(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications1()
        {
            var notifications = await _notificationService.GetNotifications(base.GetCurrentUserId());
            return Json(notifications);
        }

        [HttpPost("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var guid))
                return Unauthorized();

            await _notificationService.MarkAsRead(guid, id);
            return Ok();
        }
    }
}
