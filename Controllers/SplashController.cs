using Microsoft.AspNetCore.Mvc;

namespace My_Uni_Hub.Controllers
{
    [Route("user/splash")]
    public class SplashController : Controller
    {
        // GET /splash
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Splash");
        }
    }
}
