using buildwave.Data;
using buildwave.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// MVC
// =====================================================
builder.Services.AddControllersWithViews();

// =====================================================
// SERVICES
// =====================================================
builder.Services.AddScoped<PermissionService>();

// =====================================================
// DATABASE
// =====================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// =====================================================
// AUTHENTICATION
// =====================================================
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Denied";

        options.Cookie.Name = "buildwave.auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;

        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// =====================================================
// AUTHORIZATION
// =====================================================
builder.Services.AddAuthorization();

// =====================================================
// SESSION
// =====================================================
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "buildwave.session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    options.IdleTimeout = TimeSpan.FromHours(8);
});

var app = builder.Build();

// =====================================================
// SEED PERMISSIONS
// =====================================================
using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await buildwave.Data.Seeders.PermissionSeeder
        .SeedAsync(context);
}

// =====================================================
// ERROR HANDLING
// =====================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

// =====================================================
// MIDDLEWARE
// =====================================================
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

// =====================================================
// ADMIN ROUTES
// =====================================================
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}"
);

// =====================================================
// DEFAULT ROUTE
// =====================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

app.Run();