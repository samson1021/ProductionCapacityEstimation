using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Web.Services.Description;

using mechanical;
using mechanical.Data;
using mechanical.WebSockets;
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

/////////////
using mechanical.Mapper;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.ProductionCapacityService;
using mechanical.Services.PCE.PCECaseAssignmentService;
using Microsoft.Extensions.FileProviders;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseCommentService;
using Microsoft.AspNetCore.Authentication;
using mechanical.Services.IndBldgFacilityEquipmentCostService;
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
            options.JsonSerializerOptions.WriteIndented = true; // Indent the JSON output
            // Add any other serialization options you need

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

//production capacity estimation
builder.Services.AddScoped<IPCECaseService, PCECaseService>();
builder.Services.AddScoped<IPCECaseTimeLineService, PCECaseTimeLineService>();
// builder.Services.AddScoped<IPCEUploadFileService, PCEUploadFileService>();
//manufacturing
builder.Services.AddScoped<IProductionCapacityService, ProductionCapacityService>();
builder.Services.AddScoped<IPCECaseScheduleService, PCECaseScheduleService>();
builder.Services.AddScoped<IPCECaseAssignmentService, PCECaseAssignmentService>();
builder.Services.AddScoped<IPCECaseTerminateService, PCECaseTerminateService>();
builder.Services.AddScoped<IPCECaseCommentService, PCECaseCommentService>();

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
builder.Services.AddScoped<mechanical.Services.AuthenticatioinService.IAuthenticationService, LdapAuthenticationService>();

builder.Services.AddAutoMapper(typeof(Program));

/////////////////////////////////////////////////////////////////////////////////////////////
// Registering PCE services
// builder.Services.AddHttpClient();
// builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IPCEEvaluationService, PCEEvaluationService>();

// builder.Services.AddTransient<IReportService, ReportService>();
//////////////////////////////////////////////////////////////////////////////////////////////
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
    options.LogoutPath = "/Home/Logout";
    // options.LogoutPath = "/Home/Index";
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
//if (args.Length == 1 && args[0].ToLower() == "seeddata")
//{
//    Seed.SeedData(app);
//    SeedDistrict.SeedData(app);
//    SeedUsersRolesAndDistricts.SeedData(app);
//}

// Apply database migrations automatically (if any)

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<CbeContext>();
//    context.Database.Migrate();
//    SeedUsersRolesAndDistricts.SeedData(app);
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/////////////////////////////// Chat web sockets //////////////////////////
// Enable WebSocket support
app.UseWebSockets();


// Create an instance of your WebSocket handler
var webSocketHandler = new WebSocketHandler();

// Map the WebSocket endpoint
app.Map("/ws", async context =>
{
    await webSocketHandler.HandleWebSocket(context);
});

// Start cleanup task for disconnected clients
_ = Task.Run(() => webSocketHandler.CleanupDisconnectedClients());
///////////////////////////////////////////////////////////////////////////

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


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();