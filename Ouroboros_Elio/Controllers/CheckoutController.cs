﻿using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ouroboros_Elio.Models;
using System.Security.Claims;

namespace Ouroboros_Elio.Controllers
{
	public class CheckoutController : Controller
	{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IDesignService _designService;

        public CheckoutController(
            UserManager<ApplicationUser> userManager,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IDesignService designService)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
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
                    // Lấy lại giỏ hàng
                    var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userGuid);
                    var userCart = await _cartRepository.GetCartByUserIdAsync(userGuid);
                    var cartItemViewModels = new List<CartItemViewModel>();
                    if (cartItems != null)
                    {
                        foreach (var item in cartItems)
                        {
                            var designViewModel = await _designService.GetDesignByIdAsync(item.DesignId);
                            if (designViewModel != null)
                            {
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
                    model.CartItems = cartItemViewModels;
                    model.TotalAmount = userCart?.Total ?? 0;
                    return View(model);
                }
            }
            else
            {
                return NotFound("User not found.");
            }

            // Tạo đơn hàng từ giỏ hàng
            var cart = await _cartRepository.GetCartByUserIdAsync(userGuid);
            if (cart != null)
            {
                var order = await _orderRepository.CreateOrderFromCartAsync(cart.CartId, userGuid);
                if (order != null)
                {
                    TempData["OrderId"] = order.OrderId.ToString();
                    return RedirectToAction("Success");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể tạo đơn hàng. Vui lòng kiểm tra giỏ hàng.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Giỏ hàng trống hoặc không tồn tại.");
            }

            // Nếu có lỗi, lấy lại giỏ hàng
            var errorCartItems = await _cartRepository.GetCartItemsByUserIdAsync(userGuid);
            var errorCart = await _cartRepository.GetCartByUserIdAsync(userGuid);
            var errorCartItemViewModels = new List<CartItemViewModel>();
            if (errorCartItems != null)
            {
                foreach (var item in errorCartItems)
                {
                    var designViewModel = await _designService.GetDesignByIdAsync(item.DesignId);
                    if (designViewModel != null)
                    {
                        errorCartItemViewModels.Add(new CartItemViewModel
                        {
                            DesignId = item.DesignId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Design = designViewModel
                        });
                    }
                }
            }
            model.CartItems = errorCartItemViewModels;
            model.TotalAmount = errorCart?.Total ?? 0;
            return View(model);
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

            // Lấy OrderId từ TempData
            if (!TempData.TryGetValue("OrderId", out var orderIdObj) || !Guid.TryParse(orderIdObj?.ToString(), out var orderId))
            {
                return BadRequest("Invalid order ID.");
            }

            // Lấy thông tin đơn hàng
            var order = await _orderRepository.GetOrderByIdAsync(orderId, userGuid);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            // Tải trước tất cả DesignViewModel
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

            // Tạo SuccessViewModel
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
}
