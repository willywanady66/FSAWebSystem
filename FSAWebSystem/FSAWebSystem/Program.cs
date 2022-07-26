using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using FSAWebSystem.Services;
using System.Globalization;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions();
var connectionString = builder.Configuration.GetConnectionString("FSAWebSystemDbContextConnection") ?? throw new InvalidOperationException("Connection string 'FSAWebSystemDbContextConnection' not found.");
builder.Services.AddDbContext<FSAWebSystemDbContext>(options =>
    options.UseSqlServer(connectionString));



builder.Services.AddDefaultIdentity<FSAWebSystemUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<FSAWebSystemDbContext>();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(10);//You can set Time   
});


builder.Services.Configure<EmailContext>(builder.Configuration.GetSection("EmailContext"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IUploadDocumentService, UploadDocumentService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISKUService, SKUService>();
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddScoped<IBucketService, BucketService>();
// Add services to the container.
builder.Services.AddNotyf(config => { config.DurationInSeconds = 5; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPage", policy => policy.RequireClaim("Menu", "Admin"));
    options.AddPolicy("ReportPage", policy => policy.RequireClaim("Menu", "Report"));
    options.AddPolicy("ProposalPage", policy => policy.RequireClaim("Menu", "Proposal"));
    options.AddPolicy("ApprovalPage", policy => policy.RequireClaim("Menu", "Approval"));
    //options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", "Administrator"));
});


var app = builder.Build();

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<FSAWebSystemDbContext>();
var userMgr = services.GetRequiredService<UserManager<FSAWebSystemUser>>();
var roleSvc = services.GetRequiredService<IRoleService>();

context.Database.Migrate();
Configuration.Initialize(context, userMgr, roleSvc);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
   
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;
app.UseNotyf();


app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
