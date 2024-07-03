using mechanical.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp.CPlusPlus;

namespace mechanical.Controllers
{
    public class BaseController : Controller
    {


        protected Guid GetCurrentUserId()
        {
            var httpContext = HttpContext;
            if (httpContext != null)
            {
                var userId = Guid.Parse(httpContext.Session.GetString("userId") ?? Guid.Empty.ToString());
                if (userId != Guid.Empty)
                {

                    return userId;

                }
            }
            RedirectToAction("Login", "Account");
            return Guid.Empty;
        }
    }
}
