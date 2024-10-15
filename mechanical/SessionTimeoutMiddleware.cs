using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            if ((path == "/" && method == "get") || (path == "/" && method == "post"))
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

            // Continue to the next middleware if session is available or if on login page
            await _next(context);
        ;}
    }
}
