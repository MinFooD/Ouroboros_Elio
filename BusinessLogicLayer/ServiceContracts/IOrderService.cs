using BusinessLogicLayer.Dtos.OrderDtos;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.ServiceContracts;

public interface IOrderService
{
    Task<(OrderViewModel? Order, string Message)> CreateOrderFromCartAsync(Guid cartId, Guid userId);
    Task<OrderViewModel?> GetOrderByIdAsync(Guid orderId, Guid userId);
}
