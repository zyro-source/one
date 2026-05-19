namespace buildwave.Security;

public interface IPermissionService
{
    bool HasPermission(string userId, string permission);
}