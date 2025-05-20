using BusinessLogicLayer.Dtos.CartDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ouroboros_Elio.Controllers
{
    public class CartController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ICartService _cartService;

		public CartController(UserManager<ApplicationUser> userManager, ICartService cartService)
		{
			_userManager = userManager;
			_cartService = cartService;
		}

		public async Task<IActionResult> CartDetail()
        {
            var currentUser = HttpContext.User;
			var userId = _userManager.GetUserId(currentUser);
			if (userId == null)
			{
				return RedirectToAction("Register", "Auth");
			}
			else
			{
				var cartAndCartItems = new CartAndListCartItemViewModel()
				{
					CartViewModel = await _cartService.GetCartByUserIdAsync(Guid.Parse(userId)),
					cartItemsViewModel = await _cartService.GetCartItemsByUserIdAsync(Guid.Parse(userId))
				};
				return View(cartAndCartItems);
			}
        }

		public async Task<IActionResult> UpdateCart(Guid designId, int quantity)
		{
			var currentUser = HttpContext.User;
			var userId = _userManager.GetUserId(currentUser);
			if (userId == null)
			{
				return RedirectToAction("Register", "Auth");
			}
			else
			{
				var result = await _cartService.UpdateQuantity(Guid.Parse(userId), designId, quantity);
				return RedirectToAction("CartDetail");
			}
		}

		public async Task<IActionResult> AddToCart(Guid designId, int quantity, bool productType)
		{
			var currentUser = HttpContext.User;
			var userId = _userManager.GetUserId(currentUser);
			if (userId == null)
			{
				return RedirectToAction("Register", "Auth");
			}
			else
			{
				var result = await _cartService.AddToCart(Guid.Parse(userId), designId, quantity, productType);
				return RedirectToAction("CartDetail");
			}
		}
	}
}
