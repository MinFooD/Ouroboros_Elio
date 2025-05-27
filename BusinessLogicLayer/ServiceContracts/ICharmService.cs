using BusinessLogicLayer.Dtos.CharmDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServiceContracts
{
	public interface ICharmService
	{
		Task<CharmViewModel?> GetCharmByIdAsync(Guid charmId);
		Task<List<CharmViewModel>> GetAllCharmsAsync();
	}
}
