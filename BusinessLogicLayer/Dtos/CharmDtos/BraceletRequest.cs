using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.CharmDtos
{
	public class BraceletRequest
	{
		public string? Note { get; set; }
		public List<string?> CharmIds { get; set; } = new();
	}
}
