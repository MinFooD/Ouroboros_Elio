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
	public class CharmRepository : ICharmRepository
	{
		private readonly OuroborosContext _context;
		private readonly ICartRepository _cartRepository;

		public CharmRepository(OuroborosContext context, ICartRepository cartRepository)
		{
			_context = context;
			_cartRepository = cartRepository;
		}

		public async Task<bool> CreateCustomBracelet(string braceletName, string note, List<string> charms, Guid userId)
		{
			try
			{
				var customBracelet = new CustomBracelet
				{
					CategoryId = Guid.Parse("CAC172D4-240C-47F2-9889-8282977A282C"),
					CustomBraceletName = braceletName,
					Note = note,
					UserId = userId,
					CreatedAt = DateTime.UtcNow,
					TotalPrice = 0,
					TotalCapitalExpense = 0
				};
				await _context.CustomBracelets.AddAsync(customBracelet);
				await _context.SaveChangesAsync();
				decimal totalPrice = 15000;
				decimal? totalCapitalExpense = 0;
				// Duyệt qua danh sách charm theo thứ tự vị trí
				for (int i = 0; i < charms.Count; i++)
				{
					if (!string.IsNullOrWhiteSpace(charms[i]))
					{
						// Parse string sang Guid
						if (Guid.TryParse(charms[i], out Guid charmId))
						{
							// Lấy thông tin charm từ database
							var charm = await this.GetCharmByIdAsync(charmId);
							if (charm != null)
							{
								// Thêm charm vào vòng tay với vị trí tương ứng
								var customBraceletCharm = new CustomBraceletCharm
								{
									CustomBraceletId = customBracelet.CustomBraceletId,
									CharmId = charmId,
									OrdinalNumber = i + 1, // OrdinalNumber bắt đầu từ 1
								};
								await _context.CustomBraceletCharms.AddAsync(customBraceletCharm);
								// Cộng tổng giá
								totalPrice += charm.Price;
								totalCapitalExpense += charm.CapitalExpense;
							}
						}
					}
				}
				// Cập nhật tổng giá vòng tay
				customBracelet.TotalPrice = totalPrice;
				customBracelet.TotalCapitalExpense = totalCapitalExpense;
				// Lưu thay đổi
				await _context.SaveChangesAsync();
				await _cartRepository.AddToCart(userId, customBracelet.CustomBraceletId, 1, true);

				return true;
			}
			catch (Exception ex)
			{
				// Ghi log nếu cần
				Console.WriteLine("Lỗi khi tạo vòng tay tuỳ chỉnh: " + ex.Message);
				return false;
			}
		}

		

		public async Task<List<Charm>> GetAllCharmsAsync()
		{
			return await _context.Charms.Where(x => x.Quantity > 0 && x.IsActive == true).ToListAsync();
		}

		public async Task<Charm?> GetCharmByIdAsync(Guid charmId)
		{
			return await _context.Charms
				.FirstOrDefaultAsync(c => c.CharmId == charmId && c.IsActive == true);
		}

		public async Task<CustomBracelet?> GetCustomBraceletByIdAsync(Guid? customBraceletId)
		{
			return await _context.CustomBracelets
				.Include(cb => cb.CustomBraceletCharms)
				.FirstOrDefaultAsync(cb => cb.CustomBraceletId == customBraceletId);
		}

		public async Task<CustomBracelet?> GetCustomBraceletByUserIdAsync(Guid? userId)
		{
			var cart = await _cartRepository.GetCartByUserIdAsync(userId);
			if(cart == null)
			{
				return null;
			}
			var cartItem = await _context.CartItems
				.FirstOrDefaultAsync(x => x.CartId == cart.CartId && x.ProductType == true);
			if(cartItem == null)
			{
				return null; // Không có vòng tay tuỳ chỉnh trong giỏ hàng
			}
			return await _context.CustomBracelets
				.Include(cb => cb.CustomBraceletCharms)
				.FirstOrDefaultAsync(cb => cb.CustomBraceletId == cartItem.CustomBraceletId);
		}

		public async Task<List<CustomBraceletCharm>> GetCustomBraceletCharm(Guid? customBraceletId)
		{
			return await _context.CustomBraceletCharms
				.Include(x => x.Charm)
				.Where(x => x.CustomBraceletId == customBraceletId).ToListAsync();
		}
	}
}
