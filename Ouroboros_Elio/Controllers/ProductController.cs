using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult ProductList()
        {
            return View();
        }
    }
}
