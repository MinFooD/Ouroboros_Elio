using BusinessLogicLayer.Dtos.CartDtos;
using BusinessLogicLayer.Dtos.DesignDtos;
using DataAccessLayer.Entities;

namespace Ouroboros_Elio.Models;

public class CheckoutViewModel
{
    // Thông tin người dùng
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    // Thông tin giỏ hàng
    public List<CartItemViewModel> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CartItemViewModel
{
    public Guid? DesignId { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
    public DesignViewModel Design { get; set; }
}
