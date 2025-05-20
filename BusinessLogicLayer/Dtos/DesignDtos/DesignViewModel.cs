using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.DesignDtos
{
	public class DesignViewModel
	{
		public Guid DesignId { get; set; }
		public string? DesignName { get; set; }

		public Guid ModelId { get; set; }

		public Guid CategoryId { get; set; }

		public decimal Price { get; set; }

		public decimal CapitalExpense { get; set; }

		public string? Description { get; set; }

		public int VisitCount { get; set; }
		public int OrderCount { get; set; }
		public string? CollectionName { get; set; }
		public string? CategoryName { get; set; }
		public string? TopicName { get; set; }
		public string? ModelName { get; set; }
		public virtual DesignImageViewModel? FirstImage { get; set; } = null!;
		public virtual ICollection<DesignImageViewModel> DesignImages { get; set; } = new List<DesignImageViewModel>();
	}
}
