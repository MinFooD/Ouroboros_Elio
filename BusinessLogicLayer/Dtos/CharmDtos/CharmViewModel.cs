using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.CharmDtos
{
	public class CharmViewModel
	{
		public Guid CharmId { get; set; }

		public string? Name { get; set; }

		public decimal Price { get; set; }

		public decimal? CapitalExpense { get; set; }

		public string? ImageUrl { get; set; }

		public bool IsActive { get; set; }
	}
}
