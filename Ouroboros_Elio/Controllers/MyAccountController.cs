using BusinessLogicLayer.Dtos.CharmDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ouroboros_Elio.Models;
using System.Security.Claims;

namespace Ouroboros_Elio.Controllers;

public class MyAccountController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IDesignService _designService;
    private readonly ICharmService _charmService;
    private readonly UserManager<ApplicationUser> _userManager;

    public MyAccountController(
        IOrderService orderService,
        IDesignService designService,
        ICharmService charmService,
        UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _designService = designService;
        _charmService = charmService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account");
        }

        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        var (orders, totalCount) = await _orderService.GetOrdersByUserIdAsync(userId, pageNumber, pageSize);

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(Guid orderId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new { ReturnUrl = $"/MyAccount/OrderDetails?orderId={orderId}" });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return BadRequest("Định dạng ID người dùng không hợp lệ.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("Không tìm thấy người dùng.");
        }

        var orderViewModel = await _orderService.GetOrderByIdAsync(orderId, userGuid);
        if (orderViewModel == null)
        {
            return NotFound("Đơn hàng không tồn tại hoặc bạn không có quyền xem.");
        }

        // Lấy thông tin thiết kế cho các sản phẩm trong đơn hàng
        var designIds = orderViewModel.OrderItems
            .Where(oi => oi.DesignId.HasValue)
            .Select(oi => oi.DesignId)
            .Distinct()
            .ToList();
        var designs = await _designService.GetDesignsByIdsAsync(designIds);
        var designDict = designs.ToDictionary(d => d.DesignId, d => d.DesignName ?? "Unknown");

        // Lấy thông tin vòng tay tùy chỉnh
        var customBraceletIds = orderViewModel.OrderItems
            .Where(oi => oi.CustomBraceletId.HasValue)
            .Select(oi => oi.CustomBraceletId)
            .Distinct()
            .ToList();
        var customBracelets = new List<CustomBraceletViewModel>();
        foreach (var customBraceletId in customBraceletIds)
        {
            var customBracelet = await _charmService.GetCustomBraceletByIdAsync(customBraceletId);
            if (customBracelet != null)
            {
                customBracelets.Add(customBracelet);
            }
        }
        var customBraceletDict = customBracelets.ToDictionary(c => c.CustomBraceletId, c => c);



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
                CustomBraceletId = oi.CustomBraceletId,
                DesignName = oi.DesignId.HasValue && designDict.ContainsKey(oi.DesignId.Value)
                    ? designDict[oi.DesignId.Value]
                    : (oi.CustomBraceletId.HasValue && customBraceletDict.ContainsKey(oi.CustomBraceletId.Value)
                        ? customBraceletDict[oi.CustomBraceletId.Value].CustomBraceletName ?? "Vòng tay tùy chỉnh"
                        : "Unknown"),
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList()
        };

        return View(model);
    }
}
