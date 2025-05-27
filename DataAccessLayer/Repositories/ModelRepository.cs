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
	public class ModelRepository : IModelRepository
	{
		private readonly OuroborosContext _context;
		public ModelRepository(OuroborosContext context)
		{
			_context = context;
		}
        public async Task<List<Model>?> GetAllActiveModelsAsync()
        {
            return await _context.Models
                .Where(m => m.IsActive)
                .ToListAsync();
        }
    }
}
