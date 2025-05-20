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
	public class CartRepository : ICartRepository
	{
		private readonly OuroborosContext _context;

		public CartRepository(OuroborosContext context)
		{
			_context = context;
		}

		public async Task<bool> AddToCart(Guid userId, Guid designId, int quantity, bool productType = false)
		{
			if (quantity <= 0)
			{
				return false; // không cho thêm với số lượng <= 0
			}
			var design = await _context.Designs.FirstOrDefaultAsync(d => d.DesignId == designId);
			if (design == null)
			{
				return false;
			}
			var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
			if (cart == null)
			{
				cart = new Cart
				{
					UserId = userId,
					Total = 0,
					CreatedAt = DateTime.UtcNow,
				};
				await _context.Carts.AddAsync(cart);
				await _context.SaveChangesAsync(); // Để có CartId
			}
			var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.DesignId == designId);
			if (cartItem != null)
			{
				// Sản phẩm đã có, không thêm mới
				return false;
			}
			cartItem = new CartItem
			{
				CartId = cart.CartId,
				DesignId = designId,
				Quantity = quantity,
				ProductType = productType,
				Price = design.Price,
			};
			await _context.CartItems.AddAsync(cartItem);
			await _context.SaveChangesAsync();
			await UpdateCartTotalAsync(cart.CartId);
			return true;
		}

		public async Task<bool?> UpdateQuantity(Guid userId, Guid designId, int quantity)
		{
			var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
			if (cart == null)
			{
				return false;
			}
			var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.DesignId == designId);
			if (cartItem == null)
			{
				return false;
			}
			if (quantity <= 0)
			{
				_context.CartItems.Remove(cartItem);
			}
			else
			{
				cartItem.Quantity = quantity;
			}
			await _context.SaveChangesAsync();
			await UpdateCartTotalAsync(cart.CartId);
			return true;
		}

		private async Task UpdateCartTotalAsync(Guid cartId)
		{
			var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartId);
			if (cart == null) return;
			cart.Total = await _context.CartItems
				.Where(ci => ci.CartId == cartId)
				.SumAsync(ci => (ci.Quantity ?? 0) * (ci.Price ?? 0));

			await _context.SaveChangesAsync();
		}

		public async Task<List<CartItem>?> GetCartItemsByUserIdAsync(Guid userId)
		{
			var cart = await _context.Carts
				.FirstOrDefaultAsync(c => c.UserId == userId);
			if (cart == null)
			{
				return null; // Không có giỏ hàng thì trả về danh sách rỗng
			}

			var cartItems = await _context.CartItems
				.Where(ci => ci.CartId == cart.CartId)
				.Include(ci => ci.Design).ThenInclude(ci => ci.DesignImages) // Load thêm thông tin Design nếu cần
				.Include(ci => ci.CustomBracelet) // Load thêm thông tin CustomBracelet nếu dùng
				.ToListAsync();

			return cartItems;
		}

		public Task<Cart?> GetCartByUserIdAsync(Guid userId)
		{
			var cart = _context.Carts
				.Include(c => c.CartItems)
				.FirstOrDefaultAsync(c => c.UserId == userId);
			return cart;
		}
	}
}
