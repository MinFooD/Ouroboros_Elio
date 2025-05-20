using AutoMapper;
using BusinessLogicLayer.Dtos.CartDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
	public class CartService : ICartService
	{
		private readonly ICartRepository _cartRepository;
		private readonly IMapper _mapper;
		private readonly IDesignService _designService;

		public CartService(ICartRepository cartRepository, IMapper mapper, IDesignService designService)
		{
			_cartRepository = cartRepository;
			_mapper = mapper;
			_designService = designService;
		}

		public async Task<bool> AddToCart(Guid userId, Guid designId, int quantity, bool productType)
		{
			return await _cartRepository.AddToCart(userId, designId, quantity, productType);
		}

		public async Task<CartViewModel> GetCartByUserIdAsync(Guid userId)
		{
			var cart = await _cartRepository.GetCartByUserIdAsync(userId);
			var cartViewModel = _mapper.Map<CartViewModel>(cart);
			return cartViewModel;
		}

		public async Task<List<CartItemViewModel>?> GetCartItemsByUserIdAsync(Guid userId)
		{
			var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);
			var cartItemViewModels = _mapper.Map<List<CartItemViewModel>>(cartItems);

			if(cartItemViewModels.Count == 0)
			{
				return null;
			}
			for (int i = 0; i < cartItemViewModels.Count; i++)
			{
				var designId = cartItems[i].DesignId;
				var design = await _designService.GetDesignByIdAsync(designId); // gọi service bạn đã viết
				cartItemViewModels[i].Design = design;
			}
			return cartItemViewModels;
		}

		public async Task<bool?> UpdateQuantity(Guid userId, Guid designId, int quantity)
		{
			return await _cartRepository.UpdateQuantity(userId, designId, quantity);
		}
	}
}
