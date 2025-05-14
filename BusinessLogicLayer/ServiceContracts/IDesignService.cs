using BusinessLogicLayer.Dtos.DesignDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServiceContracts
{
	public interface IDesignService
	{
		Task<DesignViewModel?> GetDesignByIdAsync(Guid designId);
		Task<List<DesignViewModel>?> GetAllDesignsAsync();
	}
}
