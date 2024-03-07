using Microsoft.AspNetCore.Mvc;

namespace BugTracks.Controllers
{
    public class UserRolesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
