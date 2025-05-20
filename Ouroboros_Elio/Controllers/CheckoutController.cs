using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers
{
	public class CheckoutController : Controller
	{
		//[HttpGet("")]
		//public IActionResult Index()
		//{
		//	return View("index");
		//}

		public IActionResult Cancel()
		{
			return View("cancel");
		}

		//public IActionResult Success()
		//{
		//	return View("success");
		//}
	}
}
