using buildwave.Data;
using buildwave.Entities;
using buildwave.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace buildwave.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================
    // LIST
    // =========================================
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .OrderBy(x => x.FullName)
            .ToListAsync();

        return View(users);
    }

    // =========================================
    // CREATE
    // =========================================
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _context.Roles
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(model);
        }

        var exists = await _context.Users
            .AnyAsync(x => x.Email == model.Email);

        if (exists)
        {
            ModelState.AddModelError("", "Email already exists.");
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(model);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = model.FullName,
            Email = model.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,

            // ✅ BCrypt PASSWORD HASH
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };

        _context.Users.Add(user);

        // ROLE
        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = model.RoleId
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================================
    // EDIT
    // =========================================
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _context.Users
            .Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound();

        var vm = new UserEditViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            IsActive = user.IsActive,
            RoleId = user.UserRoles.Select(x => x.RoleId).FirstOrDefault()
        };

        ViewBag.Roles = await _context.Roles
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(model);
        }

        var user = await _context.Users
            .Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Id == model.Id);

        if (user == null)
            return NotFound();

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.IsActive = model.IsActive;

        // UPDATE ROLE
        _context.UserRoles.RemoveRange(user.UserRoles);

        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = model.RoleId
        });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================================
    // ACTIVATE
    // =========================================
    public async Task<IActionResult> Activate(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.IsActive = true;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================================
    // DEACTIVATE
    // =========================================
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.IsActive = false;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================================
    // DELETE
    // =========================================
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _context.Users
            .Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return NotFound();

        _context.UserRoles.RemoveRange(user.UserRoles);
        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}