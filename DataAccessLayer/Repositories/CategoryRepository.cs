using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly OuroborosContext _context;

    public CategoryRepository(OuroborosContext context)
    {
        _context = context;
    }

    public async Task<List<Category>?> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }
}
