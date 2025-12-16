using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Uni_Hub.Models.ViewModels;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]

    [Route("admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IUserService _users;
        private readonly ICourseService _courses;
        private readonly IFileService _files;

        public AdminDashboardController( IUserService users, ICourseService courses,IFileService files)
        {
            _users = users;
            _courses = courses;
            _files = files;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Index()
        {
            var model = new DashboardStatsViewModel
            {
                TotalUsers = await _users.CountAsync(),
                ActiveCourses = await _courses.CountAsync(),
                UploadedFiles = await _files.CountAsync(),
                PendingRequests = 0
            };

            return View(model);
        }
    }
}
