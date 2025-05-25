using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers
{
	public class CheckoutController : Controller
	{
		[HttpGet]
		public IActionResult PlaceOrder()
		{
			return View();
		}

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
