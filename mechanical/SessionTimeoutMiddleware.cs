using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;

namespace mechanical
{
    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionTimeoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            var method = context.Request.Method.ToLower();

            // Exclude the login page and login POST requests from session check
            if ((path == "/" && method == "get") || (path == "/" && method == "post") || path.Contains("/home/index") || path.Contains("/home/login"))
            {
                await _next(context);
                return;
            }

            // Check if the session is available for other requests
            if (!context.Session.IsAvailable || !context.Session.TryGetValue("userId", out var _))
            {
                // If session is not available, redirect to login page
                context.Response.Redirect("/");
                return;
            }

            // Check for session expiration
            var expirationTimeStr = context.Session.GetString("ExpirationTime");
            
            if (string.IsNullOrEmpty(expirationTimeStr) || !long.TryParse(expirationTimeStr, out var expirationTimeLong))
            {
                // ExpirationTime missing or invalid
                await SignOutAndRedirectAsync(context);
                return;
            }
            var expirationTime = DateTime.FromBinary(expirationTimeLong);
            if (expirationTime < DateTime.UtcNow)
            {
                // Session expired
                await SignOutAndRedirectAsync(context);
                return;
            }

            // Continue to the next middleware if session is valid
            await _next(context);
        }
        async Task SignOutAndRedirectAsync(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Session.Clear();
            context.Response.Redirect("/");
        }
    }
}
