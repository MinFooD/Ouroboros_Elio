using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts
{
	public interface ICartRepository
	{
		Task<Cart?> GetCartByUserIdAsync(Guid userId);
		Task<List<CartItem>?> GetCartItemsByUserIdAsync(Guid userId);
		Task<bool> AddToCart(Guid userId, Guid designId, int quantity, bool productType);
		Task<bool?> UpdateQuantity(Guid userId, Guid designId, int quantity);
	}
}
