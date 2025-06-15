using BusinessLogicLayer.Dtos.CartDtos;
using BusinessLogicLayer.Dtos.CharmDtos;
using BusinessLogicLayer.Dtos.DesignDtos;
using DataAccessLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ouroboros_Elio.Models;

public class CheckoutViewModel
{
    // Thông tin người dùng
    [Required(ErrorMessage = "Vui lòng nhập Tên")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên phải từ 2 đến 50 ký tự")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập Họ")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Họ phải từ 2 đến 50 ký tự")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập Địa Chỉ")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số Điện Thoại")]
	[Range(100000000, 999999999, ErrorMessage = "Số điện thoại phải đúng 9 chữ số")]
	//[Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
	public int PhoneNumber { get; set; }

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
    public CustomBraceletViewModel CustomBracelet { get; set; }
}
