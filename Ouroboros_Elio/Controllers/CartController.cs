using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers
{
    public class CartController : Controller
    {
        public IActionResult CartDetail()
        {
            return View();
        }
    }
}
