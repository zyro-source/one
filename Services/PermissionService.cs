using buildwave.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace buildwave.Services;

public class PermissionService
{
    private readonly ApplicationDbContext _context;

    public PermissionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermission(Guid userId, string permissionKey)
    {
        var hasPermission = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Key == permissionKey);

        return hasPermission;
    }
}