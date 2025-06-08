using BusinessLogicLayer.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Dashboard(string? sort)
    {
        ViewBag.ActivePage = "Dashboard";
        ViewBag.AccessCount = await _adminService.GetAccessCount();
        ViewBag.UserCount = await _adminService.GetUserCountWithRoleUser();
        ViewBag.OrderCount = await _adminService.GetOrderCount();
        ViewBag.CustomBraceletCount = await _adminService.GetCustomBraceletCount();
        ViewBag.MonthlyRevenue = await _adminService.GetMonthlyRevenue(2025);
        var topVisitedDesigns = await _adminService.GetTopVisitedDesignsAsync(6, sort);
        return View(topVisitedDesigns);
    }

    public async Task<IActionResult> ShowOrder(int page = 1, int pageSize = 10)
    {
        ViewBag.ActivePage = "ShowOrder";
        var (orders, totalCount) = await _adminService.GetAllOrder(page, pageSize); // Assuming Guid.Empty is used to fetch all orders
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateShippingCode(Guid orderId, string shippingCode)
    {
        // Logic to update shipping code
        if (string.IsNullOrEmpty(shippingCode) || shippingCode.Contains(" "))
        {
            return Json(new { success = false, message = "Shipping code cannot be empty or contain spaces." });
        }
        var result = await _adminService.UpdateShippingCode(orderId, shippingCode);
        if (!result)
        {
            return Json(new { success = false, message = "Failed to update shipping code. Order not found or update failed." });
        }
        return Json(new { success = true, message = "Shipping code updated successfully." });
    }

    public async Task<IActionResult> ShowOrderDetail(Guid orderId)
    {
        var orderItems = await _adminService.GetAllOrderItem(orderId);
        return View(orderItems);
    }

    public async Task<IActionResult> ShowUserByRole(int pageNumber = 1, int pageSize = 10)
    {
        var users = await _adminService.GetAllUser(pageNumber, pageSize);
        var totalCount = await _adminService.GetUserCountWithRoleUser();
        ViewBag.Page = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        ViewBag.ActivePage = "ShowUserByRole";
        return View(users);
    }
}
