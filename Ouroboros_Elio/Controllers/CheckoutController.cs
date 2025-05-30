using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ouroboros_Elio.Models;
using System.Security.Claims;

namespace Ouroboros_Elio.Controllers;

public class CheckoutController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICartRepository _cartRepository;
    private readonly IOrderService _orderService;
    private readonly IDesignService _designService;

    public CheckoutController(
        UserManager<ApplicationUser> userManager,
        ICartRepository cartRepository,
        IOrderService orderService,
        IDesignService designService)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _orderService = orderService;
        _designService = designService;
    }

    [HttpGet("Checkout/PlaceOrder")]
    public async Task<IActionResult> PlaceOrder()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth", new { ReturnUrl = "/Checkout/PlaceOrder" });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return BadRequest("Invalid user ID format.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var userCart = await _cartRepository.GetCartByUserIdAsync(userGuid);
        var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userGuid);

        var cartItemViewModels = new List<CartItemViewModel>();
        if (cartItems != null)
        {
            foreach (var item in cartItems)
            {
                var designViewModel = await _designService.GetDesignByIdAsync(item.DesignId);
                if (designViewModel != null)
                {
                    if (designViewModel.StockQuantity < item.Quantity)
                    {
                        // Cập nhật số lượng trong giỏ hàng
                        await _cartRepository.UpdateQuantity(userGuid, item.DesignId, designViewModel.StockQuantity);
                        TempData["StockWarning"] = $"Số lượng sản phẩm {designViewModel.DesignName} đã được điều chỉnh còn {designViewModel.StockQuantity} do tồn kho thay đổi.";
                        return RedirectToAction("CartDetail", "Cart");
                    }
                    if (designViewModel.StockQuantity == 0)
                    {
                        // Xóa sản phẩm khỏi giỏ hàng
                        await _cartRepository.UpdateQuantity(userGuid, item.DesignId, 0);
                        TempData["StockWarning"] = $"Sản phẩm {designViewModel.DesignName} đã hết hàng và được xóa khỏi giỏ hàng.";
                        return RedirectToAction("CartDetail", "Cart");
                    }
                    cartItemViewModels.Add(new CartItemViewModel
                    {
                        DesignId = item.DesignId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Design = designViewModel
                    });
                }
            }
        }

        var model = new CheckoutViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Address = user.Address,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CartItems = cartItemViewModels,
            TotalAmount = userCart?.Total ?? 0
        };

        return View(model);
    }

    [HttpPost("Checkout/PlaceOrder")]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth", new { ReturnUrl = "/Checkout/PlaceOrder" });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return BadRequest("Invalid user ID format.");
        }

        // Cập nhật thông tin người dùng
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        else
        {
            return NotFound("User not found.");
        }

        // Kiểm tra lại tồn kho trước khi tạo đơn hàng
        var cart = await _cartRepository.GetCartByUserIdAsync(userGuid);
        if (cart != null)
        {
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userGuid);
            foreach (var item in cartItems)
            {
                var design = await _designService.GetDesignByIdAsync(item.DesignId);
                if (design == null || design.StockQuantity < item.Quantity)
                {
                    TempData["Error"] = $"Sản phẩm {design?.DesignName ?? "Unknown"} không đủ tồn kho. Vui lòng kiểm tra lại giỏ hàng.";
                    return RedirectToAction("CartDetail", "Cart");
                }
            }

            var (order, message) = await _orderService.CreateOrderFromCartAsync(cart.CartId, userGuid);
            if (order != null)
            {
                TempData["OrderId"] = order.OrderId.ToString();
                return RedirectToAction("Success");
            }
            else
            {
                TempData["Error"] = message;
                return RedirectToAction("CartDetail", "Cart");
            }
        }
        else
        {
            TempData["Error"] = "Giỏ hàng trống hoặc không tồn tại.";
            return RedirectToAction("CartDetail", "Cart");
        }
    }

    public IActionResult Cancel()
    {
        return View("cancel");
    }

    [HttpGet("Checkout/Success")]
    public async Task<IActionResult> Success()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Auth", new { ReturnUrl = "/Checkout/Success" });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return BadRequest("Invalid user ID format.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (!TempData.TryGetValue("OrderId", out var orderIdObj) || !Guid.TryParse(orderIdObj?.ToString(), out var orderId))
        {
            return BadRequest("Invalid order ID.");
        }

        var order = await _orderService.GetOrderByIdAsync(orderId, userGuid);
        if (order == null)
        {
            return NotFound("Order not found.");
        }

        var designIds = order.OrderItems
            .Where(oi => oi.DesignId.HasValue)
            .Select(oi => oi.DesignId.Value)
            .Distinct()
            .ToList();
        var designTasks = designIds.Select(id => _designService.GetDesignByIdAsync(id)).ToList();
        var designs = await Task.WhenAll(designTasks);
        var designDict = designs
            .Where(d => d != null)
            .ToDictionary(d => d.DesignId, d => d.DesignName ?? "Unknown");

        var model = new SuccessViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Address = user.Address,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            OrderId = order.OrderId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
            {
                OrderItemId = oi.OrderItemId,
                DesignId = oi.DesignId,
                DesignName = oi.DesignId.HasValue && designDict.ContainsKey(oi.DesignId.Value) ? designDict[oi.DesignId.Value] : "Unknown",
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList()
        };

        return View(model);
    }
}