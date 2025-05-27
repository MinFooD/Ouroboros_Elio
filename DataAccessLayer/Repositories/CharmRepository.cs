using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
	public class CharmRepository : ICharmRepository
	{
		private readonly OuroborosContext _context;

		public CharmRepository(OuroborosContext context)
		{
			_context = context;
		}

		public async Task<List<Charm>> GetAllCharmsAsync()
		{
			return await _context.Charms.ToListAsync();
		}

		public async Task<Charm?> GetCharmByIdAsync(Guid charmId)
		{
			return await _context.Charms
				.FirstOrDefaultAsync(c => c.CharmId == charmId);
		}
	}
}
