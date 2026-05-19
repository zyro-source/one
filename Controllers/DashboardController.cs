using Microsoft.AspNetCore.Mvc;
using buildwave.Security;

namespace buildwave.Controllers;

public class DashboardController : Controller
{
    [RequirePermission("dashboard.view")]
    public IActionResult Index()
    {
        return View();
    }
}