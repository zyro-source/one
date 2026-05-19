namespace buildwave.Entities;

public class Permission
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Key { get; set; } = default!;
    // e.g. users.create, inventory.approve

    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();
}