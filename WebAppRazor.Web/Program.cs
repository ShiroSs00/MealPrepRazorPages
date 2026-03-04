using Microsoft.AspNetCore.Authentication.Cookies;
using WebAppRazor.BLL.DependencyInjection;
using WebAppRazor.Web.BackgroundServices;
using WebAppRazor.Web.Hubs;
using WebAppRazor.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Register DAL + BLL via extension method (3-layer architecture)
builder.Services.AddBusinessLayer(builder.Configuration);

// Register Background Services (Web layer only)
builder.Services.AddHostedService<NotificationSchedulerService>();
builder.Services.AddHostedService<ReminderBackgroundService>();

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Auto-migrate database
await app.Services.InitializeDatabaseAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
