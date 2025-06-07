using DataAccessLayer.Entities;

namespace DataAccessLayer.RepositoryContracts;

public interface IOrderRepository
{
    Task<(Order? Order, string Message)> CreateOrderFromCartAsync(Guid cartId, Guid userId);
    Task<Order?> GetOrderByIdAsync(Guid orderId, Guid userId);
    Task<(List<Order> Orders, int TotalCount)> GetOrdersByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
}
