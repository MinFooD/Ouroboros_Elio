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
		private readonly ICharmService _charmService;

		public CartService(ICartRepository cartRepository, IMapper mapper, IDesignService designService, ICharmService charmService)
		{
			_cartRepository = cartRepository;
			_mapper = mapper;
			_designService = designService;
			_charmService = charmService;
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
				var productId = cartItems[i].DesignId;
				if(productId != null)
				{
					cartItems[i].ProductType = false;
					var design = await _designService.GetDesignByIdAsync(productId);
					cartItemViewModels[i].Design = design;
					//cartItemViewModels[i].MaxQuantity = design.Quantity;
				}
				else
				{
					cartItems[i].ProductType = true;
					productId = cartItems[i].CustomBraceletId;
					var bracelet = await _charmService.GetCustomBraceletByIdAsync(productId); 
					cartItemViewModels[i].CustomBracelet = bracelet;

					var customBraceletCharms = await _charmService.GetCustomBraceletCharm(productId);

					var minCharmQuantity = customBraceletCharms
						.Select(c => c.Charm.Quantity)
						.Min();
					cartItemViewModels[i].MaximumQuantity = minCharmQuantity;
				}
			}
			return cartItemViewModels;
		}

		public async Task<bool?> UpdateQuantity(Guid userId, Guid designId, int quantity, bool? productType)
		{
			return await _cartRepository.UpdateQuantity(userId, designId, quantity, productType);
		}
	}
}
