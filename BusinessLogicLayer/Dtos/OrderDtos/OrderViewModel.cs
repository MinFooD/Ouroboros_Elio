using BusinessLogicLayer.Dtos.PaymentDtos;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.OrderDtos
{
	public class OrderViewModel
	{
		public Guid OrderId { get; set; }
		public Guid UserId { get; set; }
		public DateTime? OrderDate { get; set; }
		public decimal? TotalAmount { get; set; }
		public string? Status { get; set; }
		public string? CodeShipping { get; set; }
		public string? ShippingAddress { get; set; }
		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<OrderItemsViewModel> OrderItems { get; set; } = new List<OrderItemsViewModel>();
		public virtual ICollection<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();
	}
}
