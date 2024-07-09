using mechanical.Data;
using Microsoft.EntityFrameworkCore;
using mechanical.Services.CaseServices;
using mechanical.Services.UploadFileService;
using mechanical.Models.Entities;
using mechanical.Services.CollateralService;
using mechanical.Services.AnnexService;
using mechanical.Services.MotorVehicleService;
using Microsoft.AspNetCore.Identity;
using mechanical;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using System.Web.Services.Description;
using mechanical.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache(); // Add distributed memory cache for session storage

//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add(typeof(HomeController));
//});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".YourApp.Session"; // Set a unique name for the session cookie
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set the session timeout
    options.Cookie.HttpOnly = true; // Ensure the session cookie is accessible only via HTTP
    options.Cookie.IsEssential = true;
});
builder.Services.AddSession();

builder.Services.AddControllers()
       
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // For Newtonsoft.Json
            options.JsonSerializerOptions.IgnoreNullValues = true; // Ignore null values
            options.JsonSerializerOptions.WriteIndented = true; // Indent the JSON output
            // Add any other serialization options you need

        });

//builder.Services.AddDbContext<CbeCreditContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("CbeCreditContext") ??
//            throw new InvalidOperationException("Connection string 'CbeCreditContext' not found.")));
builder.Services.AddDbContext<CbeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CbeCreditContext") ??
                         throw new InvalidOperationException("Connection string 'CbeContext' not found.")));

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ICaseService, CaseService>();
builder.Services.AddScoped<ICaseAssignmentService,CaseAssignmentService>();
builder.Services.AddScoped<ICaseTimeLineService, CaseTimeLineService>();
builder.Services.AddScoped<ICaseCommentService, CaseCommentService>();
builder.Services.AddScoped<ICollateralService, CollateralService>();
builder.Services.AddScoped<IConstMngAgrMachineryService, ConstMngAgrMachineryService>();
builder.Services.AddScoped<IIndBldgFacilityEquipmentService, IndBldgFacilityEquipmentService>();
builder.Services.AddScoped<ICMCaseService, CMCaseService>();
builder.Services.AddScoped<IMailService, MailService>();
//builder.Services.AddScoped<ICTLCaseService, CTLCaseService>();
builder.Services.AddScoped<IAnnexService, AnnexService>();
builder.Services.AddScoped<IMotorVehicleService, MotorVehicleService>();
builder.Services.AddScoped<IMMCaseService, MMCaseService>();
builder.Services.AddScoped<ICOCaseService, MOCaseService>();
builder.Services.AddScoped<IUploadFileService, UploadFileService>();
builder.Services.AddScoped<ISignatureService,SignatureService>();
builder.Services.AddScoped<ICorrectionService, CorrectionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICaseScheduleService, CaseScheduleService>();
builder.Services.AddScoped<ICaseTerminateService, CaseTerminateService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Home/Index";
    options.LogoutPath = "/Home/Index";
    //options.AccessDeniedPath = "/Account/AccessDenied";
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    //options.SlidingExpiration = true;

    options.AccessDeniedPath = "/Home/Index";
    options.Cookie.Name = "YourAppCookieName"; // Set a unique name for the authentication cookie
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Set the expiration time for the cookie
    options.SlidingExpiration = true; // Extend the expiration time with each request
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});
//builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    Seed.SeedData(app);
    SeedDistrict.SeedData(app);
}

Seed.SeedData(app);
SeedDistrict.SeedData(app);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession(); // Add the session middleware
//app.UseMiddleware<SessionTimeoutMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<SessionTimeoutMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
