using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts
{
	public interface IDesignRepository
	{
		Task<Design?> GetDesignByIdAsync(Guid? designId);
		Task<List<Design>?> GetAllDesignsAsync(Guid? modelId);
		Task<bool> VisitCountUp(Guid designId);
        Task<(List<Design> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, int page, int pageSize, string? sortBy = null);
        Task<(List<Design> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, decimal? minPrice, decimal? maxPrice, int page, int pageSize, string? sortBy = null);
        Task<List<Design>> GetTopOrderedDesignsAsync(int topCount);
    }
}
