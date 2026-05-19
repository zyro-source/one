using buildwave.Data;
using buildwave.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =========================
// MVC
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<PermissionService>();

// =========================
// DATABASE
// =========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// =========================
// AUTHENTICATION (COOKIE)
// =========================
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Denied";

        options.Cookie.Name = "buildwave.auth";

        options.Cookie.HttpOnly = true;

        // ⚠️ In development HTTPS is fine; in production must be HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.Cookie.SameSite = SameSiteMode.Strict;

        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.SlidingExpiration = true;
    });

// =========================
// AUTHORIZATION (IMPORTANT)
// =========================
builder.Services.AddAuthorization();

// =========================
// SESSION
// =========================
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "buildwave.session";

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;

    options.IdleTimeout = TimeSpan.FromHours(8);
});

var app = builder.Build();

// =========================
// PIPELINE
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ORDER IS CRITICAL 🔥
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();