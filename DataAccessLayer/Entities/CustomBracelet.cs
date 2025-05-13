using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities;

public partial class CustomBracelet
{
    public Guid CustomBraceletId { get; set; }

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public string? CustomBraceletName { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal? TotalCapitalExpense { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
	public virtual ApplicationUser User { get; set; }
	public virtual Category Category { get; set; } = null!;

    public virtual ICollection<CustomBraceletCharm> CustomBraceletCharms { get; set; } = new List<CustomBraceletCharm>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
