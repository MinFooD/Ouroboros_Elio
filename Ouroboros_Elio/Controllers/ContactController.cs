using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
