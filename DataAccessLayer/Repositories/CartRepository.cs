using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class CartRepository : ICartRepository
{
    private readonly OuroborosContext _context;

    public CartRepository(OuroborosContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)>
        AddToCart(Guid userId, Guid designId, int quantity, bool productType = false)
    {
        if (quantity <= 0)
        {
            return (false, "Số lượng phải lớn hơn 0.");
        }

        //Kiểm tra sản phẩm và tồn kho
        var design = await _context.Designs
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DesignId == designId);

        if (design == null)
        {
            return (false, "Sản phẩm không tồn tại.");
        }

        if (design.StockQuantity < quantity)
        {
            return (false, $"Chỉ còn {design.StockQuantity} sản phẩm trong kho.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Total = 0,
                    CreatedAt = DateTime.UtcNow,
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.DesignId == designId);

            if (cartItem != null)
            {
                // Kiểm tra tổng số lượng (hiện tại + yêu cầu) có vượt tồn kho không
                if (design.StockQuantity < cartItem.Quantity + quantity)
                {
                    return (false, $"Chỉ còn {design.StockQuantity} sản phẩm trong kho.");
                }
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    DesignId = designId,
                    Quantity = quantity,
                    ProductType = productType,
                    Price = design.Price,
                };
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();
            await UpdateCartTotalAsync(cart.CartId);
            await transaction.CommitAsync();
            return (true, "Thêm sản phẩm vào giỏ hàng thành công.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, "Có lỗi xảy ra khi thêm sản phẩm.");
        }
    }

    public async Task<(bool? Success, string Message)> UpdateQuantity(Guid userId, Guid designId, int quantity)
    {
        var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            return (false, "Giỏ hàng không tồn tại.");
        }

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.DesignId == designId);

        if (cartItem == null)
        {
            return (false, "Sản phẩm không có trong giỏ hàng.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var design = await _context.Designs
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DesignId == designId);

            if (design == null)
            {
                return (false, "Sản phẩm không tồn tại.");
            }

            if (quantity > 0 && design.StockQuantity < quantity)
            {
                return (false, $"Chỉ còn {design.StockQuantity} sản phẩm trong kho.");
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
            await transaction.CommitAsync();
            return (true, "Cập nhật số lượng thành công.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, "Có lỗi xảy ra khi cập nhật số lượng.");
        }
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
