using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OuroborosContext _context;

    public OrderRepository(OuroborosContext context)
    {
        _context = context;
    }

    public async Task<(Order? Order, string Message)> CreateOrderFromCartAsync(Guid cartId, Guid userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return (null, "Người dùng không tồn tại.");

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return (null, "Giỏ hàng trống hoặc không tồn tại.");

            // Kiểm tra và khóa tồn kho
            var designIds = cart.CartItems.Select(ci => ci.DesignId).ToList();
            var designs = await _context.Designs
                .Where(d => designIds.Contains(d.DesignId))
                .ToListAsync();

            // Kiểm tra tồn kho cho tất cả sản phẩm
            foreach (var item in cart.CartItems)
            {
                var design = await _context.Designs
                    .FirstOrDefaultAsync(d => d.DesignId == item.DesignId);

                if (design == null)
                    return (null, $"Sản phẩm {item.DesignId} không tồn tại.");

                if (design.StockQuantity < item.Quantity)
                    return (null, $"Sản phẩm xxx chỉ còn {design.StockQuantity} trong kho.");
            }

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

            // Giảm tồn kho
            foreach (var item in cart.CartItems)
            {
                var design = await _context.Designs
                    .FirstOrDefaultAsync(d => d.DesignId == item.DesignId);

                if (design != null)
                {
                    design.StockQuantity -= item.Quantity;
                }
            }

            // Xóa giỏ hàng
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return (order, "Tạo đơn hàng thành công.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (null, $"Có lỗi xảy ra khi tạo đơn hàng: {ex.Message}");
        }
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId, Guid userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Design)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
    }
}
