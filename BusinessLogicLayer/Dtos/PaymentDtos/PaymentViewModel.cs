using BusinessLogicLayer.Dtos.OrderDtos;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.PaymentDtos
{
	public class PaymentViewModel
	{
		public Guid PaymentId { get; set; }

		public Guid? OrderId { get; set; }

		public DateTime? PaymentDate { get; set; }

		public decimal? Amount { get; set; }

		public string? PaymentMethod { get; set; }

		public string? Status { get; set; }

		public virtual OrderViewModel? Order { get; set; }
	}
}
