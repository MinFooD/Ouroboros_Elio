using BusinessLogicLayer.Dtos.DesignDtos;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.CartDtos
{
	public class CartItemViewModel
	{
		public Guid CartItemId { get; set; }

		public Guid? CartId { get; set; }

		public bool ProductType { get; set; }

		public Guid? DesignId { get; set; }

		public Guid? CustomBraceletId { get; set; }

		public int? Quantity { get; set; }

		public decimal? Price { get; set; }

		public virtual DesignViewModel? Design { get; set; }
	}
}
