using buildwave.Data;
using buildwave.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace buildwave.Controllers.Admin;

public class RolePermissionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RolePermissionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync();

        return View(roles);
    }

    public async Task<IActionResult> Edit(Guid roleId)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId);

        var permissions = await _context.Permissions.ToListAsync();

        ViewBag.Permissions = permissions;

        return View(role);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid roleId, List<Guid> permissionIds)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId);

        if (role == null)
            return NotFound();

        // remove old
        _context.RolePermissions.RemoveRange(role.RolePermissions);

        // add new
        foreach (var pid in permissionIds)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = pid
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}