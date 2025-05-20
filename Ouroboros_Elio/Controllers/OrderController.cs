using BusinessLogicLayer.Dtos.PaymentDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace Ouroboros_Elio.Controllers
{
	public class OrderController : Controller
	{
		private readonly IOrderService _orderService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly PayOS _payOS;

		public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager, PayOS payOS)
		{
			_orderService = orderService;
			_userManager = userManager;
			_payOS = payOS;
		}

		[HttpGet]
		public async Task<IActionResult> Success(Guid cartId)
		{
			var currentUser = HttpContext.User;
			var userId = _userManager.GetUserId(currentUser);
			if (userId == null)
			{
				return RedirectToAction("Register", "Auth");
			}
			else
			{
				var order = await _orderService.CreateOrderFromCartAsync(cartId, Guid.Parse(userId));
				return View(order);
			}
				
		}

		[HttpPost]
		public async Task<IActionResult> CreatePaymentLink([FromForm] CreatePaymentLinkRequest body)
		{
			try
			{
				int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
				ItemData item = new ItemData(body.productName, 1, body.price);
				List<ItemData> items = new List<ItemData>();
				items.Add(item);
				PaymentData paymentData = new PaymentData(orderCode, body.price, body.description, items, body.cancelUrl, body.returnUrl);

				CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

				//return Ok(new Response(0, "success", createPayment));
				return Redirect(createPayment.checkoutUrl);
			}
			catch (System.Exception exception)
			{
				Console.WriteLine(exception);
				return RedirectToAction("Cancel","Checkout");
			}
		}
	}
}
