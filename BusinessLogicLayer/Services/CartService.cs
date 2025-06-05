using AutoMapper;
using BusinessLogicLayer.Dtos.CartDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.RepositoryContracts;

namespace BusinessLogicLayer.Services;

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

    public async Task<(bool Success, string Message)> AddToCart(Guid userId, Guid designId, int quantity, bool productType)
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
        if (cartItems == null || !cartItems.Any())
        {
            return null;
        }

        var cartItemViewModels = _mapper.Map<List<CartItemViewModel>>(cartItems);
        foreach (var (cartItem, viewModel) in cartItems.Zip(cartItemViewModels))
        {
            if (cartItem.DesignId != null)
            {
                cartItem.ProductType = false;
                var design = await _designService.GetDesignByIdAsync(cartItem.DesignId);
                viewModel.Design = design;
                // Giả sử Design có thuộc tính StockQuantity để kiểm tra số lượng tối đa
                viewModel.MaximumQuantity = design?.StockQuantity;
            }
            else if (cartItem.CustomBraceletId != null && _charmService != null)
            {
                cartItem.ProductType = true;
                var bracelet = await _charmService.GetCustomBraceletByIdAsync(cartItem.CustomBraceletId);
                viewModel.CustomBracelet = bracelet;

                var customBraceletCharms = await _charmService.GetCustomBraceletCharm(cartItem.CustomBraceletId);
                if (customBraceletCharms?.Any() == true)
                {
                    var minCharmQuantity = customBraceletCharms
                        .Select(c => c.Charm.Quantity)
                        .Min();
                    viewModel.MaximumQuantity = minCharmQuantity;
                }
            }
        }
        return cartItemViewModels;
        //if (cartItemViewModels.Count == 0)
        //{
        //    return null;
        //}
        //for (int i = 0; i < cartItemViewModels.Count; i++)
        //{
        //    var designId = cartItems[i].DesignId;
        //    var design = await _designService.GetDesignByIdAsync(designId); // gọi service bạn đã viết
        //    cartItemViewModels[i].Design = design;
        //}
        //return cartItemViewModels;
    }

    public async Task<(bool? Success, string Message)> UpdateQuantity(Guid userId, Guid? designId, int quantity, bool productType)
    {
        return await _cartRepository.UpdateQuantity(userId, designId, quantity, productType);
    }
}
