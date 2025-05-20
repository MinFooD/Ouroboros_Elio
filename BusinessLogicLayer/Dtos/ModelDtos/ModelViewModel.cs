using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.ModelDtos
{
	public class ModelViewModel
	{
		public Guid ModelId { get; set; }

		public Guid TopicId { get; set; }

		public string ModelName { get; set; } = null!;

		public string? Description { get; set; }

		public bool IsActive { get; set; }
	}
}
