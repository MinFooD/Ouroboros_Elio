using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult AboutUsDetail()
        {
            return View();
        }
    }
}
