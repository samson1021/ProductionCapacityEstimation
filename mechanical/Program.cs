using Azure;
using mechanical;
using mechanical.Data;
using mechanical.Hubs;
/////////////
using mechanical.Mapper;
using mechanical.Services.AnnexService;
using mechanical.Services.AuthenticatioinService;
using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseCommentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.CollateralService;
using mechanical.Services.ConstMngAgrMachineryService;
using mechanical.Services.CorrectionServices;
using mechanical.Services.IndBldgF;
using mechanical.Services.IndBldgFacilityEquipmentCostService;
using mechanical.Services.IndBldgFacilityEquipmentService;
using mechanical.Services.InternalReportService;
using mechanical.Services.MailService;
using mechanical.Services.MMCaseService;
using mechanical.Services.MOCaseService;
using mechanical.Services.MotorVehicleService;
using mechanical.Services.NotificationService;
using mechanical.Services.PCE.PCECaseAssignmentService;
using mechanical.Services.PCE.PCECaseCommentService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.ProductionCapacityService;
using mechanical.Services.SignatureService;
using mechanical.Services.TaskManagmentService;
using mechanical.Services.UploadFileService;
using mechanical.Services.UserService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

/////////////
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache(); // Add distributed memory cache for session storage

//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add(typeof(HomeController));
//});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "mechanical.Session"; // Set a unique name for the session cookie

    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set the session timeout
    options.Cookie.HttpOnly = true; // Ensure the session cookie is accessible only via HTTP
    options.Cookie.IsEssential = true;
});
builder.Services.AddSession();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.IgnoreNullValues = true;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

////////////////////
builder.Services.AddSwaggerGen();
////////////////////
///
//builder.Services.AddDbContext<CbeCreditContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("CbeCreditContext") ??
//            throw new InvalidOperationException("Connection string 'CbeCreditContext' not found.")));
builder.Services.AddDbContext<CbeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CbeContext") ??
                            throw new InvalidOperationException("Connection string 'CbeContext' not found.")));

/////////////////////////////////////////////////////////////////////////////////////////////
// builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(typeof(MappingProfile));

//production capacity estimation
builder.Services.AddScoped<IPCEEvaluationService, PCEEvaluationService>();

builder.Services.AddScoped<IPCECaseService, PCECaseService>();
builder.Services.AddScoped<IPCECaseTimeLineService, PCECaseTimeLineService>();
// builder.Services.AddScoped<IPCEUploadFileService, PCEUploadFileService>();
//manufacturing
builder.Services.AddScoped<IProductionCapacityService, ProductionCapacityService>();
builder.Services.AddScoped<IPCECaseScheduleService, PCECaseScheduleService>();
builder.Services.AddScoped<IPCECaseAssignmentService, PCECaseAssignmentService>();
builder.Services.AddScoped<IPCECaseTerminateService, PCECaseTerminateService>();
builder.Services.AddScoped<IPCECaseCommentService, PCECaseCommentService>();
builder.Services.AddScoped<IInternalReportService, InternalReportService>();
builder.Services.AddScoped<ICaseService, CaseService>();
builder.Services.AddScoped<ICaseAssignmentService, CaseAssignmentService>();
builder.Services.AddScoped<ICaseTimeLineService, CaseTimeLineService>();
builder.Services.AddScoped<ICaseCommentService, CaseCommentService>();
builder.Services.AddScoped<ICollateralService, CollateralService>();
builder.Services.AddScoped<IConstMngAgrMachineryService, ConstMngAgrMachineryService>();
builder.Services.AddScoped<IIndBldgFacilityEquipmentService, IndBldgFacilityEquipmentService>();
builder.Services.AddScoped<IIndBldgFacilityEquipmentCostService, IndBldgFacilityEquipmentCostService>();
builder.Services.AddScoped<ICMCaseService, CMCaseService>();
builder.Services.AddScoped<IMailService, MailService>();
//builder.Services.AddScoped<ICTLCaseService, CTLCaseService>();
builder.Services.AddScoped<IAnnexService, AnnexService>();
builder.Services.AddScoped<IMotorVehicleService, MotorVehicleService>();
builder.Services.AddScoped<IMMCaseService, MMCaseService>();
builder.Services.AddScoped<ICOCaseService, MOCaseService>();
builder.Services.AddScoped<IUploadFileService, UploadFileService>();
builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<ICorrectionService, CorrectionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICaseScheduleService, CaseScheduleService>();
builder.Services.AddScoped<ICaseTerminateService, CaseTerminateService>();
builder.Services.AddScoped<ITaskManagmentService, TaskManagmentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<mechanical.Services.AuthenticatioinService.IAuthenticationService, LdapAuthenticationService>();

builder.Services.AddAutoMapper(typeof(Program));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    string SanitizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty");

        path = path.Trim();
        if (!path.StartsWith("/"))
            path = "/" + path;

        if (path.Contains("..") || path.Contains("//"))
            throw new ArgumentException("Invalid path format");

        return path;
    }

    options.LoginPath = SanitizePath("/Home/Index");
    options.LogoutPath = SanitizePath("/Home/Logout");
//options.LoginPath = "/Home/Index";
//options.LogoutPath = "/Home/Logout";
// options.LogoutPath = "/Home/Index";
//options.AccessDeniedPath = "/Account/AccessDenied";
//options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
//options.SlidingExpiration = true;

options.AccessDeniedPath = "/Home/Index";
    options.Cookie.Name = "MechanicalCookie"; // Set a unique name for the authentication cookie
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Set the expiration time for the cookie
    options.SlidingExpiration = true; // Extend the expiration time with each request
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt")["Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

//builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
})
.AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null; // Keep PascalCase
});


// Register the custom IUserIdProvider
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/javascript" });
});
var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    // Seed.SeedData(app);
    // SeedDistrict.SeedData(app);
    // SeedUsersRolesAndDistricts.SeedData(app);
}

// Apply database migrations automatically (if any)

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<CbeContext>();
//    context.Database.Migrate(); // Apply migrations
//    // Seed.SeedData(app);
//    // SeedDistrict.SeedData(app);
//    SeedUsersRolesAndDistricts.SeedData(app);
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


//app.Use(async (context, next) =>
//{


//    context.Response.Headers.Remove("Server");
//    //context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
//    // Generate a random nonce per request
//    var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

//    context.Response.Headers.Add("Content-Security-Policy",
//        $"default-src 'self'; script-src 'self' 'nonce-{nonce}';");

//    // Pass the nonce to your view (if using Razor)
//    ViewData["Nonce"] = nonce;
//    context.Response.Headers.Remove("X-Powered-By");
//    context.Response.Headers["X-Content-Type-Options"] = "nosniff"; // Example additional header
//    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
//    context.Response.Headers["X-Frame-Options"] = "DENY";
//    context.Response.Headers["Cache-Control"] = "no-cache";
//    //context.Response.Headers["Content-Encoding"] = "gzip"; // Be careful with this; it might be set by the server itself
//    context.Response.Headers["Expires"] = "-1";
//    context.Response.Headers["Pragma"] = "no-cache";

//    await next();
//});
app.Use(async (context, next) =>
{
    // Remove server identification headers
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");

    // For development only - allows inline styles and scripts
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline'; " +  // Allows inline scripts
        "style-src 'self' 'unsafe-inline'; " +   // Allows inline styles
        "img-src 'self' data:; " +              // Allows images from self and data URIs
        "font-src 'self'; " +                   // Allows fonts from self
        "connect-src 'self'; " +                // Allows AJAX/WebSocket connections
        "object-src 'none';");                  // Blocks plugins (Flash, etc.)

    // Add other security headers
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Cache-Control"] = "no-cache";
    context.Response.Headers["Expires"] = "-1";
    context.Response.Headers["Pragma"] = "no-cache";

    await next();
});

app.UseSession(); // Add the session middleware
//app.UseMiddleware<SessionTimeoutMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Serve static files from UploadFile directory

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), @"UploadFile")),
    RequestPath = new PathString("/UploadFile")
});

///////////////////////////////
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});

// app.UseRouting();
// app.UseAuthorization();
// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
// });
//////////////////////////////

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<SessionTimeoutMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapHub<NotificationHub>("/notificationHub");
    // endpoints.MapControllers();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// app.MapHub<NotificationHub>("/notificationHub");

app.Run();