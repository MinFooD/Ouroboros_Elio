using BusinessLogicLayer.Dtos.CartDtos;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Messaging;

namespace Ouroboros_Elio.Controllers;

public class CartController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICartService _cartService;
    private readonly IDesignService _designService;

    public CartController(UserManager<ApplicationUser> userManager, ICartService cartService, IDesignService designService)
    {
        _userManager = userManager;
        _cartService = cartService;
        _designService = designService;
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

        // Kiểm tra tồn kho trước khi cập nhật
        var design = await _designService.GetDesignByIdAsync(designId);
        if (design != null && quantity > design.StockQuantity)
        {
            return Json(new { success = false, message = $"Chỉ còn {design.StockQuantity} sản phẩm trong kho." });
        }

        var (success, message) = await _cartService.UpdateQuantity(Guid.Parse(userId), designId, quantity);
        if (success == true)
        {
            return Json(new { success = true, message = message });
        }
        return Json(new { success = false, message = message });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(Guid designId, int quantity, bool productType)
    {
        var currentUser = HttpContext.User;
        var userId = _userManager.GetUserId(currentUser);

        if (userId == null)
        {
            Response.StatusCode = 401;
            return Json(new { success = false, message = "Vui lòng đăng nhập." });
        }

        var (success, message) = await _cartService.AddToCart(Guid.Parse(userId), designId, quantity, productType);
        if (success)
        {
            return Json(new { success = true, message = message });
        }
        return Json(new { success = false, message = message });
    }

    [HttpGet]
    public async Task<IActionResult> GetCartItemCount()
    {
        var currentUser = HttpContext.User;
        var userId = _userManager.GetUserId(currentUser);

        if (userId == null)
        {
            return Json(new { success = false, count = 0 });
        }

        try
        {
            var cartItems = await _cartService.GetCartItemsByUserIdAsync(Guid.Parse(userId));
            var count = cartItems?.Sum(item => item.Quantity) ?? 0;
            return Json(new { success = true, count = count });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, count = 0 });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCartItemQuantity(Guid designId)
    {
        var currentUser = HttpContext.User;
        var userId = _userManager.GetUserId(currentUser);

        if (userId == null)
        {
            return Json(new { success = false, quantity = 0 });
        }

        try
        {
            var cartItems = await _cartService.GetCartItemsByUserIdAsync(Guid.Parse(userId));
            var quantity = cartItems?.FirstOrDefault(item => item.DesignId == designId)?.Quantity ?? 0;
            return Json(new { success = true, quantity = quantity });
        }
        catch (Exception)
        {
            return Json(new { success = false, quantity = 0 });
        }
    }
}
