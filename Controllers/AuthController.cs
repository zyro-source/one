using buildwave.Data;
using buildwave.Entities;
using buildwave.ViewModels;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace buildwave.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================
    // LOGIN PAGE
    // =========================
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // =========================
    // LOGIN POST (DEBUG VERSION)
    // =========================
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("❌ MODEL STATE INVALID");
            return View(model);
        }

        // 🔴 BREAKPOINT 1 — USER LOOKUP
        Console.WriteLine("🔵 STEP 1: Looking up user");

        var user = await _context.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        // 🔴 BREAKPOINT 2 — USER FOUND?
        if (user == null)
        {
            Console.WriteLine("❌ USER NOT FOUND");
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        Console.WriteLine($"✅ USER FOUND: {user.Email}");

        // 🔴 BREAKPOINT 3 — PASSWORD CHECK
        bool validPassword = BCrypt.Net.BCrypt.Verify(
            model.Password,
            user.PasswordHash
        );

        if (!validPassword)
        {
            Console.WriteLine("❌ INVALID PASSWORD");
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        Console.WriteLine("✅ PASSWORD VALID");

        // 🔴 BREAKPOINT 4 — ROLE LOADING CHECK
        Console.WriteLine($"🔵 ROLE COUNT: {user.UserRoles?.Count ?? 0}");

        foreach (var r in user.UserRoles)
        {
            Console.WriteLine($"➡ ROLE FROM DB: {r.Role?.Name}");
        }

        // =========================
        // CLAIMS BUILDING
        // =========================
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName ?? ""),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // 🔴 BREAKPOINT 5 — ROLE CLAIMS CREATION
        foreach (var userRole in user.UserRoles)
        {
            Console.WriteLine($"🟡 ADDING CLAIM ROLE: {userRole.Role?.Name}");

            claims.Add(new Claim(
                ClaimTypes.Role,
                userRole.Role.Name
            ));
        }

        Console.WriteLine($"🔵 TOTAL CLAIMS: {claims.Count}");

        foreach (var c in claims)
        {
            Console.WriteLine($"CLAIM => {c.Type} : {c.Value}");
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        // 🔴 BREAKPOINT 6 — SIGN IN
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        Console.WriteLine("✅ USER SIGNED IN");

        // =========================
        // SESSION CREATION
        // =========================
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            SessionToken = Guid.NewGuid().ToString(),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
            UserAgent = Request.Headers.UserAgent.ToString(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            IsRevoked = false
        };

        _context.UserSessions.Add(session);

        user.LastLoginAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        Console.WriteLine("✅ SESSION SAVED");

        return RedirectToAction("Index", "Dashboard");
    }

    // =========================
    // DEBUG ENDPOINT (VERY IMPORTANT)
    // =========================
    [HttpGet]
    public IActionResult WhoAmI()
    {
        var data = User.Claims.Select(c => new
        {
            c.Type,
            c.Value
        });

        return Json(data);
    }

    // =========================
    // LOGOUT
    // =========================
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        Console.WriteLine("🔵 LOGOUT START");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userId, out var id))
        {
            var sessions = await _context.UserSessions
                .Where(x => x.UserId == id && !x.IsRevoked)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsRevoked = true;
            }

            await _context.SaveChangesAsync();

            Console.WriteLine("✅ SESSIONS REVOKED");
        }

        await HttpContext.SignOutAsync();
        HttpContext.Session.Clear();

        Console.WriteLine("✅ LOGOUT COMPLETE");

        return RedirectToAction("Login", "Auth");
    }
}