using BusinessLogicLayer.Dtos.CharmDtos;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Errors;
using Net.payOS.Types;
using Ouroboros_Elio.Models;
using System.Linq;
using System.Security.Claims;

namespace Ouroboros_Elio.Controllers;

public class CheckoutController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICartRepository _cartRepository;
    private readonly IOrderService _orderService;
    private readonly IDesignService _designService;
    private readonly ICharmService _charmService;
    private readonly PayOS _payOS;

    public CheckoutController(
        UserManager<ApplicationUser> userManager,
        ICartRepository cartRepository,
        IOrderService orderService,
        IDesignService designService,
        IConfiguration configuration,
        ICharmService charmService)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _orderService = orderService;
        _designService = designService;
        _payOS = new PayOS(
            configuration["PayOS:ClientId"] ?? throw new Exception("PayOS ClientId not found"),
            configuration["PayOS:ApiKey"] ?? throw new Exception("PayOS ApiKey not found"),
            configuration["PayOS:ChecksumKey"] ?? throw new Exception("PayOS ChecksumKey not found")
        );
        _charmService = charmService;
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

        var (model, redirectResult) = await PrepareCheckoutViewModel(userGuid, user);
        if (redirectResult != null)
        {
            return redirectResult;
        }

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

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Cập nhật thông tin người dùng
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

        // Kiểm tra giỏ hàng và tồn kho
        var cart = await _cartRepository.GetCartByUserIdAsync(userGuid);
        if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng trống hoặc không tồn tại.";
            return RedirectToAction("CartDetail", "Cart");
        }

        var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userGuid);
        // Lấy tất cả DesignViewModel cho các DesignId trong giỏ hàng
        var designIds = cartItems.Select(item => item.DesignId).Distinct().ToList();
        var customBraceletIds = cartItems.Select(item => item.CustomBraceletId).Distinct().ToList();

        var designs = await _designService.GetDesignsByIdsAsync(designIds);
        var designDict = designs.ToDictionary(d => d.DesignId, d => d);

        //var customBracelets = await _charmService.GetCustomBraceletCharm(customBaceletIds);
        var customBraceletList = new List<CustomBraceletViewModel>();
        foreach (var customBraceletId in customBraceletIds)
        {
            var customBracelet = await _charmService.GetCustomBraceletByIdAsync(customBraceletId);
            if (customBracelet != null)
            {
                customBraceletList.Add(customBracelet);
            }
        }
        var customBraceletDict = customBraceletList.ToDictionary(c => c.CustomBraceletId, c => c);

        //foreach (var item in cartItems)
        //{
        //    // Kiểm tra nếu item.DesignId là null
        //    if (!item.DesignId.HasValue)
        //    {
        //        TempData["Error"] = "Có sản phẩm trong giỏ hàng không có thông tin thiết kế hợp lệ.";
        //        return RedirectToAction("CartDetail", "Cart");
        //    }

        //    if (!designDict.ContainsKey(item.DesignId.Value) || designDict[item.DesignId.Value].StockQuantity < item.Quantity)
        //    {
        //        TempData["Error"] = $"Sản phẩm {designDict.GetValueOrDefault(item.DesignId.Value)?.DesignName ?? "Unknown"} không đủ tồn kho. Vui lòng kiểm tra lại giỏ hàng.";
        //        return RedirectToAction("CartDetail", "Cart");
        //    }
        //}

        // Tạo orderCode duy nhất dựa trên timestamp và random
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random().Next(1000, 9999);
        long orderCode = long.Parse($"{timestamp % 100000}{random}"); // 5 chữ số timestamp + 4 chữ số random

        // Tạo link thanh toán PayOS
        var items = cartItems.Select(item =>
        {
            string itemName;
            if (item.DesignId.HasValue)
            {
                var design = designDict.GetValueOrDefault(item.DesignId.Value);
                itemName = design != null
                ? $"{design.CollectionName}-{design.TopicName}-{design.ModelName}-{design.CategoryName}"
                : "Sản phẩm không xác định";
            }
            else
            {
                var customBracelet = item.CustomBraceletId.HasValue
                ? customBraceletDict.GetValueOrDefault(item.CustomBraceletId.Value)
                : null;
                itemName = customBracelet != null
                ? customBracelet.CustomBraceletName ?? "Custom Bracelet"
                : "Sản phẩm không xác định";
            }
            return new ItemData(
            name: itemName,
            quantity: item.Quantity,
            price: (int)item.Price
        );
        }).ToList();

        var paymentData = new PaymentData(
            orderCode: orderCode,
            amount: (int)cart.Total,
            description: "Thanh toán giỏ hàng",
            items: items,
            returnUrl: Url.Action("Success", "Checkout", null, Request.Scheme),
            cancelUrl: Url.Action("Cancel", "Checkout", null, Request.Scheme)
        );

        // Kiểm tra trạng thái link thanh toán cũ
        try
        {
            var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);
            if (paymentInfo != null && paymentInfo.status == "PENDING")
            {
                // Tạo lại link thanh toán với cùng orderCode để lấy checkoutUrl
                var response = await _payOS.createPaymentLink(paymentData);
                TempData["CartId"] = cart.CartId.ToString();
                return Redirect(response.checkoutUrl);
            }
        }
        catch
        {
            // Nếu không tìm thấy link hoặc lỗi khác, tiếp tục tạo link mới
        }


        try
        {
            var response = await _payOS.createPaymentLink(paymentData);
            TempData["CartId"] = cart.CartId.ToString(); // Lưu CartId để sử dụng trong Success
            return Redirect(response.checkoutUrl);
        }
        catch (PayOSError ex) when (ex.Message.Contains("Đơn thanh toán đã tồn tại"))
        {
            // Thử lại với orderCode mới
            orderCode = int.Parse($"{(timestamp + 1) % 1000000}{random + 1}");
            paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)cart.Total,
                description: "Thanh toán giỏ hàng",
                items: items,
                returnUrl: Url.Action("Success", "Checkout", null, Request.Scheme),
                cancelUrl: Url.Action("Cancel", "Checkout", null, Request.Scheme)
            );

            try
            {
                var response = await _payOS.createPaymentLink(paymentData);
                TempData["CartId"] = cart.CartId.ToString();
                return Redirect(response.checkoutUrl);
            }
            catch (Exception ex2)
            {
                TempData["Error"] = $"Lỗi khi tạo link thanh toán: {ex2.Message}";
                return RedirectToAction("CartDetail", "Cart");
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi tạo link thanh toán: {ex.Message}";
            return RedirectToAction("CartDetail", "Cart");
        }
    }

    public IActionResult Cancel()
    {
        return View("cancel");
    }

    [HttpGet("Checkout/Success")]
    public async Task<IActionResult> Success(long? orderCode)
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

        // Lấy CartId từ TempData
        if (!TempData.TryGetValue("CartId", out var cartIdObj) || !Guid.TryParse(cartIdObj?.ToString(), out var cartId))
        {
            TempData["Error"] = "Không tìm thấy thông tin giỏ hàng.";
            return RedirectToAction("CartDetail", "Cart");
        }

        // Kiểm tra trạng thái thanh toán qua PayOS
        if (!orderCode.HasValue)
        {
            TempData["Error"] = "Mã đơn thanh toán không hợp lệ.";
            return RedirectToAction("CartDetail", "Cart");
        }

        try
        {
            var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode.Value);
            if (paymentInfo == null || paymentInfo.status != "PAID")
            {
                TempData["Error"] = "Thanh toán chưa được xác nhận hoặc không thành công.";
                return RedirectToAction("CartDetail", "Cart");
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi kiểm tra trạng thái thanh toán: {ex.Message}";
            return RedirectToAction("PlaceOrder", "Checkout");
        }

        // Tạo đơn hàng từ giỏ hàng
        var (orderViewModel, message) = await _orderService.CreateOrderFromCartAsync(cartId, userGuid);
        if (orderViewModel == null)
        {
            TempData["Error"] = message;
            return RedirectToAction("CartDetail", "Cart");
        }

        // Lưu OrderId vào TempData để hiển thị thông tin đơn hàng
        TempData["OrderId"] = orderViewModel.OrderId.ToString();

        // Lấy thông tin thiết kế cho các sản phẩm trong đơn hàng
        var designIds = orderViewModel.OrderItems
            .Where(oi => oi.DesignId.HasValue)
            .Select(oi => oi.DesignId)
            .Distinct()
            .ToList();
        var designs = await _designService.GetDesignsByIdsAsync(designIds);
        var designDict = designs.ToDictionary(d => d.DesignId, d => d.DesignName ?? "Unknown");

        // Chuẩn bị model cho view
        var model = new SuccessViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Address = user.Address,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            OrderId = orderViewModel.OrderId,
            OrderDate = orderViewModel.OrderDate,
            TotalAmount = orderViewModel.TotalAmount,
            Status = orderViewModel.Status,
            OrderItems = orderViewModel.OrderItems.Select(oi => new OrderItemViewModel
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

    private async Task<(CheckoutViewModel, IActionResult?)> PrepareCheckoutViewModel(Guid userGuid, ApplicationUser user)
    {
        var userCart = await _cartRepository.GetCartByUserIdAsync(userGuid);
        var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userGuid);
        var cartItemViewModels = new List<CartItemViewModel>();

        if (cartItems != null)
        {
            foreach (var item in cartItems)
            {
                if (item.DesignId.HasValue)
                {
                    var designViewModel = await _designService.GetDesignByIdAsync(item.DesignId.Value);
                    if (designViewModel != null)
                    {
                        if (designViewModel.StockQuantity < item.Quantity)
                        {
                            await _cartRepository.UpdateQuantity(userGuid, item.DesignId.Value, designViewModel.StockQuantity, false);
                            TempData["StockWarning"] = $"Số lượng sản phẩm {designViewModel.DesignName} đã được điều chỉnh còn {designViewModel.StockQuantity} do tồn kho thay đổi.";
                            return (null, RedirectToAction("CartDetail", "Cart"));
                        }
                        if (designViewModel.StockQuantity == 0)
                        {
                            await _cartRepository.UpdateQuantity(userGuid, item.DesignId.Value, 0, false);
                            TempData["StockWarning"] = $"Sản phẩm {designViewModel.DesignName} đã hết hàng và được xóa khỏi giỏ hàng.";
                            return (null, RedirectToAction("CartDetail", "Cart"));
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
                else
                {
                    var customBracelet = await _charmService.GetCustomBraceletByIdAsync(item.CustomBraceletId.Value);
                    cartItemViewModels.Add(new CartItemViewModel
                    {
                        DesignId = item.DesignId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        CustomBracelet = customBracelet
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

        return (model, null);
    }
}