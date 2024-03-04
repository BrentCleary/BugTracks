using BugTracks.Data;
using BugTracks.Models;
using BugTracks.Services;
using BugTracks.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(DataUtility.GetConnectionString(builder.Configuration),
                prop => prop.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BTUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();


// --- BTSERVICES ---
// Roles Services
builder.Services.AddScoped<IBTRolesService, BTRolesService>();
// CompanyInfo Services
builder.Services.AddScoped<IBTCompanyInfoService, BTCompanyInfoService>();
// Project Services
builder.Services.AddScoped<IBTProjectService, BTProjectService>();
// Ticket Services
builder.Services.AddScoped<IBTTicketService, BTTicketService>();
// TicketHistory Services
builder.Services.AddScoped<IBTTicketHistoryService, BTTicketHistoryService>();
// Email Services
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailSender, BTEmailService>();
// Notification Service
builder.Services.AddScoped<IBTNotificationService, BTNotificationService>();
// Invite Service
builder.Services.AddScoped<IBTInviteService, BTInviteService>();
// File Service
builder.Services.AddScoped<IBTFileService, BTFileService>();



var app = builder.Build();

await DataUtility.ManageDataAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
