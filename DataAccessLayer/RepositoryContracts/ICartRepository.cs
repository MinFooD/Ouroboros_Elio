using DataAccessLayer.Entities;

namespace DataAccessLayer.RepositoryContracts;

public interface ICartRepository
{
    Task<Cart?> GetCartByUserIdAsync(Guid userId);
    Task<List<CartItem>?> GetCartItemsByUserIdAsync(Guid userId);
    Task<(bool Success, string Message)> AddToCart(Guid userId, Guid designId, int quantity, bool productType);
    Task<(bool? Success, string Message)> UpdateQuantity(Guid userId, Guid designId, int quantity);
}
