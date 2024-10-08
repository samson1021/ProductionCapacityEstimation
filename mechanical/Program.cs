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
using mechanical.Controllers;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.UploadFileService;
using mechanical.Services.PCE.ProductionCapacityService;
using mechanical.Services.PCE.ProductionCorrectionService;
using mechanical.Services.PCE.PCECaseAssignmentService;
using Microsoft.Extensions.FileProviders;
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

//production capacity estimation
builder.Services.AddScoped<IPCECaseService, PCECaseService>();
builder.Services.AddScoped<IPCECaseTimeLineService, PCECaseTimeLineService>();
// builder.Services.AddScoped<IPCEUploadFileService, PCEUploadFileService>();
//manufacturing
builder.Services.AddScoped<IProductionCapacityService, ProductionCapacityService>();
builder.Services.AddScoped<IPCECaseScheduleService, PCECaseScheduleService>();
builder.Services.AddScoped<IProductionCorrectionService, ProductionCorrectionService>();
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/////////////////////////////// Chat web sockets //////////////////////////
// Enable WebSocket support
app.UseWebSockets();

// Store connected clients
var clients = new ConcurrentDictionary<WebSocket, string>();

app.Map("/ws", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        clients[webSocket] = ""; 
        
        await Receive(webSocket);
    }
});


async Task Receive(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4]; 
    var receivedMessage = new StringBuilder(); 

    try
    {
        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var messageSegment = Encoding.UTF8.GetString(buffer, 0, result.Count);
                receivedMessage.Append(messageSegment);

            } while (!result.EndOfMessage);

            var message = receivedMessage.ToString();
            receivedMessage.Clear();

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                clients.TryRemove(webSocket, out _);
            }
            else
            {
                // Broadcast message to all connected clients
                foreach (var client in clients.Keys)
                {
                    // if (client != webSocket && client.State == WebSocketState.Open)
                    if (client.State == WebSocketState.Open)                    
                    {
                        var msg = Encoding.UTF8.GetBytes(message);
                        await client.SendAsync(new ArraySegment<byte>(msg), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
        }
    }
    catch (WebSocketException ex)
    {
        Console.WriteLine($"WebSocket error: {ex.Message}");
        clients.TryRemove(webSocket, out _);
    }
}

Task.Run(async () =>
{
    while (true)
    {
        foreach (var client in clients.Keys)
        {
            if (client.State == WebSocketState.Closed || client.State == WebSocketState.Aborted)
            {
                clients.TryRemove(client, out _);
            }
        }
        await Task.Delay(TimeSpan.FromMinutes(1)); // Run cleanup every minute
    }
});
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



app.UseRouting();

app.UseMiddleware<SessionTimeoutMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();