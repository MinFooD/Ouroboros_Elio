using AutoMapper;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.Dtos.OrderDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;

namespace BusinessLogicLayer.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;

    public AdminService(IAdminRepository adminRepository, IMapper mapper)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
    }

    public async Task<int> GetAccessCount()
    {
        return await _adminRepository.GetAccessCount();
    }

    public async Task<(List<OrderViewModel>, int)> GetAllOrder(int page, int pageSize)
    {
        var (orders, count) = await _adminRepository.GetAllOrder(page, pageSize);
        var orderViewModels = _mapper.Map<List<OrderViewModel>>(orders);
        return (orderViewModels, count);
    }

    public async Task<List<OrderItemsViewModel>> GetAllOrderItem(Guid orderId)
    {
        var orderItems = await _adminRepository.GetAllOrderItem(orderId);
        var orderItemsViewModels = _mapper.Map<List<OrderItemsViewModel>>(orderItems);
        return orderItemsViewModels;
    }

    public async Task<int> GetCustomBraceletCount()
    {
        return await _adminRepository.GetCustomBraceletCount();
    }

    public async Task<decimal[]> GetMonthlyRevenue(int year)
    {
        return await _adminRepository.GetMonthlyRevenue(year);
    }

    public async Task<int> GetOrderCount()
    {
        return await _adminRepository.GetOrderCount();
    }

    public async Task<List<DesignViewModel>> GetTopVisitedDesignsAsync(int topCount, string? sort)
    {
        var designs = await _adminRepository.GetTopVisitedDesignsAsync(topCount, sort);
        var designViewModels = _mapper.Map<List<DesignViewModel>>(designs);
        for (int i = 0; i < designs.Count; i++)
        {
            var design = designs[i];
            designViewModels[i].DesignName = $"{design.Model.ModelName}-{design.Category.CategoryName}";
            designViewModels[i].CollectionName = design.Model.Topic.Collection.CollectionName;
            designViewModels[i].ModelName = design.Model.ModelName;
            designViewModels[i].TopicName = design.Model.Topic.TopicName;
            designViewModels[i].CategoryName = design.Category.CategoryName;
            designViewModels[i].FirstImage = _mapper.Map<DesignImageViewModel>(design.DesignImages.FirstOrDefault());
        }
        return designViewModels;
    }

    public async Task<int> GetUserCountWithRoleUser()
    {
        return await _adminRepository.GetUserCountWithRoleUser();
    }

    public async Task<bool> UpdateShippingCode(Guid orderId, string shippingCode)
    {
        return await _adminRepository.UpdateShippingCode(orderId, shippingCode);
    }

    public async Task<List<ApplicationUser>> GetAllUser(int pageNumber, int pageSize)
    {
        var users = await _adminRepository.GetAllUser(pageNumber, pageSize);
        return users ?? new List<ApplicationUser>();
    }
}
