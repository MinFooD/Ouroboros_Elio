using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts
{
	public interface IOrderRepository
	{
		Task<Order?> CreateOrderFromCartAsync(Guid cartId, Guid userId);
        Task<Order?> GetOrderByIdAsync(Guid orderId, Guid userId);
    }
}
