using BusinessLogicLayer.Dtos.OrderDtos;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.ServiceContracts;

public interface IOrderService
{
    Task<(OrderViewModel? Order, string Message)> CreateOrderFromCartAsync(Guid cartId, Guid userId);
    Task<OrderViewModel?> GetOrderByIdAsync(Guid orderId, Guid userId);
    Task<(List<OrderMyAccount> Orders, int TotalCount)> GetOrdersByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
}
