namespace BusinessLogicLayer.Dtos.OrderDtos;

public class OrderMyAccount
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public List<OrderItemMyAccount> OrderItems { get; set; }
}

public class OrderItemMyAccount
{
    public Guid OrderItemId { get; set; }
    public string ProductType { get; set; }
    public Guid? DesignId { get; set; }
    public string DesignName { get; set; }
    public Guid? CustomBraceletId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
