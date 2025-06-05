using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.CharmDtos
{
	public class CustomBraceletCharmViewModel
	{
		public Guid CustomBraceletId { get; set; }

		public Guid CharmId { get; set; }

		public int OrdinalNumber { get; set; }

		public virtual CharmViewModel Charm { get; set; } = null!;
	}
}
