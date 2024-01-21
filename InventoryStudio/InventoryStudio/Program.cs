using InventoryStudio.Authorization;
using InventoryStudio.Data;
using InventoryStudio.Models;
using InventoryStudio.Services;
using InventoryStudio.Services.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
    "Mgo+DSMBMAY9C3t2VlhhQlJCfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn9RdkRiXX9fcHVXQmZa");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//If the account needs to click on email confirmation to log in, this configuration needs to be added
//options => options.SignIn.RequireConfirmedAccount = true
builder.Services.AddDefaultIdentity<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.LoginPath = "/Account/Login";
                       options.AccessDeniedPath = "/Account/AccessDenied";
                       options.LogoutPath = "/Account/Logout";
                   });

var tokenSection = builder.Configuration.GetSection("Authentication:Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = tokenSection["Issuer"],
        ValidAudience = tokenSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSection["Key"])),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var authService = context.HttpContext.RequestServices.GetRequiredService<InventoryStudio.Services.Authorization.IAuthorizationService>();
            await authService.ValidateToken(context);
        }

    };
}).AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddScoped<InventoryStudio.Services.Authorization.IAuthorizationService, AuthorizationService>();

// Add PermissionService
//delete this , when deploy to prod, this server will update permissions to match system
builder.Services.AddSingleton<PermissionService>();

//Dynamic Register Policy to make sure the poliy have define
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();

// PermissionHandler
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

//config email server
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();

builder.Services.AddLogging();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//delete this , when deploy to prod, this server will update permissions to match system
var permissionService = app.Services.GetRequiredService<PermissionService>();
permissionService.InitializePermissions();


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();