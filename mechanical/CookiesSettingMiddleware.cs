using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;

namespace mechanical
{
        public class CookieSettingMiddleware
        {
            private readonly RequestDelegate _next;

            public CookieSettingMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                // Set the cookie
                context.Response.Cookies.Append(
                    "X-BackEndCookie",
                    "your-cookie-value",
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30),
                        Path = "/owa",
                        Secure = true,
                        HttpOnly = true
                    });

                await _next(context); // Call the next middleware in the pipeline
            }
        }
}

