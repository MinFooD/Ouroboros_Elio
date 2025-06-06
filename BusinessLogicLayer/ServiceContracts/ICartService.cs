﻿using BusinessLogicLayer.Dtos.CartDtos;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServiceContracts
{
	public interface ICartService
	{
		Task<List<CartItemViewModel>?> GetCartItemsByUserIdAsync(Guid userId);
		Task<CartViewModel> GetCartByUserIdAsync(Guid userId);
		Task<bool> AddToCart(Guid userId, Guid designId, int quantity, bool productType);
		Task<bool?> UpdateQuantity(Guid userId, Guid designId, int quantity, bool? productType);
	}
}
