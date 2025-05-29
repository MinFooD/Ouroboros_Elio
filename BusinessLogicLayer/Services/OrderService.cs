using AutoMapper;
using BusinessLogicLayer.Dtos.OrderDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;

namespace BusinessLogicLayer.Services;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;

    public OrderService(IMapper mapper, IOrderRepository orderRepository)
    {
        _mapper = mapper;
        _orderRepository = orderRepository;
    }

    public async Task<(OrderViewModel? Order, string Message)> CreateOrderFromCartAsync(Guid cartId, Guid userId)
    {
        var (order, message) = await _orderRepository.CreateOrderFromCartAsync(cartId, userId);
        var orderViewModel = _mapper.Map<OrderViewModel>(order);
        return (orderViewModel, message);
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId, Guid userId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId, userId);
        return order;
    }
}
