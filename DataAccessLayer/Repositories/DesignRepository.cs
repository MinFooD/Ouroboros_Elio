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
	public class DesignRepository : IDesignRepository
	{
		private readonly OuroborosContext _context;

		public DesignRepository(OuroborosContext context)
		{
			_context = context;
		}

		public async Task<List<Design>?> GetAllDesignsAsync()
		{
			return await _context.Designs
				.Include(d => d.Category)
				.Include(d => d.DesignImages)
				.Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
				.ToListAsync();
		}

		public async Task<Design?> GetDesignByIdAsync(Guid designId)
		{
			return await _context.Designs
				.Include(d => d.Category)
				.Include(d => d.DesignImages)
				.Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
				.FirstOrDefaultAsync(d => d.DesignId == designId);
		}
	}
}
