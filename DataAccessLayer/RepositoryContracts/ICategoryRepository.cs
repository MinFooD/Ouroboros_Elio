using DataAccessLayer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts;

public interface ICategoryRepository
{
    Task<List<Category>?> GetAllCategoriesAsync();
}