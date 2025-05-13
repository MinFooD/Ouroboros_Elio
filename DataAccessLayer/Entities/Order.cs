using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities;
public partial class Order
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Status { get; set; }
    public string? CodeShipping { get; set; }
    public string? ShippingAddress { get; set; }
	public virtual ApplicationUser User { get; set; }
	public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
