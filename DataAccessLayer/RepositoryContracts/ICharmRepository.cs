using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts
{
	public interface ICharmRepository
	{
		Task<CustomBracelet?> GetCustomBraceletByUserIdAsync(Guid? userId);
		Task<Charm?> GetCharmByIdAsync(Guid charmId);
		Task<List<Charm>> GetAllCharmsAsync();
		Task<List<CustomBraceletCharm>> GetCustomBraceletCharm(Guid? customBraceletId);
		Task<bool> CreateCustomBracelet(string braceletName, string note, List<string> charms, Guid userId);
	}
}
