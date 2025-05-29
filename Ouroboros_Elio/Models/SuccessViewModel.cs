namespace Ouroboros_Elio.Models;

public class SuccessViewModel
{
    // Thông tin người dùng
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public int PhoneNumber { get; set; }

    // Thông tin đơn hàng
    public Guid OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal? TotalAmount { get; set; }
    public string Status { get; set; }
    public List<OrderItemViewModel> OrderItems { get; set; }
}

public class OrderItemViewModel
{
    public Guid OrderItemId { get; set; }
    public Guid? DesignId { get; set; }
    public string DesignName { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
}