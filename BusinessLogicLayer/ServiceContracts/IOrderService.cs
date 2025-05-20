using BusinessLogicLayer.Dtos.OrderDtos;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServiceContracts
{
	public interface IOrderService
	{
		Task<OrderViewModel?> CreateOrderFromCartAsync(Guid cartId, Guid userId);
	}
}
