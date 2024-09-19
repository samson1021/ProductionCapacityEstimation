﻿//using CreditBackOffice.Models;
using mechanical.Data;
using mechanical.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using NuGet.Protocol.Plugins;
using System.DirectoryServices.AccountManagement;
using mechanical.Models.Login;
using mechanical.Services.AuthenticatioinService;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace mechanical.Controllers
{
    public class HomeController : Controller
    {
        private readonly CbeContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(CbeContext context, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            //this.authService = authService;
        }

       
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (HttpContext.Session.GetString("userId") != null)
                {
                    var loggedUser = _context.CreateUsers.Include(c => c.Role).Include(c=>c.District).FirstOrDefault(u => u.Id == Guid.Parse(HttpContext.Session.GetString("userId")));

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
        public IActionResult Index(CreateUser logins)
        {
            //String UserEmail = "";
            //if (logins == null)
            //{
            //    return null;
            //}
            //else
            //{
            //    LdapAuthenticationService _authtenticate = new LdapAuthenticationService();

            //      UserEmail= _authtenticate.AuthenticateUserByAD(logins.Email, logins.Password);
            //}
            //if (UserEmail==null)
            //{
            //    ModelState.AddModelError(string.Empty, "Invalid email or a password");
            //    ViewBag.ShowErrorModal = true;
            //    ViewBag.ErrorMessage = "Invalid email or a password";
            //    return View("Index", logins);
            //}


            if (User.Identity.IsAuthenticated)
            {
                if (HttpContext.Session.GetString("userId") != null)
                {
                    var loggedUser = _context.CreateUsers.Include(c => c.Role).Include(c => c.District).FirstOrDefault(u => u.Id == Guid.Parse(HttpContext.Session.GetString("userId")));

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

            var user = _context.CreateUsers.Include(c => c.Role).Include(c => c.District).Where(c => c.Email == logins.Email.ToUpper()).FirstOrDefault();

            if (user == null || (user.Password != logins.Password))
            {
                ViewData["Error"] = "Incorrect Username or Password";
                // ModelState.AddModelError(string.Empty, "Invalid email or a password");
                // ViewBag.ShowErrorModal = true;
                // ViewBag.ErrorMessage = "Invalid email or a password";
                return View("Index", logins);

            }
            else
            {
                //var district = _context.Districts.FirstOrDefault(c => c.Id == id);
                string userRole = user.Role.Name;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, userRole) // Set the role for the user
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                byte[] userId = Encoding.UTF8.GetBytes(user.Id.ToString());
                _httpContextAccessor.HttpContext.Session.Set("UserId", userId);
                HttpContext.Session.SetString("userRole", userRole);
                HttpContext.Session.SetString("userName", user.Name);
                HttpContext.Session.SetString("userId", user.Id.ToString());
                HttpContext.Session.SetString("EmployeeId", user.emp_ID.ToString());
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
        }

        // [Authorize]
        // [HttpGet("logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");

            // await HttpContext.SignOutAsync();
            // return Ok();
        }

       
        public IActionResult RedirectToDashboard(CreateUser user)
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
                        return RedirectToAction("MM", "Dashboard", user.Role.Name);
                    case "Checker Manager":
                        ViewData["Center"] = user.District.Name;
                        return RedirectToAction("MM", "Dashboard", user.Role.Name);
                    case "Checker Officer":
                        return RedirectToAction("MM", "Dashboard", user.Role.Name);
                    case "District Valuation Manager":
                        return RedirectToAction("MM", "Dashboard", user.Role.Name);
                    default:
                        return RedirectToAction("HO", "Dashboard", user.Role.Name);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}