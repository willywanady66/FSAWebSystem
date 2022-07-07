using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FSAWebSystem.Areas.Identity.Data;
using FSAWebSystem.Models.Context;
using FSAWebSystem.Services.Interface;
using FSAWebSystem.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("FSAWebSystemDbContextConnection") ?? throw new InvalidOperationException("Connection string 'FSAWebSystemDbContextConnection' not found.");

builder.Services.AddDbContext<FSAWebSystemDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDefaultIdentity<FSAWebSystemUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<FSAWebSystemDbContext>();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(10);//You can set Time   
});
    
builder.Services.AddScoped<IUploadDocumentService, UploadDocumentService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISKUService, SKUService>();
builder.Services.AddScoped<IProposalService, ProposalService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
newCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
newCulture.DateTimeFormat.DateSeparator = "/";
Thread.CurrentThread.CurrentCulture = newCulture;


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


app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
