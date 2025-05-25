using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly OuroborosContext _context;

		public OrderRepository(OuroborosContext context)
		{
			_context = context;
		}

		public async Task<Order?> CreateOrderFromCartAsync(Guid cartId, Guid userId)
		{

			var cart = await _context.Carts
				.Include(c => c.CartItems)
				.FirstOrDefaultAsync(c => c.CartId == cartId);

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
				return null;

			if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
				return null;

			// Tạo đơn hàng mới
			var order = new Order
			{
				OrderId = Guid.NewGuid(),
				UserId = cart.UserId,
				OrderDate = DateTime.UtcNow,
				TotalAmount = cart.Total,
				Status = "Chưa giao", 
				ShippingAddress = user.Address,
				OrderItems = cart.CartItems.Select(item => new OrderItem
				{
					OrderItemId = Guid.NewGuid(),
					ProductType = item.ProductType,
					DesignId = item.DesignId,
					CustomBraceletId = item.CustomBraceletId,
					Quantity = item.Quantity,
					Price = item.Price
				}).ToList()
			};

			await _context.Orders.AddAsync(order);

			_context.CartItems.RemoveRange(cart.CartItems);
			_context.Carts.Remove(cart);

			await _context.SaveChangesAsync();
			return order;
		}

        public async Task<Order?> GetOrderByIdAsync(Guid orderId, Guid userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Design)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }
    }
}
