using BusinessLogicLayer.Dtos.DesignDtos;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.ServiceContracts;

public interface IDesignService
{
    Task<DesignViewModel?> GetDesignByIdAsync(Guid? designId);
    Task<List<DesignViewModel>?> GetAllDesignsAsync(Guid? modelId);
    Task<bool> VisitCountUp(Guid designId);
    Task<(List<DesignViewModel> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, int page, int pageSize, string? sortBy = null);
    Task<(List<DesignViewModel> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, decimal? minPrice, decimal? maxPrice, int page, int pageSize, string? sortBy = null);
    Task<List<DesignViewModel>?> GetDesignsByCategoryAsync(Guid categoryId);
    Task<List<DesignViewModel>> GetTopOrderedDesignsAsync(int topCount);
}
