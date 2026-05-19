using buildwave.Data;
using buildwave.Entities;
using Microsoft.EntityFrameworkCore;

namespace buildwave.Data.Seeders;

public static class PermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // =========================
        // 1. SEED PERMISSIONS
        // =========================
        var permissions = new List<Permission>
        {
            new Permission { Name = "View Dashboard", Key = "dashboard.view" },
            new Permission { Name = "Admin Access", Key = "admin.access" },
            new Permission { Name = "Manage Users", Key = "users.manage" },
            new Permission { Name = "Manage Roles", Key = "roles.manage" },
            new Permission { Name = "Manage Permissions", Key = "permissions.manage" }
        };

        foreach (var permission in permissions)
        {
            var exists = await context.Permissions
                .AnyAsync(p => p.Key == permission.Key);

            if (!exists)
            {
                context.Permissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();

        // =========================
        // 2. GET ADMIN ROLE
        // =========================
        var adminRole = await context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r =>
                r.Name.ToLower() == "admin" ||
                r.Name.ToLower() == "superadmin"
            );

        if (adminRole == null)
        {
            throw new Exception("Admin/SuperAdmin role not found. Check role seeding.");
        }

        // =========================
        // 3. ATTACH ALL PERMISSIONS TO ADMIN ROLE
        // =========================
        var allPermissions = await context.Permissions.ToListAsync();

        foreach (var permission in allPermissions)
        {
            var exists = adminRole.RolePermissions
                .Any(rp => rp.PermissionId == permission.Id);

            if (!exists)
            {
                adminRole.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await context.SaveChangesAsync();
    }
}