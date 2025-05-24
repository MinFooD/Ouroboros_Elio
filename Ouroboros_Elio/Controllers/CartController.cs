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

		public async Task<IActionResult> CartDetail(Guid? designId)
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

                ViewBag.designId = designId;

                return View(cartAndCartItems);
			}
        }

        public async Task<IActionResult> UpdateCart(Guid designId, int quantity)
        {
            var currentUser = HttpContext.User;
            var userId = _userManager.GetUserId(currentUser);
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
            }

            try
            {
                var result = await _cartService.UpdateQuantity(Guid.Parse(userId), designId, quantity);
                if (result == true)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật số lượng." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid designId, int quantity, bool productType)
        {
            var currentUser = HttpContext.User;
            var userId = _userManager.GetUserId(currentUser);

            if (userId == null)
            {
                Response.StatusCode = 401; // Trả về mã trạng thái 401
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
                //return RedirectToAction("Login", "Auth");
            }

            try
            {
                var result = await _cartService.AddToCart(Guid.Parse(userId), designId, quantity, productType);
                if (result)
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không thể thêm sản phẩm vào giỏ hàng." });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm sản phẩm." });
            }
        }
    }
}
