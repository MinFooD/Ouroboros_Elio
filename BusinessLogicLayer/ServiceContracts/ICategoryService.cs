using BusinessLogicLayer.Dtos.CategoryDtos;

namespace BusinessLogicLayer.ServiceContracts;

public interface ICategoryService
{
    Task<List<CategoryViewModel>?> GetAllCategoriesAsync();
}
