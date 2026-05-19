using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using buildwave.Services;
using System.Security.Claims;

namespace buildwave.Security;

public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _permission;

    public RequirePermissionAttribute(string permission)
    {
        _permission = permission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userIdValue = context.HttpContext.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        var service = context.HttpContext.RequestServices
            .GetRequiredService<PermissionService>();

        var hasPermission = await service.HasPermission(userId, _permission);

        if (!hasPermission)
        {
            context.Result = new RedirectToActionResult("Denied", "Auth", null);
        }
    }
}