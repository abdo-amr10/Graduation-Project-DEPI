using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace My_Uni_Hub.Controllers.User
{

    [Authorize]
    [Route("user/chat")]
    public class ChatController : Controller
    {
        // GET: /user/chat
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Chat");  // يعرض صفحة Views/User/Chat/Index.cshtml
        }
    }
}
