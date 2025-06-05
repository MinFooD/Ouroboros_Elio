using BusinessLogicLayer.Dtos.CharmDtos;
using DataAccessLayer.Entities;
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
		Task<CharmViewModel[]> GetArrayCharmsAsync();
		Task<CustomBraceletViewModel?> GetCustomBraceletByIdAsync(Guid? customBraceletId);
		Task<List<CustomBraceletCharmViewModel>> GetCustomBraceletCharm(Guid? customBraceletId);
		Task<(bool? Success, string Message)> CreateCustomBracelet(string braceletName, string note, List<string> charms, Guid userId);
	}
}
