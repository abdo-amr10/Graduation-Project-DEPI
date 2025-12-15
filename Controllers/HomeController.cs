using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.ViewModels;
using System.Diagnostics;

namespace My_Uni_Hub.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return RedirectToAction("Splash", "User");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
