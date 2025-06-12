using OpenCvSharp.CPlusPlus;
using Microsoft.AspNetCore.Mvc;

using mechanical.Services.UserService;

namespace mechanical.Controllers
{
    public class BaseController : Controller
    {
        protected Guid GetCurrentUserId()
        {
            var httpContext = HttpContext;
            if (httpContext != null)
            {
                var userIdString = httpContext.Session.GetString("userId");
                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId) && userId != Guid.Empty)
                {
                    return userId;
                }
            }
            // RedirectToAction("Index", "Home");
            return Guid.Empty;
        }
    }
}
