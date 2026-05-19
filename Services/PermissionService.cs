using buildwave.Data;
using Microsoft.EntityFrameworkCore;

namespace buildwave.Services;

public class PermissionService
{
    private readonly ApplicationDbContext _context;

    public PermissionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> HasPermission(Guid userId, string permissionKey)
    {
        return _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Key == permissionKey);
    }
}