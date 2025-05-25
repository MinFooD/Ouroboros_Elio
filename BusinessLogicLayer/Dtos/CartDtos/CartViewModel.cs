using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.CartDtos
{
	public class CartViewModel
	{
		public Guid CartId { get; set; }
		public Guid UserId { get; set; }
		public DateTime? CreatedAt { get; set; }
		public decimal Total { get; set; }
	}
}
