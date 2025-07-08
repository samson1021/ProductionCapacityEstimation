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
using mechanical.Models.Login;
using mechanical.Models.Entities;
using mechanical.Models.Dto.UserDto;
using mechanical.Services.AuthenticatioinService;
using Microsoft.AspNetCore.Authorization;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Admin,Super Admin,Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
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

        // Centralized session timeout config
        public static class SessionTimeoutConfig
        {
            public const double TimeoutMinutes = 20;
        }

        // Helper to set session expiration
        protected void SetSessionExpiration()
        {
            HttpContext.Session.SetString("ExpirationTime", DateTime.UtcNow.AddMinutes(SessionTimeoutConfig.TimeoutMinutes).ToBinary().ToString());
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userIdStr = HttpContext.Session.GetString("userId");
                if (!string.IsNullOrWhiteSpace(userIdStr) && Guid.TryParse(userIdStr, out var userId))
                {
                    var loggedUser = _context.Users.Include(c => c.Role).Include(c => c.District).FirstOrDefault(u => u.Id == userId);
                    if (loggedUser != null)
                    {
                        TempData["ToastMessage"] = "You are already logged in.";
                        return RedirectToDashboard(loggedUser);
                    }
                }
                // Auth cookie present but session missing or invalid: sign out
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                HttpContext.Session.Clear();
                if (TempData["ToastMessage"] != null)
                {
                    ViewBag.ToastMessage = TempData["ToastMessage"];
                }
                else
                {
                    ViewBag.ToastMessage =  "Your have been logged out. Please log in again.";
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(loginDto logins)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var loggedUser = await GetUserBySessionAsync();
                if (loggedUser != null)
                {
                    TempData["ToastMessage"] = "You are already logged in.";
                    return RedirectToDashboard(loggedUser);
                }
                return RedirectToAction("Logout");
            }
            return await Login(logins);
        }

        // [Authorize]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(loginDto logins)
        {
            if (logins.Email == null || logins.Password == null)
            {
                _logger.LogWarning("Login attempt failed for email: {Email}", logins.Email);
                return View("Index", logins);
            }

            var user = await _context.Users.Include(c => c.Role).Include(c => c.District)
                                            .Where(c => c.Email.ToUpper() == logins.Email.ToUpper() || c.emp_ID == logins.Email)
                                            .FirstOrDefaultAsync();
            
            if (user == null)
            {
                ViewData["Error"] = "Incorrect username or password.";
                _logger.LogWarning("Login attempt failed for email: {Email}", logins.Email);
                return View("Index", logins);
            }
            if (_authetnicationService.AuthenticateUserByAD(logins.Email,logins.Password))
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
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(SessionTimeoutConfig.TimeoutMinutes)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                SetUserSession(user);
                SetSessionExpiration();
                _logger.LogInformation("User {Email} logged in successfully as {Role}", user.Email, userRole);
                
                TempData["ToastMessage"] = $"Welcome, {user.Name}!";
                return RedirectToDashboard(user);
            }
            else
            {
                ViewData["Error"] = "Incorrect username or password.";
                _logger.LogWarning("Login attempt failed for email: {Email}", logins.Email);
                return View("Index", logins);
            }
        }

        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            TempData["ToastMessage"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
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

            TempData["ToastMessage"] =  "You have not logged in yet. Please log in again.";
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult Privacy()
        {
            return View();
        } 
        
        // Helper: Get user by session
        private async Task<User?> GetUserBySessionAsync()
        {
            var userIdStr = HttpContext.Session.GetString("userId");
            if (Guid.TryParse(userIdStr, out var userId))
            {
                return await _context.Users
                    .Include(c => c.Role)
                    .Include(c => c.District)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            return null;
        }

        // Helper: Set user session
        private void SetUserSession(User user)
        {
            HttpContext.Session.SetString(SessionKeys.UserId, user.Id.ToString());
            HttpContext.Session.SetString(SessionKeys.UserRole, user.Role.Name);
            HttpContext.Session.SetString(SessionKeys.UserName, user.Name);
            HttpContext.Session.SetString(SessionKeys.EmployeeId, user.emp_ID.ToString());
            if (user.Role.Name == "Relation Manager")
            {
                HttpContext.Session.SetString("unit", user.Unit?.ToString() ?? "");
                HttpContext.Session.SetString("segment", user.BroadSegment?.ToString() ?? "");
                HttpContext.Session.SetString("district", user.District?.Name ?? "");
            }
        }

        // Session key constants
        public static class SessionKeys
        {
            public const string UserId = "userId";
            public const string UserRole = "userRole";
            public const string UserName = "userName";
            public const string EmployeeId = "EmployeeId";
        }

        [HttpGet]
        public IActionResult Ping()
        {
            SetSessionExpiration();
            return Ok();
        }

    }
}