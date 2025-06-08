using DataAccessLayer.Entities;

namespace DataAccessLayer.RepositoryContracts;

public interface IAdminRepository
{
    Task<(List<Order>, int)> GetAllOrder(int page, int pageSize);
    Task<bool> UpdateShippingCode(Guid orderId, string shippingCode);
    Task<List<OrderItem>> GetAllOrderItem(Guid orderId);
    Task<int> GetAccessCount();
    Task<int> GetUserCountWithRoleUser();
    Task<int> GetOrderCount();
    Task<int> GetCustomBraceletCount();
    Task<decimal[]> GetMonthlyRevenue(int year);
    Task<List<Design>> GetTopVisitedDesignsAsync(int topCount, string? sort);
    Task<List<ApplicationUser>> GetAllUser(int pageNumber, int pageSize);
    Task<List<ContactMessage>> GetContactMessages(int pageNumber, int pageSize);
}
