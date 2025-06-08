using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.Dtos.OrderDtos;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.ServiceContracts;

public interface IAdminService
{
    Task<(List<OrderViewModel>, int)> GetAllOrder(int page, int pageSize);
    Task<bool> UpdateShippingCode(Guid orderId, string shippingCode);
    Task<List<OrderItemsViewModel>> GetAllOrderItem(Guid orderId);
    Task<int> GetAccessCount();
    Task<int> GetUserCountWithRoleUser();
    Task<int> GetOrderCount();
    Task<int> GetCustomBraceletCount();
    Task<decimal[]> GetMonthlyRevenue(int year);
    Task<List<DesignViewModel>> GetTopVisitedDesignsAsync(int topCount, string? sort);
    Task<List<ApplicationUser>> GetAllUser(int pageNumber, int pageSize);
}
