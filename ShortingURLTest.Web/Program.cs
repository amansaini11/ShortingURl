using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShortingURL.Web.Context;
using ShortingURLTest.Web.Context;
using ShortingURLTest.Web.Entity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Password.RequireNonAlphanumeric = false;
})
.AddRoles<AppRole>()
.AddRoleManager<RoleManager<AppRole>>()
.AddSignInManager<SignInManager<AppUser>>()
.AddRoleValidator<RoleValidator<AppRole>>()
.AddEntityFrameworkStores<DataContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
            .AddIdentityCookies();

builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
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

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Middleware to handle all web request and logic to redirect//
app.Use(async (context, next) =>
{
    var domain = context.Request.Host.ToString();
    var siteUrl = context.Request.Path.Value.ToString();
    if (!string.IsNullOrEmpty(siteUrl) && siteUrl != "/" && !siteUrl.ToLower().Contains("/entrynotfound") && siteUrl.ToLower() != "/favicon.ico" && !siteUrl.ToLower().Contains("/notvalidpage")
    && !siteUrl.ToLower().Contains("/saveurlmapping") && !siteUrl.ToLower().Contains("/register") && !siteUrl.ToLower().Contains("/login") && !siteUrl.ToLower().Contains("/profile") && !siteUrl.ToLower().Contains("/forgetpassword")
    && !siteUrl.ToLower().Contains("/logout") && !siteUrl.ToLower().Contains("/myurls") && !siteUrl.ToLower().Contains("/removeurl") && !siteUrl.ToLower().Contains("/removeuser"))
    {
        siteUrl = siteUrl.Remove(0, 1);
        var _context = builder.Services.BuildServiceProvider().GetService<DataContext>();

        //check url in database//
        var result = _context.UrlMappings.Where(x => x.TinyUrl == siteUrl).FirstOrDefault();
        if (result == null)
        {
            context.Response.Redirect("EntryNotFound");
            return;
        }       
        else
        {
            Uri uriResult;
            bool urlValid = Uri.TryCreate(result.Orignalurl, UriKind.Absolute, out uriResult);
            if (urlValid)
            {
                context.Response.Redirect(result.Orignalurl);
                return;
            }
            else
            {
                context.Response.Redirect("NotValidPage");
                return;
            }
        }
    }
    await next(context);
});

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedRoles(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}
app.Run();
