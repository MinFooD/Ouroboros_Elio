namespace DataAccessLayer.Entities;

public partial class Design
{
    public Guid DesignId { get; set; }
    public Guid ModelId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Price { get; set; }
    public int? StockQuantity { get; set; }
    public decimal CapitalExpense { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int VisitCount { get; set; }
    public int OrderCount { get; set; }

	public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<DesignImage> DesignImages { get; set; } = new List<DesignImage>();
    public virtual Model Model { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
