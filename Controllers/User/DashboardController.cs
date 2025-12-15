using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Uni_Hub.Services.Interfaces;
using System.Security.Claims;

namespace My_Uni_Hub.Controllers.User
{
    [Authorize]
    [Route("user/dashboard")]

    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET /user/dashboard
        [HttpGet("")]
        public async Task<IActionResult> UserDashboard(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
               return RedirectToAction("Login", "Account");

            }

            var vm = await _dashboardService.GetDashboardForUserAsync(userId, ct);
            return View("UserDashboard", vm); 
        }
    }
}
