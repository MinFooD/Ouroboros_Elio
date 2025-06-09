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
	public class CustomBraceletRepository : ICustomBraceletRepository
	{
		private readonly OuroborosContext _context;

		public CustomBraceletRepository(OuroborosContext context)
		{
			_context = context;
		}

		public async Task<bool> DeleteCustomBracelet(Guid customBraceletId)
		{
			var exitingCustomBracelet = await _context.CustomBracelets
				.Include(cb => cb.CustomBraceletCharms)
				.FirstOrDefaultAsync(cb => cb.CustomBraceletId == customBraceletId);
			if (exitingCustomBracelet == null)
			{
				return false;
			}
			// Xóa các Charm liên kết với CustomBracelet
			_context.CustomBraceletCharms.RemoveRange(exitingCustomBracelet.CustomBraceletCharms);
			_context.CustomBracelets.Remove(exitingCustomBracelet);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				// Log the exception or handle it as needed
				Console.WriteLine($"Error deleting custom bracelet: {ex.Message}");
				return false;
			}
			return true;
		}
		public async Task<CustomBracelet?> GetCustomBraceletByIdAsync(Guid? customBraceletId)
		{
			return await _context.CustomBracelets
				.Include(cb => cb.CustomBraceletCharms)
				.FirstOrDefaultAsync(cb => cb.CustomBraceletId == customBraceletId);
		}
	}
}
