using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.CartDtos
{
	public class CartAndListCartItemViewModel
	{
		public List<CartItemViewModel> cartItemsViewModel;
		public CartViewModel CartViewModel;
	}
}
