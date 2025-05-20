using BusinessLogicLayer.Dtos.DesignDtos;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.OrderDtos
{
	public class OrderItemsViewModel
	{
		public Guid OrderItemId { get; set; }

		public Guid? OrderId { get; set; }

		public bool ProductType { get; set; }

		public Guid? DesignId { get; set; }

		public Guid? CustomBraceletId { get; set; }

		public int? Quantity { get; set; }

		public decimal? Price { get; set; }

		public virtual DesignViewModel? Design { get; set; }

		public virtual OrderViewModel? Order { get; set; }
	}
}
