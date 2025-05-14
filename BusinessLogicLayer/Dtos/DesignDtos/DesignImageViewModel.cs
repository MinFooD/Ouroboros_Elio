using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.DesignDtos
{
	public class DesignImageViewModel
	{
		public Guid ImageId { get; set; }

		public Guid DesignId { get; set; }

		public string ImageUrl { get; set; } = null!;

		public virtual Design Design { get; set; } = null!;
	}
}
