using buildwave.Data;
using buildwave.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace buildwave.Controllers.Admin;

[Authorize]
public class PermissionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PermissionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var permissions = await _context.Permissions
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View(permissions);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Permission model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.Id = Guid.NewGuid();

        _context.Permissions.Add(model);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var permission = await _context.Permissions.FindAsync(id);

        if (permission == null)
            return NotFound();

        return View(permission);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Permission model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.Permissions.Update(model);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var permission = await _context.Permissions.FindAsync(id);

        if (permission == null)
            return NotFound();

        _context.Permissions.Remove(permission);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}