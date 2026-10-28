
using DigitalAccessibilityResearch1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// SQLite database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Account and password management + Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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


// Development-only: create the Admin role and assign it
// to the explicitly configured, already-registered account.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = "Admin";

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        var roleResult =
            await roleManager.CreateAsync(new IdentityRole(adminRole));

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                "Could not create the Admin role: " +
                string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }
    }

    var adminEmail = app.Configuration["AdminSeed:Email"];

    if (!string.IsNullOrWhiteSpace(adminEmail))
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser != null &&
            !await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            var result =
                await userManager.AddToRoleAsync(adminUser, adminRole);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not assign Admin role: " +
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}



app.Run();
