using buildwave.Security;
using Microsoft.AspNetCore.Mvc;

namespace buildwave.Controllers;

public class DashboardController : Controller
{
    // =========================
    // DASHBOARD HOME
    // =========================
    [RequirePermission("dashboard.view")]
    public IActionResult Index()
    {
        return View();
    }
}