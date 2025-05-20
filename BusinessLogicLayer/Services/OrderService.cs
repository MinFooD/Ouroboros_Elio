using AutoMapper;
using BusinessLogicLayer.Dtos.OrderDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
	public class OrderService : IOrderService
	{
		private readonly IMapper _mapper;
		private readonly IOrderRepository _orderRepository;

		public OrderService(IMapper mapper, IOrderRepository orderRepository)
		{
			_mapper = mapper;
			_orderRepository = orderRepository;
		}

		public async Task<OrderViewModel?> CreateOrderFromCartAsync(Guid cartId, Guid userId)
		{
			var order = await _orderRepository.CreateOrderFromCartAsync(cartId, userId);
			var orderViewModel = _mapper.Map<OrderViewModel>(order);
			return orderViewModel;
		}
	}
}
