using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers
{
	public class CustomController : Controller
	{
		[HttpGet]
		public IActionResult Custom()
		{
			return View();
		}
	}
}
