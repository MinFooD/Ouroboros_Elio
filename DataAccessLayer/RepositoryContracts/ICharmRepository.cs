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
		Task<Charm?> GetCharmByIdAsync(Guid charmId);
		Task<List<Charm>> GetAllCharmsAsync();
	}
}
