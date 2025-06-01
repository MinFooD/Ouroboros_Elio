using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class DesignRepository : IDesignRepository
{
    private readonly OuroborosContext _context;

    public DesignRepository(OuroborosContext context)
    {
        _context = context;
    }

    public async Task<(List<Design> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, int page, int pageSize)
    {
        var query = _context.Designs
            .Include(d => d.Category)
            .Include(d => d.DesignImages)
            .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .AsQueryable();

        if (modelId != null)
        {
            query = query.Where(d => d.ModelId == modelId);
        }

        int totalCount = await query.CountAsync();
        var designs = await query
            .OrderBy(d => d.DesignId) // Sắp xếp theo DesignId để đảm bảo thứ tự nhất quán
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (designs, totalCount);
    }

    public async Task<(List<Design> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, decimal? minPrice, decimal? maxPrice, int page, int pageSize)
    {
        var query = _context.Designs
            .Include(d => d.Category)
            .Include(d => d.DesignImages)
            .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .AsQueryable();

        if (modelId != null)
        {
            query = query.Where(d => d.ModelId == modelId);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(d => d.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(d => d.Price <= maxPrice.Value);
        }

        int totalCount = await query.CountAsync();
        var designs = await query
            .OrderBy(d => d.DesignId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (designs, totalCount);
    }

    public async Task<List<Design>?> GetAllDesignsAsync(Guid? modelId)
    {
        if (modelId != null)
        {
            return await _context.Designs
                .Include(d => d.Category)
                .Include(d => d.DesignImages)
                .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
                .Where(d => d.ModelId == modelId)
                .ToListAsync();
        }
        return await _context.Designs
            .Include(d => d.Category)
            .Include(d => d.DesignImages)
            .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .ToListAsync();
    }

    public async Task<bool> VisitCountUp(Guid designId)
    {
        var design = await _context.Designs.FirstOrDefaultAsync(d => d.DesignId == designId);
        if (design != null)
        {
            design.VisitCount += 1;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<Design?> GetDesignByIdAsync(Guid? designId)
    {
        return await _context.Designs
            .Include(d => d.Category)
            .Include(d => d.DesignImages)
            .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .FirstOrDefaultAsync(d => d.DesignId == designId);
    }

    public async Task<List<Design>> GetTopOrderedDesignsAsync(int topCount)
    {
        return await _context.Designs
            .Include(d => d.Category)
            .Include(d => d.DesignImages)
            .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .OrderByDescending(d => d.OrderCount)
            .Take(topCount)
            .ToListAsync();
    }
}
