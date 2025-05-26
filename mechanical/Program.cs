using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using System.Web.Services.Description;

using mechanical;
using mechanical.Data;
using mechanical.Hubs;
using mechanical.Controllers;
using mechanical.Models.Entities;

using mechanical.Services.CaseServices;
using mechanical.Services.UploadFileService;
using mechanical.Services.CollateralService;
using mechanical.Services.AnnexService;
using mechanical.Services.MotorVehicleService;
using mechanical.Services.SignatureService;
using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.AuthenticatioinService;
using mechanical.Services.ConstMngAgrMachineryService;
using mechanical.Services.CorrectionServices;
using mechanical.Services.UserService;
using mechanical.Services.MMCaseService;
using mechanical.Services.MailService;
using mechanical.Services.MOCaseService;
using mechanical.Services.CTLCaseService;
using mechanical.Services.CaseCommentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.IndBldgF;
using mechanical.Services.IndBldgFacilityEquipmentService;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.IndBldgFacilityEquipmentCostService;
using mechanical.Services.InternalReportService;
using mechanical.Services.TaskManagmentService;
using mechanical.Services.NotificationService;

/////////////
using mechanical.Mapper;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.ProductionCapacityService;
using mechanical.Services.PCE.PCECaseAssignmentService;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseCommentService;

/////////////

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache(); // Add distributed memory cache for session storage

//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add(typeof(HomeController));
//});

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
    options.LoginPath = "/Home/Index";
    options.LogoutPath = "/Home/Logout";
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

var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    // Seed.SeedData(app);
    // SeedDistrict.SeedData(app);
    // SeedUsersRolesAndDistricts.SeedData(app);
}

// Apply database migrations automatically (if any)

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CbeContext>();
    context.Database.Migrate(); // Apply migrations
    // Seed.SeedData(app);
    // SeedDistrict.SeedData(app);
    SeedUsersRolesAndDistricts.SeedData(app);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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