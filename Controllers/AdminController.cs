using buildwave.Data;
using buildwave.Entities;
using buildwave.Security; // <-- IMPORTANT (RequirePermission)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace buildwave.Controllers;

// 🔥 SWITCHED FROM ROLES → PERMISSIONS (OPTIONAL HYBRID)
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================
    // USERS LIST
    // =========================
    [RequirePermission("users.view")]
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .ToListAsync();

        return View(users);
    }

    // =========================
    // ROLE ASSIGN PAGE
    // =========================
    [RequirePermission("users.manage")]
    public async Task<IActionResult> AssignRole(Guid userId)
    {
        var user = await _context.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound();

        var roles = await _context.Roles.ToListAsync();

        ViewBag.Roles = roles;

        return View(user);
    }

    // =========================
    // ASSIGN ROLE (POST)
    // =========================
    [HttpPost]
    [RequirePermission("users.manage")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
    {
        var exists = await _context.UserRoles
            .AnyAsync(x => x.UserId == userId && x.RoleId == roleId);

        if (!exists)
        {
            _context.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("AssignRole", new { userId });
    }

    // =========================
    // REMOVE ROLE
    // =========================
    [HttpPost]
    [RequirePermission("users.manage")]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.RoleId == roleId
            );

        if (userRole != null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("AssignRole", new { userId });
    }
}