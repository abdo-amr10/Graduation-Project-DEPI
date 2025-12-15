using Microsoft.AspNetCore.Mvc;

namespace My_Uni_Hub.Controllers
{
    [Route("landing")]
    public class LandingController : Controller
    {
        // GET /landing
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Landing");
        }
    }
}
