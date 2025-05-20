using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts
{
	public interface IDesignRepository
	{
		//Task<bool> AddDesignAsync(Design design);
		//Task<bool> UpdateDesignAsync(Design design);
		//Task<bool> DeleteDesignAsync(Guid designId);
		Task<Design?> GetDesignByIdAsync(Guid? designId);
		Task<List<Design>?> GetAllDesignsAsync(Guid? modelId);
		Task<bool> VisitCountUp(Guid designId);
		//Task<IEnumerable<Design>> GetDesignsByCategoryIdAsync(Guid categoryId);
		//Task<IEnumerable<Design>> GetTopVisitedDesignsAsync(int count);
		//Task<IEnumerable<Design>> GetTopOrderedDesignsAsync(int count);
		//Task<IEnumerable<Design>> SearchDesignsAsync(string searchTerm);
		//Task<bool> IsExistAsync(Guid designId);
	}
}
