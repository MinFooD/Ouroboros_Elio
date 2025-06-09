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

            // Kiểm tra tồn kho
            foreach (var item in cart.CartItems)
            {
                if (item.ProductType == false)
                {
					var design = designs.FirstOrDefault(d => d.DesignId == item.DesignId);
					if (design == null)
						return (null, $"Sản phẩm {item.DesignId} không tồn tại.");
					if (design.StockQuantity < item.Quantity)
						return (null, $"Sản phẩm xxx chỉ còn {design.StockQuantity} trong kho.");
				}
            }

            // Tạo đơn hàng mới
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = cart.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.Total,
                Status = "Chưa giao hàng",
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

            // Giảm tồn kho với optimistic locking
            foreach (var item in cart.CartItems)
            {
                var design = designs.FirstOrDefault(d => d.DesignId == item.DesignId);
                if (design != null)
                {
                    var updatedRows = await _context.Designs
                        .Where(d => d.DesignId == item.DesignId && d.StockQuantity >= item.Quantity)
                        .ExecuteUpdateAsync(s => s.SetProperty(d => d.StockQuantity, d => d.StockQuantity - item.Quantity));

                    if (updatedRows == 0)
                    {
                        await transaction.RollbackAsync();
                        return (null, $"Sản phẩm xxx đã hết hàng trong lúc xử lý đơn hàng.");
                    }
                }
                else
                {
                    var customBracelet = await _context.CustomBracelets.Include(cb => cb.CustomBraceletCharms).ThenInclude(cbc => cbc.Charm)
						.FirstOrDefaultAsync(cb => cb.CustomBraceletId == item.CustomBraceletId);
                    var charms = customBracelet?.CustomBraceletCharms.Select(cbc => cbc.Charm).ToList();
                    if(charms != null && charms.Any())
                    {
						foreach (var charm in charms)
						{
							if (charm.Quantity < item.Quantity)
							{
								await transaction.RollbackAsync();
								return (null, $"Charm {charm.Name} chỉ còn {charm.Quantity} trong kho.");
							}
							charm.Quantity -= item.Quantity;
						}
					}
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

    public async Task<(List<Order> Orders, int TotalCount)> GetOrdersByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var orders = await query
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Design)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }
}