using BusinessLogicLayer.Dtos.DesignDtos;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.ServiceContracts;

public interface IDesignService
{
    Task<DesignViewModel?> GetDesignByIdAsync(Guid? designId);
    Task<List<DesignViewModel>?> GetAllDesignsAsync(Guid? modelId);
    Task<bool> VisitCountUp(Guid designId);
    Task<(List<DesignViewModel> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, int page, int pageSize);
}
