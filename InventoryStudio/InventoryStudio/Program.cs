using InventoryStudio.Authorization;
using InventoryStudio.Data;
using InventoryStudio.File;
using InventoryStudio.Importer;
using InventoryStudio.Models;
using InventoryStudio.Services;
using InventoryStudio.Services.Authorization;
using InventoryStudio.Services.File;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
    "Mgo+DSMBMAY9C3t2UVhhQlVFfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTX5QdkRjWnpdc3BRRGVV");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(typeof(IFileHandler<>), typeof(CsvFileHandler<>));
builder.Services.AddScoped(typeof(IFileHandler<>), typeof(ExcelFileHandler<>));
builder.Services.AddTransient<IFileParser, ExcelFileParser>();
builder.Services.AddTransient<IFileParser, CsvFileParser>();
builder.Services.AddSingleton<IFileParserFactory, FileParserFactory>();
builder.Services.AddScoped<CustomerImporter>();
builder.Services.AddScoped<VendorImporter>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//If the account needs to click on email confirmation to log in, this configuration needs to be added
//options => options.SignIn.RequireConfirmedAccount = true
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
});


var tokenSection = builder.Configuration.GetSection("Authentication:Jwt");
//defalt using Cookie 
builder.Services.AddAuthentication()
     .AddCookie(options =>
     {
         options.LoginPath = "/Identity/Account/Login";
         options.AccessDeniedPath = "/Identity/Account/AccessDenied";
         options.LogoutPath = "/Identity/Account/Logout";
     })
    .AddJwtBearer(options =>
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