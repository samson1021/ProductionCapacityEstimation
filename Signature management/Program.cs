using Microsoft.AspNetCore.Identity;
using Signature_management.Data;
using Signature_management.Service.AcknowledgementService;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Web.Services.Description;
using AutoMapper;
using Signature_management.Service.SignatureService;


var builder = WebApplication.CreateBuilder(args);

//services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy =>
//        policy.RequireRole("Admin"));
//});
// Add services to the container.

//builder.Services.AddDefaultIdentity<IdentityUser>();
//    .AddRoles<IdentityRole>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CbeCreditContext") ??
                         throw new InvalidOperationException("Connection string 'MyDbContext' not found.")));


builder.Services.AddScoped<IAcknowledgementService, AcknowledgementService>();
builder.Services.AddScoped<ISignatureService, SignatureService>();
//builder.Services.AddSingleton<IWebHostEnvironment>(provider => provider.GetRequiredService<IWebHostEnvironment>());
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole",
         policy => policy.RequireRole("Administrator"));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

