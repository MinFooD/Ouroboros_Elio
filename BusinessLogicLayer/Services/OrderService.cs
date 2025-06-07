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

    public async Task<OrderViewModel?> GetOrderByIdAsync(Guid orderId, Guid userId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId, userId);
        return _mapper.Map<OrderViewModel>(order);
    }

    public async Task<(List<OrderMyAccount> Orders, int TotalCount)> GetOrdersByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        var (orders, totalCount) = await _orderRepository.GetOrdersByUserIdAsync(userId, pageNumber, pageSize);
        var orderViewModels = _mapper.Map<List<OrderMyAccount>>(orders);

        // Gán DesignName cho từng OrderItem trong mỗi OrderMyAccount
        foreach (var orderViewModel in orderViewModels)
        {
            if (orderViewModel.OrderItems != null)
            {
                var originalOrder = orders.FirstOrDefault(o => o.OrderId == orderViewModel.OrderId);
                if (originalOrder != null)
                {
                    foreach (var item in orderViewModel.OrderItems)
                    {
                        var orderItem = originalOrder.OrderItems.FirstOrDefault(oi => oi.OrderItemId == item.OrderItemId);
                        if (orderItem?.Design != null)
                        {
                            var design = orderItem.Design;
                            var collectionName = design.Model?.Topic?.Collection?.CollectionName ?? "Không xác định";
                            var topicName = design.Model?.Topic?.TopicName ?? "Không xác định";
                            var modelName = design.Model?.ModelName ?? "Không xác định";
                            var categoryName = design.Category?.CategoryName ?? "Không xác định";
                            item.DesignName = $"{collectionName} - {topicName} - {modelName} - {categoryName}";
                        }
                        else
                        {
                            item.DesignName = "Không xác định";
                        }
                    }
                }
            }
        }
        return (orderViewModels, totalCount);
    }
}
