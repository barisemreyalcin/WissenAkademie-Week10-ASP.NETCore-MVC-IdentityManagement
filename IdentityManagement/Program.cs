using IdentityManagement.IdentityPolicy;
using IdentityManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>();

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/Authenticate/Login";
//    options.AccessDeniedPath = "/Authenticate/AccessDenied"; // AccessDenied path deðiþtirilebilir
//});

//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.User.RequireUniqueEmail = true;
//    options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz";
//});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "TestCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
});

// Custom Username and Email Policy
builder.Services.AddTransient<IUserValidator<AppUser>, CustomUsernameEmailPolicy>();

// Custom Password Policy
builder.Services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordPolicy>();

// Password Policy Configuration
//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.Password.RequiredLength = 8;
//	options.Password.RequireLowercase= false;
//	options.Password.RequireDigit= true;

//});

var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
