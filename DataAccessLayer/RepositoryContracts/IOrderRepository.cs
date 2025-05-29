using DataAccessLayer.Entities;

namespace DataAccessLayer.RepositoryContracts;

public interface IOrderRepository
{
    Task<(Order? Order, string Message)> CreateOrderFromCartAsync(Guid cartId, Guid userId);
    Task<Order?> GetOrderByIdAsync(Guid orderId, Guid userId);
}
