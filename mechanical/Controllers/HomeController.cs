using System.Text;
using System.Net.Http;
using System.Diagnostics;
using System.Security.Claims;
using NuGet.Protocol.Plugins;
using System.DirectoryServices.AccountManagement;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models.Entities;
using mechanical.Models.Dto.UserDto;
using mechanical.Models.Login;
using mechanical.Services.AuthenticatioinService;

namespace mechanical.Controllers
{
    public class HomeController : Controller
    {
        private readonly CbeContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private mechanical.Services.AuthenticatioinService.IAuthenticationService _authetnicationService;

        public HomeController(CbeContext context, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, mechanical.Services.AuthenticatioinService.IAuthenticationService authetnicationService)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _authetnicationService = authetnicationService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (HttpContext.Session.GetString("userId") != null)
                {
                    var loggedUser = _context.Users.Include(c => c.Role).Include(c=>c.District).FirstOrDefault(u => u.Id == Guid.Parse(HttpContext.Session.GetString("userId")));

                    if (loggedUser != null)
                    {
                        return RedirectToDashboard(loggedUser);
                    }
                    return RedirectToAction("Logout");
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(loginDto logins)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                if (HttpContext.Session.GetString("userId") != null)
                {
                    var loggedUser = await _context.Users.Include(c => c.Role).Include(c => c.District).FirstOrDefaultAsync(u => u.Id == Guid.Parse(HttpContext.Session.GetString("userId")));

                    if (loggedUser != null)
                    {
                        return RedirectToDashboard(loggedUser);
                    }

                    return RedirectToAction("Logout");
                }
            }

            if (logins.Email == null || logins.Password == null)
            {
                if (logins.Password == null)
                {
                    ModelState.AddModelError(nameof(logins.Password), "The Password field is required.");
                }
                return View("Index", logins);
            }

            var user = await _context.Users.Include(c => c.Role).Include(c => c.District).Where(c => c.Email.ToUpper() == logins.Email.ToUpper() || c.emp_ID == logins.Email).FirstOrDefaultAsync();

            if (user == null)
            {
                ViewData["Error"] = "You do not have the necessary permissions to use the system.";
                return View("Index", logins);
            }
            if (logins.Password == "1234")
            {
                string userRole = user.Role.Name;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, userRole),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                byte[] userId = Encoding.UTF8.GetBytes(user.Id.ToString());
                _httpContextAccessor.HttpContext.Session.Set("UserId", userId);
                HttpContext.Session.SetString("userRole", userRole);
                HttpContext.Session.SetString("userName", user.Name);
                HttpContext.Session.SetString("userId", user.Id.ToString());
                HttpContext.Session.SetString("EmployeeId", user.emp_ID.ToString());

                if (userRole == "Relation Manager")
                {
                    HttpContext.Session.SetString("unit", user.Unit.ToString());
                    HttpContext.Session.SetString("segment", user.BroadSegment.ToString());
                    HttpContext.Session.SetString("district", user.District.Name.ToString());
                }

                var viewbagg = ViewBag.UserRole;
                if (HttpContext.Session.TryGetValue("ExpirationTime", out var expirationTimeBytes))
                {
                    var expirationTime = DateTime.FromBinary(BitConverter.ToInt64(expirationTimeBytes, 0));
                    if (expirationTime < DateTime.UtcNow)
                    {
                        throw new SessionTimeoutException("Session has expired.");
                    }
                }
                return RedirectToDashboard(user);
            }
            else
            {
                ViewData["Error"] = "Incorrect username or password.";
                return View("Index", logins);
            }
        }

        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");

            // await HttpContext.SignOutAsync();
            // return Ok();
        }

        public IActionResult RedirectToDashboard(User user)
        {
            if (user != null)
            {
                switch (user.Role.Name)
                {
                    case "Relation Manager":
                        return RedirectToAction("RM", "Dashboard", user.Role.Name);
                    case "Maker Officer":
                        return RedirectToAction("MO", "Dashboard", user.Role.Name);
                    case "Maker Manager":
                        ViewData["Center"] = user.District.Name;
                        return RedirectToAction("MM", "Dashboard", user.Role.Name);
                    case "Maker TeamLeader":
                        return RedirectToAction("MTL", "Dashboard", user.Role.Name);
                    case "Admin":
                        return RedirectToAction("Index", "UserManagment");
                    case "Checker TeamLeader":
                        return RedirectToAction("CTL", "Dashboard", user.Role.Name);
                    case "Checker Manager":
                        ViewData["Center"] = user.District.Name;
                        return RedirectToAction("CM", "Dashboard", user.Role.Name);
                    case "Checker Officer":
                        return RedirectToAction("CO", "Dashboard", user.Role.Name);
                    case "District Valuation Manager":
                        return RedirectToAction("DVM", "Dashboard", user.Role.Name);
                    default:
                        return RedirectToAction("HO", "Dashboard", user.Role.Name);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}