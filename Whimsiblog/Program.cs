using DataAccessLayer.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Whimsiblog.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BlogContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

// - - - - - - - - - - 18+ Section - - - - - - - - - - 

// Add the 18+ validation onto the whole site
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Age18+",
        policy => policy.Requirements.Add(new AgeRequirement(18))); // Set the age of the validation (18+)
});
//  /!\ Handler lifetime MUST be scoped if it uses BlogContext /!\
builder.Services.AddScoped<IAuthorizationHandler, AgeRequirementHandler>();

// A global filter for the toast so I know if it worked
builder.Services.AddControllersWithViews(options =>
{
    // Protects the whole site
    options.Filters.Add(new AuthorizeFilter("Age18+"));

    // Add the toast filter so we can show the success message ONCE
    options.Filters.Add<AgeSuccessToastFilter>();
});

// - - - - - - - - - - End Section - - - - - - - - - - 

// Try to read a dedicated connection string for the DefaultConnection.
var blogConnection = builder.Configuration.GetConnectionString("DefaultConnection");

// Register BlogContext in the DI container, telling EF Core to use SQL Server
// with the resolved connection string above. This lets you inject BlogContext
// (e.g., into controllers) and have it connect to the right database.
builder.Services.AddDbContext<BlogContext>(options =>
    options.UseSqlServer(blogConnection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapRazorPages();

app.Run();
